using Microsoft.Extensions.ObjectPool;
using SysDVR.Client.Core;
using SysDVR.Client.Targets.Decoding;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static SysDVR.Client.ThirdParty.Openh264.OpenH264Native;

namespace SysDVR.Client.ThirdParty.Openh264
{
	// https://github1s.com/cisco/openh264/blob/master/codec/api/wels/codec_def.h#L187-L193
	// https://github.com/cisco/openh264/wiki/UsageExampleForDecoder

	internal unsafe class OpenH264Native
	{
		public enum DecodingState
		{
			dsErrorFree = 0x00,
			dsFramePending = 0x01,
			dsRefLost = 0x02,
			dsBitstreamError = 0x04,
			dsDepLayerLost = 0x08,
			dsNoParamSets = 0x10,
			dsDataErrorConcealed = 0x20,
			dsRefListNullPtrs = 0x40,
			dsInvalidArgument = 0x1000,
			dsInitialOptExpected = 0x2000,
			dsOutOfMemory = 0x4000,
			dsDstBufNeedExpan = 0x8000
		};

		public enum ERROR_CON_IDC
		{
			DISABLE = 0,
			FRAME_COPY,
			SLICE_COPY,
			FRAME_COPY_CROSS_IDR,
			SLICE_COPY_CROSS_IDR,
			SLICE_COPY_CROSS_IDR_FREEZE_RES_CHANGE,
			SLICE_MV_COPY_CROSS_IDR,
			SLICE_MV_COPY_CROSS_IDR_FREEZE_RES_CHANGE
		}

