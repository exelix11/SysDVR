using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsbStream.RTSP
{
	public static class LE16Packetizer
	{
		private const double SampleRate = 48000;
		private const double SamplesInRtpPacket = RTPPacketUtil.MaxPayloadSize / 4;
		private const double AudiolenRtpPackeMs = SamplesInRtpPacket / SampleRate * 1000;

		private const int rtp_version = 2;
		private const int rtp_padding = 0;
		private const int rtp_extension = 0;
		private const int rtp_csrc_count = 0;
		private const int rtp_payload_type = 97;

		//TODO: reimplement these using Memory and Span<byte> 

		public static List<byte[]> PacketizeSamples(Memory<byte> samples, ulong tsMs)
		{
			List<byte[]> rtp_packets = new List<byte[]>();

			Span<byte> smp = samples.Span;
			double diffTs = 0;

			while (smp.Length > 0)
			{
				int dataLen = smp.Length > RTPPacketUtil.MaxPayloadSize ? RTPPacketUtil.MaxPayloadSize : smp.Length;

				byte[] packet = new byte[dataLen + RTPPacketUtil.HeaderLength];
				Span<byte> packetData = new Span<byte>(packet, RTPPacketUtil.HeaderLength, dataLen);

				for (int i = 0; i < dataLen; i += 2)
				{
					packetData[i] = smp[i + 1];
					packetData[i + 1] = smp[i];
				}

				RTPPacketUtil.WriteHeader(packet, rtp_version, rtp_padding, rtp_extension, rtp_csrc_count, 0, rtp_payload_type);

				UInt32 empty_sequence_id = 0;
				RTPPacketUtil.WriteSequenceNumber(packet, empty_sequence_id);

				RTPPacketUtil.WriteTS(packet, (UInt32)((tsMs + diffTs) * 48));

				UInt32 empty_ssrc = 0;
				RTPPacketUtil.WriteSSRC(packet, empty_ssrc);

				rtp_packets.Add(packet);

				smp = smp.Slice(dataLen);
				diffTs += AudiolenRtpPackeMs;
			}

			return rtp_packets;
		}
	}
}
