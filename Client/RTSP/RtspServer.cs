using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Rtsp;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SysDVRClient.RTSP
{
	class RtspServer : IDisposable
	{
		const uint global_ssrc = 0; // 8 hex digits

		private TcpListener _RTSPServerListener;
		private ManualResetEvent _Stopping;
		private Thread _ListenTread;

		private SysDVRVideoRTSPTarget video_source = null;
		private SysDVRAudioRTSPTarget audio_source = null;

		enum StreamKind : int
		{
			Video = 0,
			Audio = 1
		}

		byte[] raw_sps = null;
		byte[] raw_pps = null;

		List<RTSPConnection> rtsp_list = new List<RTSPConnection>(); // list of RTSP Listeners

		int session_handle = 1;

		private int portNumber;

		/// <summary>
		/// Initializes a new instance of the <see cref="RTSPServer"/> class.
		/// </summary>
		/// <param name="aPortNumber">A numero port.</param>
		public RtspServer(int portNumber, SysDVRVideoRTSPTarget video_source, SysDVRAudioRTSPTarget audio_source, bool localOnly = false)
		{
			if (portNumber < System.Net.IPEndPoint.MinPort || portNumber > System.Net.IPEndPoint.MaxPort)
				throw new ArgumentOutOfRangeException("aPortNumber", portNumber, "Port number must be between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort");
			Contract.EndContractBlock();

			this.video_source = video_source;
			this.audio_source = audio_source;

			if (video_source != null)
				video_source.DataAvailable += SysDVR_ReceivedVideoFrame;
			if (audio_source != null)
				audio_source.DataAvailable += SysDVR_ReceivedAudioData;

			this.portNumber = portNumber;
			//RtspUtils.RegisterUri();
			_RTSPServerListener = new TcpListener(localOnly ? IPAddress.Loopback : IPAddress.Any, portNumber);
		}

		/// <summary>
		/// Starts the listen.
		/// </summary>
		public void StartListenerThread()
		{
			_RTSPServerListener.Start();

			_Stopping = new ManualResetEvent(false);
			_ListenTread = new Thread(new ThreadStart(AcceptConnection));
			_ListenTread.Start();
		}

		/// <summary>
		/// Accepts the connection.
		/// </summary>
		private void AcceptConnection()
		{
			Console.WriteLine($"Now streaming via RTSP on port {portNumber}");
			Console.WriteLine($"Connect with your player to rtsp://127.0.0.1:{portNumber}/");
			try
			{
				while (!_Stopping.WaitOne(0))
				{
					// Wait for an incoming TCP Connection
					TcpClient oneClient = _RTSPServerListener.AcceptTcpClient();
					Console.WriteLine("Connection from " + oneClient.Client.RemoteEndPoint.ToString());

					// Hand the incoming TCP connection over to the RTSP classes
					var rtsp_socket = new RtspTcpTransport(oneClient);
					RtspListener newListener = new RtspListener(rtsp_socket);
					newListener.MessageReceived += RTSP_Message_Received;
					//RTSPDispatcher.Instance.AddListener(newListener);

					// Add the RtspListener to the RTSPConnections List
					lock (rtsp_list)
					{
						RTSPConnection new_connection = new RTSPConnection();
						new_connection.listener = newListener;
						new_connection.client_hostname = newListener.RemoteAdress.Split(':')[0];
						new_connection.ssrc = global_ssrc;

						new_connection.time_since_last_rtsp_keepalive = DateTime.UtcNow;
						//new_connection.video_time_since_last_rtcp_keepalive = DateTime.UtcNow;

						rtsp_list.Add(new_connection);
					}

					newListener.Start();
				}
			}
			catch (Exception error)
			{
				if (!_Stopping.WaitOne(0))
					Console.WriteLine("[AcceptConnection] Error: " + error.ToString());
			}
		}

		public void StopListen()
		{
			lock (rtsp_list) {
				foreach (RTSPConnection connection in rtsp_list)
					connection.Dispose();
				rtsp_list.Clear();
			}
			_RTSPServerListener.Stop();
			_Stopping.Set();
			_ListenTread.Join();
		}

		#region IDisposable Membres

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				StopListen();
				_Stopping.Dispose();
			}
		}

		#endregion

		// Process each RTSP message that is received
		private void RTSP_Message_Received(object sender, RtspChunkEventArgs e)
		{
			// Cast the 'sender' and 'e' into the RTSP Listener (the Socket) and the RTSP Message
			Rtsp.RtspListener listener = sender as Rtsp.RtspListener;
			Rtsp.Messages.RtspMessage message = e.Message as Rtsp.Messages.RtspMessage;

			Console.WriteLine("RTSP message received " + message);

			// Update the RTSP Keepalive Timeout
			// We could check that the message is GET_PARAMETER or OPTIONS for a keepalive but instead we will update the timer on any message
			lock (rtsp_list)
			{
				foreach (RTSPConnection connection in rtsp_list)
				{
					if (connection.listener.RemoteAdress.Equals(listener.RemoteAdress))
					{
						// found the connection
						connection.time_since_last_rtsp_keepalive = DateTime.UtcNow;
						break;

					}
				}
			}

			#region Handle OPTIONS message
			if (message is Rtsp.Messages.RtspRequestOptions)
			{
				// Create the reponse to OPTIONS
				Rtsp.Messages.RtspResponse options_response =
					(e.Message as Rtsp.Messages.RtspRequestOptions).CreateResponse("DESCRIBE,SETUP,PLAY,TEARDOWN");
				listener.SendMessage(options_response);
			}
			#endregion

			#region Handle DESCRIBE message
			if (message is Rtsp.Messages.RtspRequestDescribe)
			{
				String requested_url = (message as Rtsp.Messages.RtspRequestDescribe).RtspUri.ToString();
				Console.WriteLine("Request for " + requested_url);

				// TODO. Check the requsted_url is valid. In this example we accept any RTSP URL

				// Make the Base64 SPS and PPS
				raw_sps = StreamInfo.SPS; // no 0x00 0x00 0x00 0x01 or 32 bit size header
				raw_pps = StreamInfo.PPS; // no 0x00 0x00 0x00 0x01 or 32 bit size header
				String sps_str = Convert.ToBase64String(raw_sps);
				String pps_str = Convert.ToBase64String(raw_pps);

				StringBuilder sdp = new StringBuilder();

				// Generate the SDP
				// The sprop-parameter-sets provide the SPS and PPS for H264 video
				// The packetization-mode defines the H264 over RTP payloads used but is Optional
				sdp.Append("v=0\n");
				sdp.Append("o=user 123 0 IN IP4 0.0.0.0\n");
				sdp.Append($"s=SysDVR - https://github.com/exelix11/sysdvr - [PID {Process.GetCurrentProcess().Id}]\n");
				if (video_source != null)
				{
					sdp.Append("m=video 0 RTP/AVP 96\n");
					sdp.Append("c=IN IP4 0.0.0.0\n");
					sdp.Append("a=control:trackID=0\n");
					sdp.Append("a=rtpmap:96 H264/90000\n");
					sdp.Append("a=fmtp:96 profile-level-id=42A01E; sprop-parameter-sets=" + sps_str + "," + pps_str + ";\n");
				}
				if (audio_source != null)
				{
					sdp.Append("m=audio 0 RTP/AVP 97\n");
					sdp.Append("a=rtpmap:97 L16/48000/2\n");
					sdp.Append("a=control:trackID=1\n");
				}

				byte[] sdp_bytes = Encoding.ASCII.GetBytes(sdp.ToString());

				// Create the reponse to DESCRIBE
				// This must include the Session Description Protocol (SDP)
				Rtsp.Messages.RtspResponse describe_response = (e.Message as Rtsp.Messages.RtspRequestDescribe).CreateResponse();

				describe_response.AddHeader("Content-Base: " + requested_url);
				describe_response.AddHeader("Content-Type: application/sdp");
				describe_response.Data = sdp_bytes;
				describe_response.AdjustContentLength();
				listener.SendMessage(describe_response);
			}
			#endregion

			#region Handle SETUP message
			if (message is Rtsp.Messages.RtspRequestSetup)
			{
				var setupMessage = message as Rtsp.Messages.RtspRequestSetup;

				// Check the RTSP transport
				// If it is UDP or Multicast, create the sockets
				// If it is RTP over RTSP we send data via the RTSP Listener

				// FIXME client may send more than one possible transport.
				// very rare
				Rtsp.Messages.RtspTransport transport = setupMessage.GetTransports()[0];

				// Construct the Transport: reply from the Server to the client
				Rtsp.Messages.RtspTransport transport_reply = new Rtsp.Messages.RtspTransport();
				transport_reply.SSrc = global_ssrc.ToString("X8"); // Convert to Hex, padded to 8 characters

				if (transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.TCP)
				{
					// RTP over RTSP mode
					transport_reply.LowerTransport = Rtsp.Messages.RtspTransport.LowerTransportType.TCP;
					transport_reply.Interleaved = new Rtsp.Messages.PortCouple(transport.Interleaved.First, transport.Interleaved.Second);
				}

				Rtsp.UDPSocket udp_pair = null;
				if (transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.UDP
					&& transport.IsMulticast == false)
				{
					Boolean udp_supported = true;
					if (udp_supported)
					{
						// RTP over UDP mode
						// Create a pair of UDP sockets - One is for the Video, one is for the RTCP
						udp_pair = new Rtsp.UDPSocket(50000, 51000); // give a range of 500 pairs (1000 addresses) to try incase some address are in use
						udp_pair.DataReceived += (object local_sender, RtspChunkEventArgs local_e) =>
						{
							// RTCP data received
							Console.WriteLine("RTCP data received " + local_sender.ToString() + " " + local_e.ToString());
						};
						udp_pair.Start(); // start listening for data on the UDP ports

						// Pass the Port of the two sockets back in the reply
						transport_reply.LowerTransport = Rtsp.Messages.RtspTransport.LowerTransportType.UDP;
						transport_reply.IsMulticast = false;
						transport_reply.ClientPort = new Rtsp.Messages.PortCouple(udp_pair.data_port, udp_pair.control_port);
					}
					else
					{
						transport_reply = null;
					}
				}

				if (transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.UDP
					&& transport.IsMulticast == true)
				{
					// RTP over Multicast UDP mode}
					// Create a pair of UDP sockets in Multicast Mode
					// Pass the Ports of the two sockets back in the reply
					transport_reply.LowerTransport = Rtsp.Messages.RtspTransport.LowerTransportType.UDP;
					transport_reply.IsMulticast = true;
					transport_reply.Port = new Rtsp.Messages.PortCouple(7000, 7001);  // FIX

					// for now until implemented
					transport_reply = null;
				}


				if (transport_reply != null)
				{
					// Update the session with transport information
					String copy_of_session_id = "";
					lock (rtsp_list)
					{
						foreach (RTSPConnection connection in rtsp_list)
						{
							if (connection.listener.RemoteAdress.Equals(listener.RemoteAdress))
							{
								var stream = int.Parse(setupMessage.RtspUri.LocalPath.Split('=')[1]);
								connection.sessions[stream].is_active = true;
								// found the connection
								// Add the transports to the connection
								connection.sessions[stream].client_transport = transport;
								connection.sessions[stream].transport_reply = transport_reply;

								// If we are sending in UDP mode, add the UDP Socket pair and the Client Hostname
								connection.sessions[stream].udp_pair = udp_pair;
								
								connection.sessions[stream].session_id = session_handle.ToString();
								session_handle++;

								// Copy the Session ID
								copy_of_session_id = connection.sessions[stream].session_id;
								break;
							}
						}
					}

					Rtsp.Messages.RtspResponse setup_response = setupMessage.CreateResponse();
					setup_response.Headers[Rtsp.Messages.RtspHeaderNames.Transport] = transport_reply.ToString();
					setup_response.Session = copy_of_session_id;
					listener.SendMessage(setup_response);
				}
				else
				{
					Rtsp.Messages.RtspResponse setup_response = setupMessage.CreateResponse();
					// unsuported transport
					setup_response.ReturnCode = 461;
					listener.SendMessage(setup_response);
				}

			}
			#endregion

			#region Handle PLAY message 
			// Must have a Session ID
			if (message is Rtsp.Messages.RtspRequestPlay)
			{
				lock (rtsp_list)
				{
					// Search for the Session in the Sessions List. Change the state to "PLAY"
					bool session_found = false;
					foreach (RTSPConnection connection in rtsp_list)
					{
						if (message.Session == connection.video.session_id || message.Session == connection.audio.session_id)
						{
							// found the session
							session_found = true;
							connection.play = true;  // ACTUALLY YOU COULD PAUSE JUST THE VIDEO (or JUST THE AUDIO)

							string range = "npt=0-";   // Playing the 'video' from 0 seconds until the end
							string rtp_info = "url=" + ((Rtsp.Messages.RtspRequestPlay)message).RtspUri + ";seq=" + connection.video.sequence_number; // TODO Add rtptime  +";rtptime="+session.rtp_initial_timestamp;

							// Send the reply
							Rtsp.Messages.RtspResponse play_response = (e.Message as Rtsp.Messages.RtspRequestPlay).CreateResponse();
							play_response.AddHeader("Range: " + range);
							play_response.AddHeader("RTP-Info: " + rtp_info);
							listener.SendMessage(play_response);

							break;
						}
					}

					if (session_found == false)
					{
						// Session ID was not found in the list of Sessions. Send a 454 error
						Rtsp.Messages.RtspResponse play_failed_response = (e.Message as Rtsp.Messages.RtspRequestPlay).CreateResponse();
						play_failed_response.ReturnCode = 454; // Session Not Found
						listener.SendMessage(play_failed_response);
					}
				}
			}
			#endregion

			#region Handle TEARDOWN
			// Must have a Session ID
			if (message is Rtsp.Messages.RtspRequestTeardown)
			{
				lock (rtsp_list)
				{
					// Search for the Session in the Sessions List.
					foreach (RTSPConnection connection in rtsp_list.ToArray()) // Convert to ToArray so we can delete from the rtp_list
					{
						rtsp_list.Remove(connection);
						connection.Dispose();
						listener.Dispose();
					}
				}
			}
			#endregion
		}

		int GetPlayCountPerStream(StreamKind stream)
		{
			DateTime now = DateTime.UtcNow;
			const int timeout_in_seconds = 70;

			int s = (int)stream;

			int current_rtp_play_count = 0;
			lock (rtsp_list)
			{
				foreach (RTSPConnection connection in rtsp_list.ToArray())
				{ // Convert to Array to allow us to delete from rtsp_list
				  // RTSP Timeout (clients receiving RTP video over the RTSP session
				  // do not need to send a keepalive (so we check for Socket write errors)
					Boolean sending_rtp_via_tcp = false;
					if ((connection.sessions[s].client_transport != null) &&
						(connection.sessions[s].client_transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.TCP))
					{
						sending_rtp_via_tcp = true;
					}

					if (sending_rtp_via_tcp == false && ((now - connection.time_since_last_rtsp_keepalive).TotalSeconds > timeout_in_seconds))
					{
						Console.WriteLine("Removing session " + connection.sessions[s].session_id + " due to TIMEOUT");
						rtsp_list.Remove(connection);
						connection.Dispose();
						continue;
					}
					else if (connection.play) current_rtp_play_count++;
				}
			}
			return current_rtp_play_count;
		}

		void SysDVR_ReceivedVideoFrame(Span<byte> Data, ulong tsMsec)
		{
			if (GetPlayCountPerStream(StreamKind.Video) == 0) return;
			var rtp_packets = H264Packetizer.PacketizeNALArray(Data, tsMsec);
			PushRtspData(StreamKind.Video, rtp_packets, tsMsec);
		}

		private void SysDVR_ReceivedAudioData(Span<byte> Data, ulong tsMsec)
		{
			if (GetPlayCountPerStream(StreamKind.Audio) == 0) return;
			var samples = LE16Packetizer.PacketizeSamples(Data, tsMsec);
			PushRtspData(StreamKind.Audio, samples, tsMsec);
		}

		void PushRtspData(StreamKind stream, List<byte[]> rtp_packets, ulong tsMsec)
		{
			int s = (int)stream;

			lock (rtsp_list)
			{

				// Go through each RTSP connection and output the NAL on the Video Session
				foreach (RTSPConnection connection in rtsp_list.ToArray()) // ToArray makes a temp copy of the list.
																		   // This lets us delete items in the foreach
																		   // eg when there is Write Error
				{
					// Only process Sessions in Play Mode
					if (connection.play == false) continue;
#if DEBUG && LOG
					String connection_type = "";
					if (connection[s].client_transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.TCP) connection_type = "TCP";
					if (connection[s].client_transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.UDP
						&& connection[s].client_transport.IsMulticast == false) connection_type = "UDP";
					if (connection[s].client_transport.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.UDP
						&& connection[s].client_transport.IsMulticast == true) connection_type = "Multicast";
					Console.WriteLine($"[{stream}] Sending video session {connection[s].session_id} {connection_type} Timestamp(ms)={tsMsec} Sequence={connection[s].sequence_number}");
#endif

					// There could be more than 1 RTP packet (if the data is fragmented)
					Boolean write_error = false;
					foreach (byte[] rtp_packet in rtp_packets)
					{
						// Add the specific data for each transmission
						RTPPacketUtil.WriteSequenceNumber(rtp_packet, connection[s].sequence_number);
						connection[s].sequence_number++;

						// Add the specific SSRC for each transmission
						RTPPacketUtil.WriteSSRC(rtp_packet, connection.ssrc);

						//if (stream == StreamKind.Video) bin.Write(rtp_packet);

						// Send as RTP over RTSP (Interleaved)
						if (connection[s].transport_reply.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.TCP)
						{
							int video_channel = connection[s].transport_reply.Interleaved.First; // second is for RTCP status messages)
							object state = new object();
							try
							{
								// send the whole NAL. With RTP over RTSP we do not need to Fragment the NAL (as we do with UDP packets or Multicast)
								//session.listener.BeginSendData(video_channel, rtp_packet, new AsyncCallback(session.listener.EndSendData), state);
								connection.listener.SendData(video_channel, rtp_packet);
							}
							catch
							{
								Console.WriteLine($"[{stream}] Error writing to listener " + connection.listener.RemoteAdress);
								write_error = true;
								break; // exit out of foreach loop
							}
						}

						// Send as RTP over UDP
						if (connection[s].transport_reply.LowerTransport == Rtsp.Messages.RtspTransport.LowerTransportType.UDP && connection[s].transport_reply.IsMulticast == false)
						{
							try
							{
								// send the whole NAL. ** We could fragment the RTP packet into smaller chuncks that fit within the MTU
								// Send to the IP address of the Client
								// Send to the UDP Port the Client gave us in the SETUP command
								connection[s].udp_pair.Write_To_Data_Port(rtp_packet, connection.client_hostname, connection[s].client_transport.ClientPort.First);
							}
							catch (Exception e)
							{
								Console.WriteLine($"[{stream}] UDP Write Exception " + e.ToString());
								Console.WriteLine($"[{stream}] Error writing to listener " + connection.listener.RemoteAdress);
								write_error = true;
								break; // exit out of foreach loop
							}
						}

						// TODO. Add Multicast
					}
					if (write_error)
					{
						Console.WriteLine($"[{stream}] Removing session " + connection[s].session_id + " due to write error");
						connection.sessions[s].Close();
						rtsp_list.Remove(connection);
						connection.Dispose();
					}
				}
			}
		}

		public class RTSPConnection : IDisposable
		{
			public Rtsp.RtspListener listener = null;  // The RTSP client connection
			public bool play = false;                  // set to true when Session is in Play mode
			public DateTime time_since_last_rtsp_keepalive = DateTime.UtcNow; // Time since last RTSP message received - used to spot dead UDP clients
			public UInt32 ssrc = 0;                    // SSRC value used with this client connection
			public String client_hostname = "";        // Client Hostname/IP Address-

			public class SessionInfo
			{
				public bool is_active = false;

				public String session_id = "";             // RTSP Session ID used with this client connection
				public UInt16 sequence_number = 1;         // 16 bit RTP packet sequence number used with this client connection
				public Rtsp.Messages.RtspTransport client_transport; // Transport: string from the client to the server
				public Rtsp.Messages.RtspTransport transport_reply; // Transport: reply from the server to the client
				public Rtsp.UDPSocket udp_pair = null;     // Pair of UDP sockets (data and control) used when sending via UDP
														   //public DateTime video_time_since_last_rtcp_keepalive = DateTime.UtcNow; // Time since last RTCP message received - used to spot dead UDP clients

				public void Close()
				{
					is_active = false;
					// If this is UDP, close the transport
					// For TCP there is no transport to close (as RTP packets were interleaved into the RTSP connection)
					udp_pair?.Stop();
				}
			}

			public readonly SessionInfo[] sessions = new SessionInfo[2] { new SessionInfo(), new SessionInfo() }; // 0 is video, 1 audio

			public SessionInfo this[int index] => sessions[index];

			public SessionInfo video => sessions[0];
			public SessionInfo audio => sessions[1];
			
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected void Dispose(bool managed)
			{
				if (!managed) return;
				foreach (var s in sessions)
					s.Close();
				listener?.Stop();
				listener?.Dispose();
			}

			~RTSPConnection() => Dispose(false);
		}
	}
}