		public enum VIDEO_BITSTREAM_TYPE
		{
			AVC = 0,
			SVC = 1,
			DEFAULT = SVC
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SVideoProperty
		{
			public uint size;          ///< size of the struct
			public VIDEO_BITSTREAM_TYPE eVideoBsType;  ///< video stream type (AVC/SVC)
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SDecodingParam
		{
			public char* pFileNameRestructed;
			public uint uiCpuLoad;             ///< CPU load
			public byte uiTargetDqLayer;       ///< setting target dq layer id

			public ERROR_CON_IDC eEcActiveIdc;          ///< whether active error concealment feature in decoder
			public bool bParseOnly;                     ///< decoder for parse only, no reconstruction. When it is true, SPS/PPS size should not exceed SPS_PPS_BS_SIZE (128). Otherwise, it will return error info

			public SVideoProperty sVideoProperty;    ///< video stream property
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct SSysMEMBuffer
		{
			public int iWidth;
			public int iHeight;
			public int iFormat;
			public int iStride_0;
			public int iStride_1;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SBufferInfo
		{
			public int iBufferStatus;
			public ulong uiInBsTimeStamp;
			public ulong uiOutYuvTimeStamp;
			public SSysMEMBuffer userData_sSystemBuffer;
			public byte* pDst_0;
			public byte* pDst_1;
			public byte* pDst_2;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ISVCDecoder
		{
			public ISVCDecoderVtbl* Vtbl;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ISVCDecoderVtbl
		{
			public IntPtr Initialize;
			public IntPtr Uninitialize;
			public IntPtr DecodeFrame;
			public IntPtr DecodeFrameNoDelay;
			public IntPtr DecodeFrame2;
			public IntPtr FlushFrame;
			public IntPtr DecodeParser;
			public IntPtr DecodeFrameEx;
			public IntPtr SetOption;
			public IntPtr GetOption;
		}

		public struct ISVCDecoderVtblManaged
		{
			public ISVCdecoderInitialize Initialize;
			public ISVCdecoderUninitialize Uninitialize;
			public ISVCdecoderDecodeFrameNoDelay DecodeFrameNoDelay;

			public delegate long ISVCdecoderInitialize(ISVCDecoder* decoder, SDecodingParam* param);

			public delegate long ISVCdecoderUninitialize(ISVCDecoder* decoder);

			public delegate DecodingState ISVCdecoderDecodeFrameNoDelay(
					ISVCDecoder* decoder, byte* pSrc, int iSrcLen,
					byte** ppDst, SBufferInfo* pDstInfo);

			public ISVCDecoderVtblManaged(in ISVCDecoderVtbl vtbl)
			{
				Initialize = Marshal.GetDelegateForFunctionPointer<ISVCdecoderInitialize>(vtbl.Initialize);
				Uninitialize = Marshal.GetDelegateForFunctionPointer<ISVCdecoderUninitialize>(vtbl.Uninitialize);
				DecodeFrameNoDelay = Marshal.GetDelegateForFunctionPointer<ISVCdecoderDecodeFrameNoDelay>(vtbl.DecodeFrameNoDelay);
			}
		}

		[DllImport("openh264", CallingConvention = CallingConvention.Cdecl)]
		public static extern long WelsCreateDecoder(ISVCDecoder** ppDecoder);

		[DllImport("openh264", CallingConvention = CallingConvention.Cdecl)]
		public static extern long WelsDestroyDecoder(ISVCDecoder** ppDecoder);
	}

	internal class OpenH264Decoder : IDisposable
	{
		unsafe OpenH264Native.ISVCDecoder* decoder;
		ISVCDecoderVtblManaged vtable;

		unsafe public OpenH264Decoder()
		{
			OpenH264Native.ISVCDecoder* decoder;
			if (OpenH264Native.WelsCreateDecoder(&decoder) != 0)
			{
				throw new InvalidOperationException("Failed to create decoder");
			}

			if (decoder == null || decoder->Vtbl == null)
			{
				throw new Exception("Decoder pointers are not valid");
			}

			this.decoder = decoder;
			this.vtable = new ISVCDecoderVtblManaged(in *decoder->Vtbl);
		}

		unsafe public void Initialize(in SDecodingParam param)
		{
			fixed (SDecodingParam* pParam = &param)
			{
				if (vtable.Initialize(decoder, pParam) != 0)
				{
					throw new InvalidOperationException("Failed to initialize decoder");
				}
			}
		}

		unsafe public DecodingState DecodeNoDelay(Span<byte> data, IntPtr outY, IntPtr outU, IntPtr outV, ref SBufferInfo info)
		{
			fixed (SBufferInfo* pBufInfo = &info)
				fixed(byte* ptr = data)
			{
				byte** dst = stackalloc byte*[3] { (byte*)outY, (byte*)outU, (byte*)outV };
				return vtable.DecodeFrameNoDelay(decoder, ptr, data.Length, dst, pBufInfo);
			}
		}

		unsafe public void Dispose()
		{
			if (decoder != null)
			{
				var tmpDec = decoder;
				OpenH264Native.WelsDestroyDecoder(&tmpDec);
				decoder = null;
			}
		}
	}

	internal class OpenH264PlayerTarget : OutStream
	{
		readonly OpenH264Decoder Decoder;
		readonly AutoResetEvent onFrame = new AutoResetEvent(false);

		readonly BlockingCollection<PoolBuffer> Decode = new();
		readonly BlockingCollection<PlanarYUVFrame> Render = new();

		readonly ObjectPool<PlanarYUVFrame> FramePool =
			ObjectPool.Create<PlanarYUVFrame>();

		Task DecodingTask;

		public OpenH264PlayerTarget() 
		{
			Decoder = new OpenH264Decoder();
			Decoder.Initialize(new()
			{
				sVideoProperty = new()
				{
					size = (uint)Marshal.SizeOf<SVideoProperty>(),
					eVideoBsType = VIDEO_BITSTREAM_TYPE.AVC
				}
			});

			DecodingTask = Task.Run(DecodeLoop);
		}

		public PlanarYUVFrame AcquireFrame()
		{
			return FramePool.Get();
		}

		public void ReleaseFrame(PlanarYUVFrame frame)
		{
			FramePool.Return(frame);
		}

		public bool SwapBuffers(ref PlanarYUVFrame frame)
		{
			if (!Render.TryTake(out var doneFrame))
				return false;

			ReleaseFrame(frame);
			frame = doneFrame;

			return true;
		}

		unsafe void DecodeLoop() 
		{
			foreach (var block in Decode.GetConsumingEnumerable())
			{
				using var trace = new TimeTrace("DecodeFrame");

				var info = new SBufferInfo();
				var frame = FramePool.Get();
				var state = Decoder.DecodeNoDelay(block.Span, frame.Y.Pinned, frame.U.Pinned, frame.V.Pinned, ref info);

				if (state == DecodingState.dsErrorFree && info.iBufferStatus == 1)
				{
					frame.YDecoded = (IntPtr)info.pDst_0;
					frame.UDecoded = (IntPtr)info.pDst_1;
					frame.VDecoded = (IntPtr)info.pDst_2;

					frame.YLineSize = info.userData_sSystemBuffer.iStride_0;
					frame.ULineSize = info.userData_sSystemBuffer.iStride_1;
					frame.VLineSize = info.userData_sSystemBuffer.iStride_1;

					// On success, push this frame to the render queue
					Render.Add(frame);
				}
				else
				{
					// On failure, return this frame to the pool
					FramePool.Return(frame);
				}

				block.Free();
			}
		}

		protected override void SendDataImpl(PoolBuffer block, ulong ts)
		{
			Decode.Add(block);
		}

		protected override void DisposeImpl()
		{
			Decoder.Dispose();
			base.DisposeImpl();
		}
	}
}
