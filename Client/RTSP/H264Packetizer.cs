using System;
using System.Collections.Generic;

namespace SysDVR.Client.RTSP
{
	public static class H264Util 
	{
		// Returns start and length of all NALs in the data block, excluding the start sequence 0 0 1
		public static IEnumerable<(int, int)> EnumerateNals(ArraySegment<byte> data)
		{
            int start = 0;
            for (int i = 2; i < data.Count; i++)
            {
                if (data[i - 2] == 0 && data[i - 1] == 0 && data[i] == 1)
                {
                    if (start != 0)
                    {
                        yield return (start + 1, i - start - 3);
                    }
                    start = i;
                }
            }
            if (start != 0)
            {
                yield return (start + 1, data.Count - start - 1);
            }
        }
	}

	public static class H264Packetizer
	{
		private const int rtp_version = 2;
		private const int rtp_padding = 0;
		private const int rtp_extension = 0;
		private const int rtp_csrc_count = 0;
		private const int rtp_payload_type = 96;

		private static byte[] PacketizeSingleNAL(Span<byte> raw_nal, UInt32 rtp_timestamp)
		{
			// Put the whole NAL into one RTP packet.
			// Note some receivers will have maximum buffers and be unable to handle large RTP packets.
			// Also with RTP over RTSP there is a limit of 65535 bytes for the RTP packet.

			byte[] rtp_packet = new byte[RTPPacketUtil.HeaderLength + raw_nal.Length]; // 12 is header size when there are no CSRCs or extensions
																					   // Create an single RTP fragment
			bool rtp_marker = (raw_nal[0] & 0x1F) <= 5;

			RTPPacketUtil.WriteHeader(rtp_packet, rtp_version, rtp_padding, rtp_extension, rtp_csrc_count, rtp_marker, rtp_payload_type);

			UInt32 empty_sequence_id = 0;
			RTPPacketUtil.WriteSequenceNumber(rtp_packet, empty_sequence_id);

			RTPPacketUtil.WriteTS(rtp_packet, rtp_timestamp);

			UInt32 empty_ssrc = 0;
			RTPPacketUtil.WriteSSRC(rtp_packet, empty_ssrc);

			// Now append the raw NAL
			raw_nal.CopyTo(new Span<byte>(rtp_packet, 12, raw_nal.Length));

			return rtp_packet;
		}

		private static void PacketizeNAL_FUA(ref List<byte[]> rtp_packets, Span<byte> raw_nal, UInt32 rtp_timestamp)
		{
			int start_bit = 1;
			int end_bit = 0;

			// consume first byte of the raw_nal. It is used in the FU header
			byte first_byte = raw_nal[0];
			raw_nal = raw_nal.Slice(1);

			while (raw_nal.Length > 0)
			{
				int payload_size = Math.Min(RTPPacketUtil.MaxPayloadSize - 2, raw_nal.Length);
				
				if (raw_nal.Length - payload_size == 0) end_bit = 1;
				//for FU-A the marker is set when this is the last RTP packet
				bool rtp_marker = end_bit == 1;

				byte[] rtp_packet = new byte[RTPPacketUtil.HeaderLength + 2 + payload_size]; // 2 bytes for FU-A header.

				RTPPacketUtil.WriteHeader(rtp_packet, rtp_version, rtp_padding, rtp_extension, rtp_csrc_count, rtp_marker, rtp_payload_type);

				UInt32 empty_sequence_id = 0;
				RTPPacketUtil.WriteSequenceNumber(rtp_packet, empty_sequence_id);

				RTPPacketUtil.WriteTS(rtp_packet, rtp_timestamp);

				UInt32 empty_ssrc = 0;
				RTPPacketUtil.WriteSSRC(rtp_packet, empty_ssrc);

				// Now append the Fragmentation Header (with Start and End marker) and part of the raw_nal
				byte f_bit = 0;
				byte nri = (byte)((first_byte >> 5) & 0x03); // Part of the 1st byte of the Raw NAL (NAL Reference ID)
				byte type = 28; // FU-A Fragmentation

				rtp_packet[12] = (byte)((f_bit << 7) + (nri << 5) + type);
				rtp_packet[13] = (byte)((start_bit << 7) + (end_bit << 6) + (0 << 5) + (first_byte & 0x1F));

				raw_nal.Slice(0, payload_size).CopyTo(new Span<byte>(rtp_packet, 14, payload_size));

				raw_nal = raw_nal.Slice(payload_size);

				rtp_packets.Add(rtp_packet);

				start_bit = 0;
			}
		}

		public static List<byte[]> PacketizeSingleNAL(Span<byte> raw_nal, ulong tsMsec)
		{
			// Build a list of 1 or more RTP packets
			// The last packet will have the M bit set to '1'
			List<byte[]> rtp_packets = new List<byte[]>();

			UInt32 rtp_timestamp = (UInt32)tsMsec * 90; // 90kHz clock

			// The H264 Payload could be sent as one large RTP packet (assuming the receiver can handle it)
			// or as a Fragmented Data, split over several RTP packets with the same Timestamp.
			bool fragmenting = false;
			if (raw_nal.Length > RTPPacketUtil.MaxPayloadSize) fragmenting = true;

			if (fragmenting == false)
				rtp_packets.Add(PacketizeSingleNAL(raw_nal, rtp_timestamp));
			else
				PacketizeNAL_FUA(ref rtp_packets, raw_nal, rtp_timestamp);

			return rtp_packets;
		}
	}
}
