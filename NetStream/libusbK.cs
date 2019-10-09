#region Copyright (c) Travis Robinson

// Copyright (c) 2012 Travis Robinson <libusbdotnet@gmail.com>
// All rights reserved.
//
// C# libusbK Bindings
// Auto-generated on: 04.28.2011
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL TRAVIS LEE ROBINSON
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable UnassignedReadonlyField

namespace libusbK
{
	public static class AllKOptions
	{
		#region Public Members

		/// <summary>
		///   Alternate libusbK library to use.  This must be assigned before any libusbK functions are called and it must be the full path and file name to a libusbK.dll.
		/// </summary>
		public static string LIBUSBK_FULLPATH_TO_ALTERNATE_DLL;

		#endregion
	}

	public static class AllKConstants
	{
		#region Public Members

		/// <summary>
		/// Allocated length for all strings in a \ref KLST_DEVINFO structure.
		/// </summary>
		public const int KLST_STRING_MAX_LEN = 256;

		/// <summary>
		/// libusbK library
		/// </summary>
		public const string LIBUSBK_DLL = "libusbK.dll";

		/// <summary>
		/// Config power mask for the \c bmAttributes field of a \ref USB_CONFIGURATION_DESCRIPTOR
		/// </summary>
		public const byte USB_CONFIG_POWERED_MASK = 0xC0;

		/// <summary>
		/// Endpoint address mask for the \c bEndpointAddress field of a \ref USB_ENDPOINT_DESCRIPTOR
		/// </summary>
		public const byte USB_ENDPOINT_ADDRESS_MASK = 0x0F;

		/// <summary>
		/// Endpoint direction mask for the \c bEndpointAddress field of a \ref USB_ENDPOINT_DESCRIPTOR
		/// </summary>
		public const byte USB_ENDPOINT_DIRECTION_MASK = 0x80;

		#endregion
	}

	public enum PipePolicyType
	{
		SHORT_PACKET_TERMINATE = 0x01,
		AUTO_CLEAR_STALL = 0x02,
		PIPE_TRANSFER_TIMEOUT = 0x03,
		IGNORE_SHORT_PACKETS = 0x04,
		ALLOW_PARTIAL_READS = 0x05,
		AUTO_FLUSH = 0x06,
		RAW_IO = 0x07,
		MAXIMUM_TRANSFER_SIZE = 0x08,
		RESET_PIPE_ON_RESUME = 0x09,

		ISO_START_LATENCY = 0x20,
		ISO_ALWAYS_START_ASAP = 0x21,
		ISO_NUM_FIXED_PACKETS = 0x22,

		SIMUL_PARALLEL_REQUESTS = 0x30,
	}

	public enum PowerPolicyType
	{
		AUTO_SUSPEND = 0x81,
		SUSPEND_DELAY = 0x83,
	}

	public enum DeviceInformationType
	{
		DEVICE_SPEED = 0x01,
	}

	public enum EndpointType
	{
		/// <summary>
		/// Indicates a control endpoint
		/// </summary>
		CONTROL = 0x00,

		/// <summary>
		/// Indicates an isochronous endpoint
		/// </summary>
		ISOCHRONOUS = 0x01,

		/// <summary>
		/// Indicates a bulk endpoint
		/// </summary>
		BULK = 0x02,

		/// <summary>
		/// Indicates an interrupt endpoint
		/// </summary>
		INTERRUPT = 0x03,

		/// <summary>
		/// Endpoint type mask for the \c bmAttributes field of a \ref USB_ENDPOINT_DESCRIPTOR
		/// </summary>
		MASK = 0x03,
	}

	public static class ErrorCodes
	{
		#region Public Members

		/// <summary>
		///   Access is denied.
		/// </summary>
		public const int AccessDenied = 5;

		/// <summary>
		///   The requested resource is in use.
		/// </summary>
		public const int Busy = 170;

		/// <summary>
		///   The operation was canceled by the user.
		/// </summary>
		public const int Cancelled = 1223;

		/// <summary>
		///   The library, drive, or media pool is empty.
		/// </summary>
		public const int Empty = 4306;

		/// <summary>
		///   The handle is invalid.
		/// </summary>
		public const int InvalidHandle = 6;

		/// <summary>
		///   The parameter is incorrect.
		/// </summary>
		public const int InvalidParameter = 87;

		/// <summary>
		///   Overlapped I/O event is not in a signaled state.
		/// </summary>
		public const int IoIncomplete = 996;

		/// <summary>
		///   Overlapped I/O operation is in progress.
		/// </summary>
		public const int IoPending = 997;

		/// <summary>
		///   More data is available.
		/// </summary>
		public const int MoreData = 234;

		/// <summary>
		///   No more data is available.
		/// </summary>
		public const int NoMoreItems = 259;

		/// <summary>
		///   Not enough storage is available to process this command.
		/// </summary>
		public const int NotEnoughMemory = 8;

		/// <summary>
		///   Element not found.
		/// </summary>
		public const int NotFound = 1168;

		/// <summary>
		///   The request is not supported.
		/// </summary>
		public const int NotSupported = 50;

		/// <summary>
		///   The I/O operation has been aborted because of either a thread exit or an application request.
		/// </summary>
		public const int OperationAborted = 995;

		/// <summary>
		///   The cluster resource is not available.
		/// </summary>
		public const int ResourceNotAvailable = 5006;

		/// <summary>
		///   The cluster resource could not be found.
		/// </summary>
		public const int ResourceNotFound = 5007;

		/// <summary>
		///   The semaphore timeout period has expired.
		/// </summary>
		public const int SemTimeout = 121;

		/// <summary>
		///   The operation completed successfully.
		/// </summary>
		public const int Success = 0;

		/// <summary>
		///   An attempt was made to operate on a thread within a specific process, but the thread specified is not in the process specified.
		/// </summary>
		public const int ThreadNotInProcess = 566;

		/// <summary>
		///   A thread termination occurred while the thread was suspended. The thread was resumed, and termination proceeded.
		/// </summary>
		public const int ThreadWasSuspended = 699;

		/// <summary>
		///   Too many dynamic-link modules are attached to this program or dynamic-link module.
		/// </summary>
		public const int TooManyModules = 214;

		#endregion
	}

	public interface IKLIB_HANDLE
	{
		#region Public Members

		IntPtr GetContext();
		KLIB_HANDLE_TYPE HandleType { get; }
		IntPtr Pointer { get; }
		bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback);
		bool SetContext(IntPtr UserContext);

		#endregion
	}

	#region Opaque library handles

	public struct KLST_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.LSTK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region Public Members

		public KLST_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	public struct KHOT_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.HOTK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region Public Members

		public KHOT_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	public struct KUSB_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.USBK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region USB Shared Device Context

		public IntPtr GetSharedContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, KLIB_HANDLE_TYPE.USBSHAREDK);
		}

		public bool SetSharedCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, KLIB_HANDLE_TYPE.USBSHAREDK, CleanupCallback);
		}

		public bool SetSharedContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, KLIB_HANDLE_TYPE.USBSHAREDK, UserContext);
		}

		#endregion

		#region Public Members

		public KUSB_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	public struct KOVL_POOL_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.OVLPOOLK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region Public Members

		public KOVL_POOL_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	public struct KOVL_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.OVLK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region Public Members

		public KOVL_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	public struct KSTM_HANDLE : IKLIB_HANDLE
	{
		#region IKLIB_HANDLE Members

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.STMK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		#endregion

		#region Public Members

		public KSTM_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		#endregion

		#region Private Members

		private readonly IntPtr mHandlePtr;

		#endregion
	}

	#endregion

	#region Internal Function Imports

	internal static class AllKFunctions
	{
		#region Delegates

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate void HotK_FreeAllDelegate();

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool HotK_FreeDelegate([In] KHOT_HANDLE Handle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool HotK_InitDelegate([Out] out KHOT_HANDLE Handle, [In, Out] ref KHOT_PARAMS InitParams);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_EnumPacketsDelegate([In] KISO_CONTEXT IsoContext, KISO_ENUM_PACKETS_CB EnumPackets, int StartPacketIndex, IntPtr UserState);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_FreeDelegate([In] KISO_CONTEXT IsoContext);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_GetPacketDelegate([In] KISO_CONTEXT IsoContext, int PacketIndex, [Out] out KISO_PACKET IsoPacket);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_InitDelegate([Out] out KISO_CONTEXT IsoContext, int NumberOfPackets, int StartFrame);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_ReUseDelegate([In] KISO_CONTEXT IsoContext);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_SetPacketDelegate([In] KISO_CONTEXT IsoContext, int PacketIndex, [In] ref KISO_PACKET IsoPacket);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool IsoK_SetPacketsDelegate([In] KISO_CONTEXT IsoContext, int PacketSize);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate void LibK_Context_FreeDelegate();

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_Context_InitDelegate(IntPtr Heap, IntPtr Reserved);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_CopyDriverAPIDelegate([Out] out KUSB_DRIVER_API DriverAPI, [In] KUSB_HANDLE UsbHandle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate IntPtr LibK_GetContextDelegate([In] IntPtr Handle, KLIB_HANDLE_TYPE HandleType);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate IntPtr LibK_GetDefaultContextDelegate(KLIB_HANDLE_TYPE HandleType);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_GetProcAddressDelegate(IntPtr ProcAddress, int DriverID, int FunctionID);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate void LibK_GetVersionDelegate([Out] out KLIB_VERSION Version);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_LoadDriverAPIDelegate([Out] out KUSB_DRIVER_API DriverAPI, int DriverID);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_SetCleanupCallbackDelegate([In] IntPtr Handle, KLIB_HANDLE_TYPE HandleType, KLIB_HANDLE_CLEANUP_CB CleanupCB);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_SetContextDelegate([In] IntPtr Handle, KLIB_HANDLE_TYPE HandleType, IntPtr ContextValue);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LibK_SetDefaultContextDelegate(KLIB_HANDLE_TYPE HandleType, IntPtr ContextValue);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_CountDelegate([In] KLST_HANDLE DeviceList, ref int Count);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_CurrentDelegate([In] KLST_HANDLE DeviceList, [Out] out KLST_DEVINFO_HANDLE DeviceInfo);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_EnumerateDelegate([In] KLST_HANDLE DeviceList, KLST_ENUM_DEVINFO_CB EnumDevListCB, IntPtr Context);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_FindByVidPidDelegate([In] KLST_HANDLE DeviceList, int Vid, int Pid, [Out] out KLST_DEVINFO_HANDLE DeviceInfo);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_FreeDelegate([In] KLST_HANDLE DeviceList);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_InitDelegate([Out] out KLST_HANDLE DeviceList, KLST_FLAG Flags);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_InitExDelegate([Out] out KLST_HANDLE DeviceList, KLST_FLAG Flags, [In] ref KLST_PATTERN_MATCH PatternMatch);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool LstK_MoveNextDelegate([In] KLST_HANDLE DeviceList, [Out] out KLST_DEVINFO_HANDLE DeviceInfo);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate void LstK_MoveResetDelegate([In] KLST_HANDLE DeviceList);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_AcquireDelegate([Out] out KOVL_HANDLE OverlappedK, [In] KOVL_POOL_HANDLE PoolHandle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_FreeDelegate([In] KOVL_POOL_HANDLE PoolHandle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate IntPtr OvlK_GetEventHandleDelegate([In] KOVL_HANDLE OverlappedK);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_InitDelegate([Out] out KOVL_POOL_HANDLE PoolHandle, [In] KUSB_HANDLE UsbHandle, int MaxOverlappedCount, KOVL_POOL_FLAG Flags);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_IsCompleteDelegate([In] KOVL_HANDLE OverlappedK);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_ReUseDelegate([In] KOVL_HANDLE OverlappedK);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_ReleaseDelegate([In] KOVL_HANDLE OverlappedK);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_WaitAndReleaseDelegate([In] KOVL_HANDLE OverlappedK, int TimeoutMS, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_WaitDelegate([In] KOVL_HANDLE OverlappedK, int TimeoutMS, KOVL_WAIT_FLAG WaitFlags, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_WaitOldestDelegate([In] KOVL_POOL_HANDLE PoolHandle, [Out] out KOVL_HANDLE OverlappedK, int TimeoutMS, KOVL_WAIT_FLAG WaitFlags, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool OvlK_WaitOrCancelDelegate([In] KOVL_HANDLE OverlappedK, int TimeoutMS, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_FreeDelegate([In] KSTM_HANDLE StreamHandle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_InitDelegate(
			[Out] out KSTM_HANDLE StreamHandle, [In] KUSB_HANDLE UsbHandle, byte PipeID, int MaxTransferSize, int MaxPendingTransfers, int MaxPendingIO, [In] ref KSTM_CALLBACK Callbacks, KSTM_FLAG Flags);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_ReadDelegate([In] KSTM_HANDLE StreamHandle, IntPtr Buffer, int Offset, int Length, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_StartDelegate([In] KSTM_HANDLE StreamHandle);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_StopDelegate([In] KSTM_HANDLE StreamHandle, int TimeoutCancelMS);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool StmK_WriteDelegate([In] KSTM_HANDLE StreamHandle, IntPtr Buffer, int Offset, int Length, out int TransferredLength);

		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
		public delegate bool UsbK_FreeDelegate([In] KUSB_HANDLE InterfaceHandle);

		#endregion

		static AllKFunctions()
		{
			if (String.IsNullOrEmpty(AllKOptions.LIBUSBK_FULLPATH_TO_ALTERNATE_DLL))
				mModuleLibusbK = LoadLibraryEx(AllKConstants.LIBUSBK_DLL, IntPtr.Zero, LoadLibraryFlags.NONE);
			else
				mModuleLibusbK = LoadLibraryEx(AllKOptions.LIBUSBK_FULLPATH_TO_ALTERNATE_DLL, IntPtr.Zero, LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH);

			if (mModuleLibusbK == IntPtr.Zero)
				throw new DllNotFoundException("libusbK.dll not found.  Please install drivers/applications and retry.");

			LoadDynamicFunctions();
		}

		#region Nested Enumerations

		[Flags]
		private enum LoadLibraryFlags
		{
			NONE = 0,
			DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
			LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
			LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
		}

		#endregion

		#region Public Members

		public static HotK_FreeDelegate HotK_Free;
		public static HotK_FreeAllDelegate HotK_FreeAll;
		public static HotK_InitDelegate HotK_Init;
		public static IsoK_EnumPacketsDelegate IsoK_EnumPackets;
		public static IsoK_FreeDelegate IsoK_Free;
		public static IsoK_GetPacketDelegate IsoK_GetPacket;
		public static IsoK_InitDelegate IsoK_Init;
		public static IsoK_ReUseDelegate IsoK_ReUse;
		public static IsoK_SetPacketDelegate IsoK_SetPacket;
		public static IsoK_SetPacketsDelegate IsoK_SetPackets;
		public static LibK_Context_FreeDelegate LibK_Context_Free;
		public static LibK_Context_InitDelegate LibK_Context_Init;

		public static LibK_CopyDriverAPIDelegate LibK_CopyDriverAPI;
		public static LibK_GetContextDelegate LibK_GetContext;
		public static LibK_GetDefaultContextDelegate LibK_GetDefaultContext;
		public static LibK_GetProcAddressDelegate LibK_GetProcAddress;
		public static LibK_GetVersionDelegate LibK_GetVersion;
		public static LibK_LoadDriverAPIDelegate LibK_LoadDriverAPI;
		public static LibK_SetCleanupCallbackDelegate LibK_SetCleanupCallback;
		public static LibK_SetContextDelegate LibK_SetContext;
		public static LibK_SetDefaultContextDelegate LibK_SetDefaultContext;
		public static LstK_CountDelegate LstK_Count;
		public static LstK_CurrentDelegate LstK_Current;
		public static LstK_EnumerateDelegate LstK_Enumerate;
		public static LstK_FindByVidPidDelegate LstK_FindByVidPid;
		public static LstK_FreeDelegate LstK_Free;
		public static LstK_InitDelegate LstK_Init;
		public static LstK_InitExDelegate LstK_InitEx;
		public static LstK_MoveNextDelegate LstK_MoveNext;
		public static LstK_MoveResetDelegate LstK_MoveReset;
		public static OvlK_AcquireDelegate OvlK_Acquire;
		public static OvlK_FreeDelegate OvlK_Free;
		public static OvlK_GetEventHandleDelegate OvlK_GetEventHandle;
		public static OvlK_InitDelegate OvlK_Init;
		public static OvlK_IsCompleteDelegate OvlK_IsComplete;
		public static OvlK_ReUseDelegate OvlK_ReUse;
		public static OvlK_ReleaseDelegate OvlK_Release;
		public static OvlK_WaitDelegate OvlK_Wait;
		public static OvlK_WaitAndReleaseDelegate OvlK_WaitAndRelease;
		public static OvlK_WaitOldestDelegate OvlK_WaitOldest;
		public static OvlK_WaitOrCancelDelegate OvlK_WaitOrCancel;
		public static StmK_FreeDelegate StmK_Free;
		public static StmK_InitDelegate StmK_Init;
		public static StmK_ReadDelegate StmK_Read;
		public static StmK_StartDelegate StmK_Start;
		public static StmK_StopDelegate StmK_Stop;
		public static StmK_WriteDelegate StmK_Write;
		public static UsbK_FreeDelegate UsbK_Free;

		#endregion

		#region Private Members

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		private static void LoadDynamicFunctions()
		{
			LibK_GetVersion = (LibK_GetVersionDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_GetVersion"), typeof(LibK_GetVersionDelegate));
			LibK_GetContext = (LibK_GetContextDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_GetContext"), typeof(LibK_GetContextDelegate));
			LibK_SetContext = (LibK_SetContextDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_SetContext"), typeof(LibK_SetContextDelegate));
			LibK_SetCleanupCallback = (LibK_SetCleanupCallbackDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_SetCleanupCallback"), typeof(LibK_SetCleanupCallbackDelegate));
			LibK_LoadDriverAPI = (LibK_LoadDriverAPIDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_LoadDriverAPI"), typeof(LibK_LoadDriverAPIDelegate));
			LibK_CopyDriverAPI = (LibK_CopyDriverAPIDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_CopyDriverAPI"), typeof(LibK_CopyDriverAPIDelegate));
			LibK_GetProcAddress = (LibK_GetProcAddressDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_GetProcAddress"), typeof(LibK_GetProcAddressDelegate));
			LibK_SetDefaultContext = (LibK_SetDefaultContextDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_SetDefaultContext"), typeof(LibK_SetDefaultContextDelegate));
			LibK_GetDefaultContext = (LibK_GetDefaultContextDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_GetDefaultContext"), typeof(LibK_GetDefaultContextDelegate));
			LibK_Context_Init = (LibK_Context_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_Context_Init"), typeof(LibK_Context_InitDelegate));
			LibK_Context_Free = (LibK_Context_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LibK_Context_Free"), typeof(LibK_Context_FreeDelegate));
			UsbK_Free = (UsbK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "UsbK_Free"), typeof(UsbK_FreeDelegate));
			LstK_Init = (LstK_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_Init"), typeof(LstK_InitDelegate));
			LstK_InitEx = (LstK_InitExDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_InitEx"), typeof(LstK_InitExDelegate));
			LstK_Free = (LstK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_Free"), typeof(LstK_FreeDelegate));
			LstK_Enumerate = (LstK_EnumerateDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_Enumerate"), typeof(LstK_EnumerateDelegate));
			LstK_Current = (LstK_CurrentDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_Current"), typeof(LstK_CurrentDelegate));
			LstK_MoveNext = (LstK_MoveNextDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_MoveNext"), typeof(LstK_MoveNextDelegate));
			LstK_MoveReset = (LstK_MoveResetDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_MoveReset"), typeof(LstK_MoveResetDelegate));
			LstK_FindByVidPid = (LstK_FindByVidPidDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_FindByVidPid"), typeof(LstK_FindByVidPidDelegate));
			LstK_Count = (LstK_CountDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "LstK_Count"), typeof(LstK_CountDelegate));
			HotK_Init = (HotK_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "HotK_Init"), typeof(HotK_InitDelegate));
			HotK_Free = (HotK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "HotK_Free"), typeof(HotK_FreeDelegate));
			HotK_FreeAll = (HotK_FreeAllDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "HotK_FreeAll"), typeof(HotK_FreeAllDelegate));
			OvlK_Acquire = (OvlK_AcquireDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_Acquire"), typeof(OvlK_AcquireDelegate));
			OvlK_Release = (OvlK_ReleaseDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_Release"), typeof(OvlK_ReleaseDelegate));
			OvlK_Init = (OvlK_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_Init"), typeof(OvlK_InitDelegate));
			OvlK_Free = (OvlK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_Free"), typeof(OvlK_FreeDelegate));
			OvlK_GetEventHandle = (OvlK_GetEventHandleDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_GetEventHandle"), typeof(OvlK_GetEventHandleDelegate));
			OvlK_Wait = (OvlK_WaitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_Wait"), typeof(OvlK_WaitDelegate));
			OvlK_WaitOldest = (OvlK_WaitOldestDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_WaitOldest"), typeof(OvlK_WaitOldestDelegate));
			OvlK_WaitOrCancel = (OvlK_WaitOrCancelDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_WaitOrCancel"), typeof(OvlK_WaitOrCancelDelegate));
			OvlK_WaitAndRelease = (OvlK_WaitAndReleaseDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_WaitAndRelease"), typeof(OvlK_WaitAndReleaseDelegate));
			OvlK_IsComplete = (OvlK_IsCompleteDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_IsComplete"), typeof(OvlK_IsCompleteDelegate));
			OvlK_ReUse = (OvlK_ReUseDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "OvlK_ReUse"), typeof(OvlK_ReUseDelegate));
			StmK_Init = (StmK_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Init"), typeof(StmK_InitDelegate));
			StmK_Free = (StmK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Free"), typeof(StmK_FreeDelegate));
			StmK_Start = (StmK_StartDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Start"), typeof(StmK_StartDelegate));
			StmK_Stop = (StmK_StopDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Stop"), typeof(StmK_StopDelegate));
			StmK_Read = (StmK_ReadDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Read"), typeof(StmK_ReadDelegate));
			StmK_Write = (StmK_WriteDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "StmK_Write"), typeof(StmK_WriteDelegate));
			IsoK_Init = (IsoK_InitDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_Init"), typeof(IsoK_InitDelegate));
			IsoK_Free = (IsoK_FreeDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_Free"), typeof(IsoK_FreeDelegate));
			IsoK_SetPackets = (IsoK_SetPacketsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_SetPackets"), typeof(IsoK_SetPacketsDelegate));
			IsoK_SetPacket = (IsoK_SetPacketDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_SetPacket"), typeof(IsoK_SetPacketDelegate));
			IsoK_GetPacket = (IsoK_GetPacketDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_GetPacket"), typeof(IsoK_GetPacketDelegate));
			IsoK_EnumPackets = (IsoK_EnumPacketsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_EnumPackets"), typeof(IsoK_EnumPacketsDelegate));
			IsoK_ReUse = (IsoK_ReUseDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(mModuleLibusbK, "IsoK_ReUse"), typeof(IsoK_ReUseDelegate));
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

		private static readonly IntPtr mModuleLibusbK = IntPtr.Zero;

		#endregion
	}

	#endregion

	#region Enumerations

	/// <Summary>Values used in the \c bmAttributes field of a \ref USB_ENDPOINT_DESCRIPTOR</Summary>
	public enum USBD_PIPE_TYPE
	{
		/// <Summary>Indicates a control endpoint</Summary>
		UsbdPipeTypeControl,

		/// <Summary>Indicates an isochronous endpoint</Summary>
		UsbdPipeTypeIsochronous,

		/// <Summary>Indicates a bulk endpoint</Summary>
		UsbdPipeTypeBulk,

		/// <Summary>Indicates an interrupt endpoint</Summary>
		UsbdPipeTypeInterrupt,
	}

	/// <Summary>Additional ISO transfer flags.</Summary>
	[Flags]
	public enum KISO_FLAG
	{
		NONE = 0,

		/// <Summary>Do not start the transfer immediately, instead use \ref KISO_CONTEXT::StartFrame.</Summary>
		SET_START_FRAME = 0x00000001,
	}

	/// <Summary>Handle type enumeration.</Summary>
	public enum KLIB_HANDLE_TYPE
	{
		/// <Summary>Hot plug handle. \ref KHOT_HANDLE</Summary>
		HOTK,

		/// <Summary>USB handle. \ref KUSB_HANDLE</Summary>
		USBK,

		/// <Summary>Shared USB handle. \ref KUSB_HANDLE</Summary>
		USBSHAREDK,

		/// <Summary>Device list handle. \ref KLST_HANDLE</Summary>
		LSTK,

		/// <Summary>Device info handle. \ref KLST_DEVINFO_HANDLE</Summary>
		LSTINFOK,

		/// <Summary>Overlapped handle. \ref KOVL_HANDLE</Summary>
		OVLK,

		/// <Summary>Overlapped pool handle. \ref KOVL_POOL_HANDLE</Summary>
		OVLPOOLK,

		/// <Summary>Pipe stream handle. \ref KSTM_HANDLE</Summary>
		STMK,

		/// <Summary>Max handle type count.</Summary>
		COUNT
	}

	/// <Summary>Device list sync flags.</Summary>
	[Flags]
	public enum KLST_SYNC_FLAG
	{
		/// <Summary>Cleared/invalid state.</Summary>
		NONE = 0,

		/// <Summary>Unchanged state,</Summary>
		UNCHANGED = 0x0001,

		/// <Summary>Added (Arrival) state,</Summary>
		ADDED = 0x0002,

		/// <Summary>Removed (Unplugged) state,</Summary>
		REMOVED = 0x0004,

		/// <Summary>Connect changed state.</Summary>
		CONNECT_CHANGE = 0x0008,

		/// <Summary>All states.</Summary>
		MASK = 0x000F,
	}

	/// <Summary>Device list initialization flags.</Summary>
	[Flags]
	public enum KLST_FLAG
	{
		/// <Summary>No flags (or 0)</Summary>
		NONE = 0,

		/// <Summary>Enable listings for the raw device interface GUID \b only. {A5DCBF10-6530-11D2-901F-00C04FB951ED}</Summary>
		INCLUDE_RAWGUID = 0x0001,

		/// <Summary>List all libusbK devices including those not currently connected.</Summary>
		INCLUDE_DISCONNECT = 0x0002,
	}

	/// <Summary>bmRequest.Dir</Summary>
	public enum BMREQUEST_DIR
	{
		HOST_TO_DEVICE = 0,
		DEVICE_TO_HOST = 1,
	}

	/// <Summary>bmRequest.Type</Summary>
	public enum BMREQUEST_TYPE
	{
		/// <Summary>Standard request. See \ref USB_REQUEST_ENUM</Summary>
		STANDARD = 0,

		/// <Summary>Class-specific request.</Summary>
		CLASS = 1,

		/// <Summary>Vendor-specific request</Summary>
		VENDOR = 2,
	}

	/// <Summary>bmRequest.Recipient</Summary>
	public enum BMREQUEST_RECIPIENT
	{
		/// <Summary>Request is for a device.</Summary>
		DEVICE = 0,

		/// <Summary>Request is for an interface of a device.</Summary>
		INTERFACE = 1,

		/// <Summary>Request is for an endpoint of a device.</Summary>
		ENDPOINT = 2,

		/// <Summary>Request is for a vendor-specific purpose.</Summary>
		OTHER = 3,
	}

	/// <Summary>Values for the bits returned by the \ref USB_REQUEST_GET_STATUS request.</Summary>
	public enum USB_GETSTATUS
	{
		/// <Summary>Device is self powered</Summary>
		SELF_POWERED = 0x01,

		/// <Summary>Device can wake the system from a low power/sleeping state.</Summary>
		REMOTE_WAKEUP_ENABLED = 0x02
	}

	/// <Summary>Standard USB descriptor types. For more information, see section 9-5 of the USB 3.0 specifications.</Summary>
	public enum USB_DESCRIPTOR_TYPE
	{
		/// <Summary>Device descriptor type.</Summary>
		DEVICE = 0x01,

		/// <Summary>Configuration descriptor type.</Summary>
		CONFIGURATION = 0x02,

		/// <Summary>String descriptor type.</Summary>
		STRING = 0x03,

		/// <Summary>Interface descriptor type.</Summary>
		INTERFACE = 0x04,

		/// <Summary>Endpoint descriptor type.</Summary>
		ENDPOINT = 0x05,

		/// <Summary>Device qualifier descriptor type.</Summary>
		DEVICE_QUALIFIER = 0x06,

		/// <Summary>Config power descriptor type.</Summary>
		CONFIG_POWER = 0x07,

		/// <Summary>Interface power descriptor type.</Summary>
		INTERFACE_POWER = 0x08,

		/// <Summary>Interface association descriptor type.</Summary>
		INTERFACE_ASSOCIATION = 0x0B,
	}

	/// <Summary>Usb handle specific properties that can be retrieved with \ref UsbK_GetProperty.</Summary>
	public enum KUSB_PROPERTY
	{
		/// <Summary>Get the internal device file handle used for operations such as GetOverlappedResult or DeviceIoControl.</Summary>
		DEVICE_FILE_HANDLE,

		COUNT
	}

	/// <Summary>Supported driver id enumeration.</Summary>
	public enum KUSB_DRVID
	{
		/// <Summary>libusbK.sys driver ID</Summary>
		LIBUSBK,

		/// <Summary>libusb0.sys driver ID</Summary>
		LIBUSB0,

		/// <Summary>WinUSB.sys driver ID</Summary>
		WINUSB,

		/// <Summary>libusb0.sys filter driver ID</Summary>
		LIBUSB0_FILTER,

		/// <Summary>Supported driver count</Summary>
		COUNT
	}

	/// <Summary>Supported function id enumeration.</Summary>
	public enum KUSB_FNID
	{
		/// <Summary>\ref UsbK_Init dynamic driver function id.</Summary>
		Init,

		/// <Summary>\ref UsbK_Free dynamic driver function id.</Summary>
		Free,

		/// <Summary>\ref UsbK_ClaimInterface dynamic driver function id.</Summary>
		ClaimInterface,

		/// <Summary>\ref UsbK_ReleaseInterface dynamic driver function id.</Summary>
		ReleaseInterface,

		/// <Summary>\ref UsbK_SetAltInterface dynamic driver function id.</Summary>
		SetAltInterface,

		/// <Summary>\ref UsbK_GetAltInterface dynamic driver function id.</Summary>
		GetAltInterface,

		/// <Summary>\ref UsbK_GetDescriptor dynamic driver function id.</Summary>
		GetDescriptor,

		/// <Summary>\ref UsbK_ControlTransfer dynamic driver function id.</Summary>
		ControlTransfer,

		/// <Summary>\ref UsbK_SetPowerPolicy dynamic driver function id.</Summary>
		SetPowerPolicy,

		/// <Summary>\ref UsbK_GetPowerPolicy dynamic driver function id.</Summary>
		GetPowerPolicy,

		/// <Summary>\ref UsbK_SetConfiguration dynamic driver function id.</Summary>
		SetConfiguration,

		/// <Summary>\ref UsbK_GetConfiguration dynamic driver function id.</Summary>
		GetConfiguration,

		/// <Summary>\ref UsbK_ResetDevice dynamic driver function id.</Summary>
		ResetDevice,

		/// <Summary>\ref UsbK_Initialize dynamic driver function id.</Summary>
		Initialize,

		/// <Summary>\ref UsbK_SelectInterface dynamic driver function id.</Summary>
		SelectInterface,

		/// <Summary>\ref UsbK_GetAssociatedInterface dynamic driver function id.</Summary>
		GetAssociatedInterface,

		/// <Summary>\ref UsbK_Clone dynamic driver function id.</Summary>
		Clone,

		/// <Summary>\ref UsbK_QueryInterfaceSettings dynamic driver function id.</Summary>
		QueryInterfaceSettings,

		/// <Summary>\ref UsbK_QueryDeviceInformation dynamic driver function id.</Summary>
		QueryDeviceInformation,

		/// <Summary>\ref UsbK_SetCurrentAlternateSetting dynamic driver function id.</Summary>
		SetCurrentAlternateSetting,

		/// <Summary>\ref UsbK_GetCurrentAlternateSetting dynamic driver function id.</Summary>
		GetCurrentAlternateSetting,

		/// <Summary>\ref UsbK_QueryPipe dynamic driver function id.</Summary>
		QueryPipe,

		/// <Summary>\ref UsbK_SetPipePolicy dynamic driver function id.</Summary>
		SetPipePolicy,

		/// <Summary>\ref UsbK_GetPipePolicy dynamic driver function id.</Summary>
		GetPipePolicy,

		/// <Summary>\ref UsbK_ReadPipe dynamic driver function id.</Summary>
		ReadPipe,

		/// <Summary>\ref UsbK_WritePipe dynamic driver function id.</Summary>
		WritePipe,

		/// <Summary>\ref UsbK_ResetPipe dynamic driver function id.</Summary>
		ResetPipe,

		/// <Summary>\ref UsbK_AbortPipe dynamic driver function id.</Summary>
		AbortPipe,

		/// <Summary>\ref UsbK_FlushPipe dynamic driver function id.</Summary>
		FlushPipe,

		/// <Summary>\ref UsbK_IsoReadPipe dynamic driver function id.</Summary>
		IsoReadPipe,

		/// <Summary>\ref UsbK_IsoWritePipe dynamic driver function id.</Summary>
		IsoWritePipe,

		/// <Summary>\ref UsbK_GetCurrentFrameNumber dynamic driver function id.</Summary>
		GetCurrentFrameNumber,

		/// <Summary>\ref UsbK_GetOverlappedResult dynamic driver function id.</Summary>
		GetOverlappedResult,

		/// <Summary>\ref UsbK_GetProperty dynamic driver function id.</Summary>
		GetProperty,


		/// <Summary>Supported function count</Summary>
		COUNT,
	}

	/// <Summary>Hot plug config flags.</Summary>
	[Flags]
	public enum KHOT_FLAG
	{
		/// <Summary>No flags (or 0)</Summary>
		NONE,

		/// <Summary>Notify all devices which match upon a succuessful call to \ref HotK_Init.</Summary>
		PLUG_ALL_ON_INIT = 0x0001,

		/// <Summary>Allow other \ref KHOT_HANDLE instances to consume this match.</Summary>
		PASS_DUPE_INSTANCE = 0x0002,

		/// <Summary>If a \c UserHwnd is specified, use \c PostMessage instead of \c SendMessage.</Summary>
		POST_USER_MESSAGE = 0x0004,
	}

	/// <Summary>\c WaitFlags used by \ref OvlK_Wait.</Summary>
	[Flags]
	public enum KOVL_WAIT_FLAG
	{
		/// <Summary>Do not perform any additional actions upon exiting \ref OvlK_Wait.</Summary>
		NONE = 0,

		/// <Summary>If the i/o operation completes successfully, release the OverlappedK back to it's pool.</Summary>
		RELEASE_ON_SUCCESS = 0x0001,

		/// <Summary>If the i/o operation fails, release the OverlappedK back to it's pool.</Summary>
		RELEASE_ON_FAIL = 0x0002,

		/// <Summary>If the i/o operation fails or completes successfully, release the OverlappedK back to its pool. Perform no actions if it times-out.</Summary>
		RELEASE_ON_SUCCESS_FAIL = 0x0003,

		/// <Summary>If the i/o operation times-out cancel it, but do not release the OverlappedK back to its pool.</Summary>
		CANCEL_ON_TIMEOUT = 0x0004,

		/// <Summary>If the i/o operation times-out, cancel it and release the OverlappedK back to its pool.</Summary>
		RELEASE_ON_TIMEOUT = 0x000C,

		/// <Summary>Always release the OverlappedK back to its pool.  If the operation timed-out, cancel it before releasing back to its pool.</Summary>
		RELEASE_ALWAYS = 0x000F,

		/// <Summary>Uses alterable wait functions.  See http://msdn.microsoft.com/en-us/library/windows/desktop/ms687036%28v=vs.85%29.aspx</Summary>
		ALERTABLE = 0x0010,
	}

	/// <Summary>\c Overlapped pool config flags.</Summary>
	[Flags]
	public enum KOVL_POOL_FLAG
	{
		NONE = 0,
	}

	/// <Summary>Stream config flags.</Summary>
	[Flags]
	public enum KSTM_FLAG : uint
	{
		/// <Summary>None</Summary>
		NONE = 0,
		NO_PARTIAL_XFERS = 0x00100000,
		USE_TIMEOUT = 0x80000000,
		TIMEOUT_MASK = 0x0001FFFF
	}

	/// <Summary>Stream config flags.</Summary>
	public enum KSTM_COMPLETE_RESULT
	{
		/// <Summary>Valid</Summary>
		VALID = 0,

		/// <Summary>Invalid</Summary>
		INVALID,
	}

	#endregion

	#region Structs

	/// <Summary>The \c WINUSB_PIPE_INFORMATION structure contains pipe information that the \ref UsbK_QueryPipe routine retrieves.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct WINUSB_PIPE_INFORMATION
	{
		/// <Summary>A \c USBD_PIPE_TYPE enumeration value that specifies the pipe type</Summary>
		public USBD_PIPE_TYPE PipeType;

		/// <Summary>The pipe identifier (ID)</Summary>
		public byte PipeId;

		/// <Summary>The maximum size, in bytes, of the packets that are transmitted on the pipe</Summary>
		public ushort MaximumPacketSize;

		/// <Summary>The pipe interval</Summary>
		public byte Interval;


		public override string ToString()
		{
			return string.Format("PipeType: {0}\nPipeId: {1}\nMaximumPacketSize: {2}\nInterval: {3}\n", PipeType, PipeId.ToString("X2") + "h", MaximumPacketSize, Interval.ToString("X2") + "h");
		}
	};

	/// <Summary>The \c WINUSB_SETUP_PACKET structure describes a USB setup packet.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct WINUSB_SETUP_PACKET
	{
		/// <Summary>The request type. The values that are assigned to this member are defined in Table 9.2 of section 9.3 of the Universal Serial Bus (USB) specification (www.usb.org).</Summary>
		public byte RequestType;

		/// <Summary>The device request. The values that are assigned to this member are defined in Table 9.3 of section 9.4 of the Universal Serial Bus (USB) specification.</Summary>
		public byte Request;

		/// <Summary>The meaning of this member varies according to the request. For an explanation of this member, see the Universal Serial Bus (USB) specification.</Summary>
		public ushort Value;

		/// <Summary>The meaning of this member varies according to the request. For an explanation of this member, see the Universal Serial Bus (USB) specification.</Summary>
		public ushort Index;

		/// <Summary>The number of bytes to transfer. (not including the \c WINUSB_SETUP_PACKET itself)</Summary>
		public ushort Length;


		public override string ToString()
		{
			return string.Format("RequestType: {0}\nRequest: {1}\nValue: {2}\nIndex: {3}\nLength: {4}\n",
								 RequestType.ToString("X2") + "h",
								 Request.ToString("X2") + "h",
								 Value.ToString("X4") + "h",
								 Index.ToString("X4") + "h",
								 Length);
		}
	};

	/// <Summary>Structure describing an isochronous transfer packet.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct KISO_PACKET
	{
		/// <Summary>Specifies the offset, in bytes, of the buffer for this packet from the beginning of the entire isochronous transfer data buffer.</Summary>
		public int Offset;

		/// <Summary>Set by the host controller to indicate the actual number of bytes received by the device for isochronous IN transfers. Length not used for isochronous OUT transfers.</Summary>
		public ushort Length;

		/// <Summary>Contains the 16 least significant USBD status bits, on return from the host controller driver, of this transfer packet.</Summary>
		public ushort Status;


		public override string ToString()
		{
			return string.Format("Offset: {0}\nLength: {1}\nStatus: {2}\n", Offset, Length, Status.ToString("X4") + "h");
		}
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct KISO_CONTEXT
	{
		private readonly IntPtr mHandlePtr;

		public KISO_CONTEXT(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		/// <summary>KISO_CONTEXT_MAP is used for calculating field offsets only</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct KISO_CONTEXT_MAP
		{
			/// <Summary>Additional ISO transfer flags. See \ref KISO_FLAG.</Summary>
			private readonly KISO_FLAG Flags;

			/// <Summary>Specifies the frame number that the transfer should begin on (0 for ASAP).</Summary>
			private readonly int StartFrame;

			/// <Summary>Contains the number of packets that completed with an error condition on return from the host controller driver.</Summary>
			private readonly short ErrorCount;

			/// <Summary>Specifies the number of packets that are described by the variable-length array member \c IsoPacket.</Summary>
			private readonly short NumberOfPackets;

			/// <Summary>Contains the URB Hdr.Status value on return from the host controller driver.</Summary>
			private readonly int UrbHdrStatus;
		};

		private static readonly int ofsFlags = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "Flags").ToInt32();
		private static readonly int ofsStartFrame = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "StartFrame").ToInt32();
		private static readonly int ofsErrorCount = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "ErrorCount").ToInt32();
		private static readonly int ofsNumberOfPackets = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "NumberOfPackets").ToInt32();
		private static readonly int ofsUrbHdrStatus = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "UrbHdrStatus").ToInt32();


		/// <Summary>Additional ISO transfer flags. See \ref KISO_FLAG.</Summary>
		public KISO_FLAG Flags
		{
			get
			{
				return (KISO_FLAG)Marshal.ReadInt32(mHandlePtr, ofsFlags);
			}
		}


		/// <Summary>Specifies the frame number that the transfer should begin on (0 for ASAP).</Summary>
		public int StartFrame
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsStartFrame);
			}
		}


		/// <Summary>Contains the number of packets that completed with an error condition on return from the host controller driver.</Summary>
		public short ErrorCount
		{
			get
			{
				return Marshal.ReadInt16(mHandlePtr, ofsErrorCount);
			}
		}


		/// <Summary>Specifies the number of packets that are described by the variable-length array member \c IsoPacket.</Summary>
		public short NumberOfPackets
		{
			get
			{
				return Marshal.ReadInt16(mHandlePtr, ofsNumberOfPackets);
			}
		}


		/// <Summary>Contains the URB Hdr.Status value on return from the host controller driver.</Summary>
		public int UrbHdrStatus
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsUrbHdrStatus);
			}
		}


		public override string ToString()
		{
			return string.Format("Flags: {0}\nStartFrame: {1}\nErrorCount: {2}\nNumberOfPackets: {3}\nUrbHdrStatus: {4}\n",
								 Flags.ToString(),
								 StartFrame,
								 ErrorCount,
								 NumberOfPackets,
								 UrbHdrStatus.ToString("X8") + "h");
		}
	}

	/// <Summary>libusbK verson information structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct KLIB_VERSION
	{
		/// <Summary>Major version number.</Summary>
		public int Major;

		/// <Summary>Minor version number.</Summary>
		public int Minor;

		/// <Summary>Micro version number.</Summary>
		public int Micro;

		/// <Summary>Nano version number.</Summary>
		public int Nano;

		public override string ToString()
		{
			return string.Format("Major: {0}\nMinor: {1}\nMicro: {2}\nNano: {3}\n", Major, Minor, Micro, Nano);
		}
	};

	/// <Summary>Common usb device information structure</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct KLST_DEV_COMMON_INFO
	{
		/// <Summary>VendorID parsed from \ref KLST_DEVINFO::DeviceID</Summary>
		public int Vid;

		/// <Summary>ProductID parsed from \ref KLST_DEVINFO::DeviceID</Summary>
		public int Pid;

		/// <Summary>Composite interface number parsed from \ref KLST_DEVINFO::DeviceID.  Set to \b -1 for devices that do not have the composite parent driver.</Summary>
		public int MI;

		// An ID that uniquely identifies a USB device.
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] public string InstanceID;


		public override string ToString()
		{
			return string.Format("Vid: {0}\nPid: {1}\nMI: {2}\nInstanceID: {3}\n", Vid.ToString("X4") + "h", Pid.ToString("X4") + "h", MI.ToString("X2") + "h", InstanceID);
		}
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct KLST_DEVINFO_HANDLE : IKLIB_HANDLE
	{
		private readonly IntPtr mHandlePtr;

		public KLST_DEVINFO_HANDLE(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		public KLIB_HANDLE_TYPE HandleType
		{
			get
			{
				return KLIB_HANDLE_TYPE.LSTINFOK;
			}
		}

		public IntPtr GetContext()
		{
			return AllKFunctions.LibK_GetContext(mHandlePtr, HandleType);
		}

		public bool SetContext(IntPtr UserContext)
		{
			return AllKFunctions.LibK_SetContext(mHandlePtr, HandleType, UserContext);
		}

		public bool SetCleanupCallback(KLIB_HANDLE_CLEANUP_CB CleanupCallback)
		{
			return AllKFunctions.LibK_SetCleanupCallback(mHandlePtr, HandleType, CleanupCallback);
		}

		/// <summary>KLST_DEVINFO_MAP is used for calculating field offsets only</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct KLST_DEVINFO_MAP
		{
			/// <Summary>Common usb device information</Summary>
			private readonly KLST_DEV_COMMON_INFO Common;

			/// <Summary>Driver id this device element is using</Summary>
			private readonly int DriverID;

			/// <Summary>Device interface GUID</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string DeviceInterfaceGUID;

			/// <Summary>Device instance ID.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string DeviceID;

			/// <Summary>Class GUID.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string ClassGUID;

			/// <Summary>Manufacturer name as specified in the INF file.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string Mfg;

			/// <Summary>Device description as specified in the INF file.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string DeviceDesc;

			/// <Summary>Driver service name.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string Service;

			/// <Summary>Unique identifier.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string SymbolicLink;

			/// <Summary>physical device filename used with the Windows \c CreateFile()</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string DevicePath;

			/// <Summary>libusb-win32 filter index id.</Summary>
			private readonly int LUsb0FilterIndex;

			/// <Summary>Indicates the devices connection state.</Summary>
			private readonly bool Connected;

			/// <Summary>Synchronization flags. (internal use only)</Summary>
			private readonly KLST_SYNC_FLAG SyncFlags;

			private readonly int BusNumber;

			private readonly int DeviceAddress;

			/// <Summary>If the the device is serialized, represents the string value of \ref USB_DEVICE_DESCRIPTOR::iSerialNumber. For Devices without a \b iSerialNumber, represents the unique \b InstanceID assigned by \b Windows.</Summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] private readonly string SerialNumber;
		};

		private static readonly int ofsCommon = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "Common").ToInt32();
		private static readonly int ofsDriverID = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DriverID").ToInt32();
		private static readonly int ofsDeviceInterfaceGUID = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DeviceInterfaceGUID").ToInt32();
		private static readonly int ofsDeviceID = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DeviceID").ToInt32();
		private static readonly int ofsClassGUID = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "ClassGUID").ToInt32();
		private static readonly int ofsMfg = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "Mfg").ToInt32();
		private static readonly int ofsDeviceDesc = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DeviceDesc").ToInt32();
		private static readonly int ofsService = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "Service").ToInt32();
		private static readonly int ofsSymbolicLink = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "SymbolicLink").ToInt32();
		private static readonly int ofsDevicePath = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DevicePath").ToInt32();
		private static readonly int ofsLUsb0FilterIndex = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "LUsb0FilterIndex").ToInt32();
		private static readonly int ofsConnected = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "Connected").ToInt32();
		private static readonly int ofsSyncFlags = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "SyncFlags").ToInt32();
		private static readonly int ofsBusNumber = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "BusNumber").ToInt32();
		private static readonly int ofsDeviceAddress = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "DeviceAddress").ToInt32();
		private static readonly int ofsSerialNumber = Marshal.OffsetOf(typeof(KLST_DEVINFO_MAP), "SerialNumber").ToInt32();


		/// <Summary>Common usb device information</Summary>
		public KLST_DEV_COMMON_INFO Common
		{
			get
			{
				return (KLST_DEV_COMMON_INFO)Marshal.PtrToStructure(new IntPtr(mHandlePtr.ToInt64() + ofsCommon), typeof(KLST_DEV_COMMON_INFO));
			}
		}


		/// <Summary>Driver id this device element is using</Summary>
		public int DriverID
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsDriverID);
			}
		}


		/// <Summary>Device interface GUID</Summary>
		public string DeviceInterfaceGUID
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsDeviceInterfaceGUID));
			}
		}


		/// <Summary>Device instance ID.</Summary>
		public string DeviceID
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsDeviceID));
			}
		}


		/// <Summary>Class GUID.</Summary>
		public string ClassGUID
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsClassGUID));
			}
		}


		/// <Summary>Manufacturer name as specified in the INF file.</Summary>
		public string Mfg
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsMfg));
			}
		}


		/// <Summary>Device description as specified in the INF file.</Summary>
		public string DeviceDesc
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsDeviceDesc));
			}
		}


		/// <Summary>Driver service name.</Summary>
		public string Service
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsService));
			}
		}


		/// <Summary>Unique identifier.</Summary>
		public string SymbolicLink
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsSymbolicLink));
			}
		}


		/// <Summary>physical device filename used with the Windows \c CreateFile()</Summary>
		public string DevicePath
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsDevicePath));
			}
		}


		/// <Summary>libusb-win32 filter index id.</Summary>
		public int LUsb0FilterIndex
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsLUsb0FilterIndex);
			}
		}


		/// <Summary>Indicates the devices connection state.</Summary>
		public bool Connected
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsConnected) != 0;
			}
		}


		/// <Summary>Synchronization flags. (internal use only)</Summary>
		public KLST_SYNC_FLAG SyncFlags
		{
			get
			{
				return (KLST_SYNC_FLAG)Marshal.ReadInt32(mHandlePtr, ofsSyncFlags);
			}
		}


		public int BusNumber
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsBusNumber);
			}
		}


		public int DeviceAddress
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsDeviceAddress);
			}
		}


		/// <Summary>If the the device is serialized, represents the string value of \ref USB_DEVICE_DESCRIPTOR::iSerialNumber. For Devices without a \b iSerialNumber, represents the unique \b InstanceID assigned by \b Windows.</Summary>
		public string SerialNumber
		{
			get
			{
				return Marshal.PtrToStringAnsi(new IntPtr(mHandlePtr.ToInt64() + ofsSerialNumber));
			}
		}


		public override string ToString()
		{
			return
				string.Format(
							  "DriverID: {0}\nDeviceInterfaceGUID: {1}\nDeviceID: {2}\nClassGUID: {3}\nMfg: {4}\nDeviceDesc: {5}\nService: {6}\nSymbolicLink: {7}\nDevicePath: {8}\nLUsb0FilterIndex: {9}\nConnected: {10}\nSyncFlags: {11}\nBusNumber: {12}\nDeviceAddress: {13}\nSerialNumber: {14}\n",
							  DriverID,
							  DeviceInterfaceGUID,
							  DeviceID,
							  ClassGUID,
							  Mfg,
							  DeviceDesc,
							  Service,
							  SymbolicLink,
							  DevicePath,
							  LUsb0FilterIndex,
							  Connected,
							  SyncFlags.ToString(),
							  BusNumber,
							  DeviceAddress,
							  SerialNumber);
		}
	}

	/// <Summary>Device list/hot-plug pattern match structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 1024)]
	public struct KLST_PATTERN_MATCH
	{
		/// <Summary>Pattern match a device instance id.</Summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] public string DeviceID;

		/// <Summary>Pattern match a device interface guid.</Summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] public string DeviceInterfaceGUID;

		/// <Summary>Pattern match a symbolic link.</Summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] public string ClassGUID;


		public override string ToString()
		{
			return string.Format("DeviceID: {0}\nDeviceInterfaceGUID: {1}\nClassGUID: {2}\n", DeviceID, DeviceInterfaceGUID, ClassGUID);
		}
	};

	/// <Summary>A structure representing the standard USB device descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_DEVICE_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>USB specification release number in binary-coded decimal.</Summary>
		public ushort bcdUSB;

		/// <Summary>USB-IF class code for the device</Summary>
		public byte bDeviceClass;

		/// <Summary>USB-IF subclass code for the device</Summary>
		public byte bDeviceSubClass;

		/// <Summary>USB-IF protocol code for the device</Summary>
		public byte bDeviceProtocol;

		/// <Summary>Maximum packet size for control endpoint 0</Summary>
		public byte bMaxPacketSize0;

		/// <Summary>USB-IF vendor ID</Summary>
		public ushort idVendor;

		/// <Summary>USB-IF product ID</Summary>
		public ushort idProduct;

		/// <Summary>Device release number in binary-coded decimal</Summary>
		public ushort bcdDevice;

		/// <Summary>Index of string descriptor describing manufacturer</Summary>
		public byte iManufacturer;

		/// <Summary>Index of string descriptor describing product</Summary>
		public byte iProduct;

		/// <Summary>Index of string descriptor containing device serial number</Summary>
		public byte iSerialNumber;

		/// <Summary>Number of possible configurations</Summary>
		public byte bNumConfigurations;


		public override string ToString()
		{
			return
				string.Format(
							  "bLength: {0}\nbDescriptorType: {1}\nbcdUSB: {2}\nbDeviceClass: {3}\nbDeviceSubClass: {4}\nbDeviceProtocol: {5}\nbMaxPacketSize0: {6}\nidVendor: {7}\nidProduct: {8}\nbcdDevice: {9}\niManufacturer: {10}\niProduct: {11}\niSerialNumber: {12}\nbNumConfigurations: {13}\n",
							  bLength,
							  bDescriptorType.ToString("X2") + "h",
							  bcdUSB.ToString("X4") + "h",
							  bDeviceClass.ToString("X2") + "h",
							  bDeviceSubClass.ToString("X2") + "h",
							  bDeviceProtocol.ToString("X2") + "h",
							  bMaxPacketSize0,
							  idVendor.ToString("X4") + "h",
							  idProduct.ToString("X4") + "h",
							  bcdDevice.ToString("X4") + "h",
							  iManufacturer,
							  iProduct,
							  iSerialNumber,
							  bNumConfigurations);
		}
	};

	/// <Summary>A structure representing the standard USB endpoint descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_ENDPOINT_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>The address of the endpoint described by this descriptor.</Summary>
		public byte bEndpointAddress;

		/// <Summary>Attributes which apply to the endpoint when it is configured using the bConfigurationValue.</Summary>
		public byte bmAttributes;

		/// <Summary>Maximum packet size this endpoint is capable of sending/receiving.</Summary>
		public ushort wMaxPacketSize;

		/// <Summary>Interval for polling endpoint for data transfers.</Summary>
		public byte bInterval;


		public override string ToString()
		{
			return string.Format("bLength: {0}\nbDescriptorType: {1}\nbEndpointAddress: {2}\nbmAttributes: {3}\nwMaxPacketSize: {4}\nbInterval: {5}\n",
								 bLength,
								 bDescriptorType.ToString("X2") + "h",
								 bEndpointAddress.ToString("X2") + "h",
								 bmAttributes.ToString("X2") + "h",
								 wMaxPacketSize,
								 bInterval);
		}
	};

	/// <Summary>A structure representing the standard USB configuration descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_CONFIGURATION_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>Total length of data returned for this configuration</Summary>
		public ushort wTotalLength;

		/// <Summary>Number of interfaces supported by this configuration</Summary>
		public byte bNumInterfaces;

		/// <Summary>Identifier value for this configuration</Summary>
		public byte bConfigurationValue;

		/// <Summary>Index of string descriptor describing this configuration</Summary>
		public byte iConfiguration;

		/// <Summary>Configuration characteristics</Summary>
		public byte bmAttributes;

		/// <Summary>Maximum power consumption of the USB device from this bus in this configuration when the device is fully operation.</Summary>
		public byte MaxPower;

		public override string ToString()
		{
			return string.Format("bLength: {0}\nbDescriptorType: {1}\nwTotalLength: {2}\nbNumInterfaces: {3}\nbConfigurationValue: {4}\niConfiguration: {5}\nbmAttributes: {6}\nMaxPower: {7}\n",
								 bLength,
								 bDescriptorType.ToString("X2") + "h",
								 wTotalLength,
								 bNumInterfaces,
								 bConfigurationValue,
								 iConfiguration,
								 bmAttributes.ToString("X2") + "h",
								 MaxPower);
		}
	};

	/// <Summary>A structure representing the standard USB interface descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_INTERFACE_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>Number of this interface</Summary>
		public byte bInterfaceNumber;

		/// <Summary>Value used to select this alternate setting for this interface</Summary>
		public byte bAlternateSetting;

		/// <Summary>Number of endpoints used by this interface (excluding the control endpoint)</Summary>
		public byte bNumEndpoints;

		/// <Summary>USB-IF class code for this interface</Summary>
		public byte bInterfaceClass;

		/// <Summary>USB-IF subclass code for this interface</Summary>
		public byte bInterfaceSubClass;

		/// <Summary>USB-IF protocol code for this interface</Summary>
		public byte bInterfaceProtocol;

		/// <Summary>Index of string descriptor describing this interface</Summary>
		public byte iInterface;


		public override string ToString()
		{
			return
				string.Format(
							  "bLength: {0}\nbDescriptorType: {1}\nbInterfaceNumber: {2}\nbAlternateSetting: {3}\nbNumEndpoints: {4}\nbInterfaceClass: {5}\nbInterfaceSubClass: {6}\nbInterfaceProtocol: {7}\niInterface: {8}\n",
							  bLength,
							  bDescriptorType.ToString("X2") + "h",
							  bInterfaceNumber,
							  bAlternateSetting,
							  bNumEndpoints,
							  bInterfaceClass.ToString("X2") + "h",
							  bInterfaceSubClass.ToString("X2") + "h",
							  bInterfaceProtocol.ToString("X2") + "h",
							  iInterface);
		}
	};

	/// <Summary>A structure representing the standard USB string descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
	public struct USB_STRING_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>Content of the string</Summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = AllKConstants.KLST_STRING_MAX_LEN)] public string bString;


		public override string ToString()
		{
			return string.Format("bLength: {0}\nbDescriptorType: {1}\nbString: {2}\n", bLength, bDescriptorType.ToString("X2") + "h", bString);
		}
	};

	/// <Summary>A structure representing the common USB descriptor.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_COMMON_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;


		public override string ToString()
		{
			return string.Format("bLength: {0}\nbDescriptorType: {1}\n", bLength, bDescriptorType.ToString("X2") + "h");
		}
	};

	/// <Summary>Allows hardware manufacturers to define groupings of interfaces.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct USB_INTERFACE_ASSOCIATION_DESCRIPTOR
	{
		/// <Summary>Size of this descriptor (in bytes)</Summary>
		public byte bLength;

		/// <Summary>Descriptor type</Summary>
		public byte bDescriptorType;

		/// <Summary>First interface number of the set of interfaces that follow this descriptor</Summary>
		public byte bFirstInterface;

		/// <Summary>The Number of interfaces follow this descriptor that are considered "associated"</Summary>
		public byte bInterfaceCount;

		/// <Summary>\c bInterfaceClass used for this associated interfaces</Summary>
		public byte bFunctionClass;

		/// <Summary>\c bInterfaceSubClass used for the associated interfaces</Summary>
		public byte bFunctionSubClass;

		/// <Summary>\c bInterfaceProtocol used for the associated interfaces</Summary>
		public byte bFunctionProtocol;

		/// <Summary>Index of string descriptor describing the associated interfaces</Summary>
		public byte iFunction;


		public override string ToString()
		{
			return string.Format("bLength: {0}\nbDescriptorType: {1}\nbFirstInterface: {2}\nbInterfaceCount: {3}\nbFunctionClass: {4}\nbFunctionSubClass: {5}\nbFunctionProtocol: {6}\niFunction: {7}\n",
								 bLength,
								 bDescriptorType.ToString("X2") + "h",
								 bFirstInterface,
								 bInterfaceCount,
								 bFunctionClass.ToString("X2") + "h",
								 bFunctionSubClass.ToString("X2") + "h",
								 bFunctionProtocol.ToString("X2") + "h",
								 iFunction);
		}
	};

	/// <Summary>USB core driver API information structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct KUSB_DRIVER_API_INFO
	{
		/// <Summary>\readonly Driver id of the driver api.</Summary>
		public int DriverID;

		/// <Summary>\readonly Number of valid functions contained in the driver API.</Summary>
		public int FunctionCount;


		public override string ToString()
		{
			return string.Format("DriverID: {0}\nFunctionCount: {1}\n", DriverID, FunctionCount);
		}
	};

	/// <Summary>Driver API function set structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 512)]
	public struct KUSB_DRIVER_API
	{
		/// <Summary>Driver API information.</Summary>
		public KUSB_DRIVER_API_INFO Info;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_InitDelegate Init;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_FreeDelegate Free;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ClaimInterfaceDelegate ClaimInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ReleaseInterfaceDelegate ReleaseInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SetAltInterfaceDelegate SetAltInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetAltInterfaceDelegate GetAltInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetDescriptorDelegate GetDescriptor;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ControlTransferDelegate ControlTransfer;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SetPowerPolicyDelegate SetPowerPolicy;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetPowerPolicyDelegate GetPowerPolicy;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SetConfigurationDelegate SetConfiguration;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetConfigurationDelegate GetConfiguration;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ResetDeviceDelegate ResetDevice;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_InitializeDelegate Initialize;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SelectInterfaceDelegate SelectInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetAssociatedInterfaceDelegate GetAssociatedInterface;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_CloneDelegate Clone;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_QueryInterfaceSettingsDelegate QueryInterfaceSettings;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_QueryDeviceInformationDelegate QueryDeviceInformation;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SetCurrentAlternateSettingDelegate SetCurrentAlternateSetting;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetCurrentAlternateSettingDelegate GetCurrentAlternateSetting;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_QueryPipeDelegate QueryPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_SetPipePolicyDelegate SetPipePolicy;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetPipePolicyDelegate GetPipePolicy;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ReadPipeDelegate ReadPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_WritePipeDelegate WritePipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_ResetPipeDelegate ResetPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_AbortPipeDelegate AbortPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_FlushPipeDelegate FlushPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_IsoReadPipeDelegate IsoReadPipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_IsoWritePipeDelegate IsoWritePipe;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetCurrentFrameNumberDelegate GetCurrentFrameNumber;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetOverlappedResultDelegate GetOverlappedResult;


		[MarshalAs(UnmanagedType.FunctionPtr)] public KUSB_GetPropertyDelegate GetProperty;
	};

	/// <Summary>Hot plug parameter structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 2048)]
	public struct KHOT_PARAMS
	{
		/// <Summary>Hot plug event window handle to send/post messages when notifications occur.</Summary>
		public IntPtr UserHwnd;

		/// <Summary>WM_USER message start offset used when sending/posting messages, See details.</Summary>
		public int UserMessage;

		/// <Summary>Additional init/config parameters</Summary>
		public KHOT_FLAG Flags;

		/// <Summary>File pattern matches for restricting notifcations to a single/group or all supported usb devices.</Summary>
		public KLST_PATTERN_MATCH PatternMatch;

		/// <Summary>Hot plug event callback function invoked when notifications occur.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KHOT_PLUG_CB OnHotPlug;


		public override string ToString()
		{
			return string.Format("UserHwnd: {0}\nUserMessage: {1}\nFlags: {2}\n", UserHwnd.ToString("X16") + "h", UserMessage.ToString("X8") + "h", Flags.ToString());
		}
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct KSTM_XFER_CONTEXT
	{
		private readonly IntPtr mHandlePtr;

		public KSTM_XFER_CONTEXT(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		/// <summary>KSTM_XFER_CONTEXT_MAP is used for calculating field offsets only</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct KSTM_XFER_CONTEXT_MAP
		{
			/// <Summary>Internal stream buffer.</Summary>
			private readonly IntPtr Buffer;

			/// <Summary>Size of internal stream buffer.</Summary>
			private readonly int BufferSize;

			/// <Summary>Number of bytes to write or number of bytes read.</Summary>
			private readonly int TransferLength;

			/// <Summary>User defined state.</Summary>
			private readonly IntPtr UserState;
		};

		private static readonly int ofsBuffer = Marshal.OffsetOf(typeof(KSTM_XFER_CONTEXT_MAP), "Buffer").ToInt32();
		private static readonly int ofsBufferSize = Marshal.OffsetOf(typeof(KSTM_XFER_CONTEXT_MAP), "BufferSize").ToInt32();
		private static readonly int ofsTransferLength = Marshal.OffsetOf(typeof(KSTM_XFER_CONTEXT_MAP), "TransferLength").ToInt32();
		private static readonly int ofsUserState = Marshal.OffsetOf(typeof(KSTM_XFER_CONTEXT_MAP), "UserState").ToInt32();


		/// <Summary>Internal stream buffer.</Summary>
		public IntPtr Buffer
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsBuffer);
			}
		}


		/// <Summary>Size of internal stream buffer.</Summary>
		public int BufferSize
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsBufferSize);
			}
		}


		/// <Summary>Number of bytes to write or number of bytes read.</Summary>
		public int TransferLength
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsTransferLength);
			}
		}


		/// <Summary>User defined state.</Summary>
		public IntPtr UserState
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsUserState);
			}
			set
			{
				Marshal.WriteIntPtr(mHandlePtr, ofsUserState, value);
			}
		}


		public override string ToString()
		{
			return string.Format("Buffer: {0}\nBufferSize: {1}\nTransferLength: {2}\nUserState: {3}\n", Buffer.ToString("X16") + "h", BufferSize, TransferLength, UserState.ToString("X16") + "h");
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct KSTM_INFO
	{
		private readonly IntPtr mHandlePtr;

		public KSTM_INFO(IntPtr Handle)
		{
			mHandlePtr = Handle;
		}

		public IntPtr Pointer
		{
			get
			{
				return mHandlePtr;
			}
		}

		/// <summary>KSTM_INFO_MAP is used for calculating field offsets only</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private struct KSTM_INFO_MAP
		{
			/// <Summary>\ref KUSB_HANDLE this stream uses.</Summary>
			private readonly IntPtr UsbHandle;

			/// <Summary>This parameter corresponds to the bEndpointAddress field in the endpoint descriptor.</Summary>
			private readonly byte PipeID;

			/// <Summary>Maximum transfer read/write request allowed pending.</Summary>
			private readonly int MaxPendingTransfers;

			/// <Summary>Maximum transfer sage size.</Summary>
			private readonly int MaxTransferSize;

			/// <Summary>Maximum number of I/O request allowed pending.</Summary>
			private readonly int MaxPendingIO;

			/// <Summary>Populated with the endpoint descriptor for the specified \c PipeID.</Summary>
			private readonly USB_ENDPOINT_DESCRIPTOR EndpointDescriptor;

			/// <Summary>Populated with the driver api for the specified \c UsbHandle.</Summary>
			private readonly KUSB_DRIVER_API DriverAPI;

			/// <Summary>Populated with the device file handle for the specified \c UsbHandle.</Summary>
			private readonly IntPtr DeviceHandle;

			/// <Summary>Stream handle.</Summary>
			private readonly IntPtr StreamHandle;

			/// <Summary>Stream info user defined state.</Summary>
			private readonly IntPtr UserState;
		};

		private static readonly int ofsUsbHandle = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "UsbHandle").ToInt32();
		private static readonly int ofsPipeID = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "PipeID").ToInt32();
		private static readonly int ofsMaxPendingTransfers = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "MaxPendingTransfers").ToInt32();
		private static readonly int ofsMaxTransferSize = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "MaxTransferSize").ToInt32();
		private static readonly int ofsMaxPendingIO = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "MaxPendingIO").ToInt32();
		private static readonly int ofsEndpointDescriptor = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "EndpointDescriptor").ToInt32();
		private static readonly int ofsDriverAPI = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "DriverAPI").ToInt32();
		private static readonly int ofsDeviceHandle = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "DeviceHandle").ToInt32();
		private static readonly int ofsStreamHandle = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "StreamHandle").ToInt32();
		private static readonly int ofsUserState = Marshal.OffsetOf(typeof(KSTM_INFO_MAP), "UserState").ToInt32();


		/// <Summary>\ref KUSB_HANDLE this stream uses.</Summary>
		public IntPtr UsbHandle
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsUsbHandle);
			}
		}


		/// <Summary>This parameter corresponds to the bEndpointAddress field in the endpoint descriptor.</Summary>
		public byte PipeID
		{
			get
			{
				return Marshal.ReadByte(mHandlePtr, ofsPipeID);
			}
		}


		/// <Summary>Maximum transfer read/write request allowed pending.</Summary>
		public int MaxPendingTransfers
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsMaxPendingTransfers);
			}
		}


		/// <Summary>Maximum transfer sage size.</Summary>
		public int MaxTransferSize
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsMaxTransferSize);
			}
		}


		/// <Summary>Maximum number of I/O request allowed pending.</Summary>
		public int MaxPendingIO
		{
			get
			{
				return Marshal.ReadInt32(mHandlePtr, ofsMaxPendingIO);
			}
		}


		/// <Summary>Populated with the endpoint descriptor for the specified \c PipeID.</Summary>
		public USB_ENDPOINT_DESCRIPTOR EndpointDescriptor
		{
			get
			{
				return (USB_ENDPOINT_DESCRIPTOR)Marshal.PtrToStructure(new IntPtr(mHandlePtr.ToInt64() + ofsEndpointDescriptor), typeof(USB_ENDPOINT_DESCRIPTOR));
			}
		}


		/// <Summary>Populated with the driver api for the specified \c UsbHandle.</Summary>
		public KUSB_DRIVER_API DriverAPI
		{
			get
			{
				return (KUSB_DRIVER_API)Marshal.PtrToStructure(new IntPtr(mHandlePtr.ToInt64() + ofsDriverAPI), typeof(KUSB_DRIVER_API));
			}
		}


		/// <Summary>Populated with the device file handle for the specified \c UsbHandle.</Summary>
		public IntPtr DeviceHandle
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsDeviceHandle);
			}
		}


		/// <Summary>Stream handle.</Summary>
		public IntPtr StreamHandle
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsStreamHandle);
			}
		}


		/// <Summary>Stream info user defined state.</Summary>
		public IntPtr UserState
		{
			get
			{
				return Marshal.ReadIntPtr(mHandlePtr, ofsUserState);
			}
			set
			{
				Marshal.WriteIntPtr(mHandlePtr, ofsUserState, value);
			}
		}


		public override string ToString()
		{
			return string.Format("UsbHandle: {0}\nPipeID: {1}\nMaxPendingTransfers: {2}\nMaxTransferSize: {3}\nMaxPendingIO: {4}\nDeviceHandle: {5}\nStreamHandle: {6}\nUserState: {7}\n",
								 UsbHandle.ToString("X16") + "h",
								 PipeID.ToString("X2") + "h",
								 MaxPendingTransfers,
								 MaxTransferSize,
								 MaxPendingIO,
								 DeviceHandle.ToString("X16") + "h",
								 StreamHandle.ToString("X16") + "h",
								 UserState.ToString("X16") + "h");
		}
	}

	/// <Summary>Stream callback structure.</Summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 64)]
	public struct KSTM_CALLBACK
	{
		/// <Summary>Executed when a transfer error occurs.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_ERROR_CB Error;

		/// <Summary>Executed to submit a transfer.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_SUBMIT_CB Submit;

		/// <Summary>Executed when a valid transfer completes.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_COMPLETE_CB Complete;

		/// <Summary>Executed for every transfer context when the stream is started with \ref StmK_Start.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_STARTED_CB Started;

		/// <Summary>Executed for every transfer context when the stream is stopped with \ref StmK_Stop.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_STOPPED_CB Stopped;

		/// <Summary>Executed immediately after a transfer completes.</Summary>
		[MarshalAs(UnmanagedType.FunctionPtr)] public KSTM_BEFORE_COMPLETE_CB BeforeComplete;
	};

	#endregion

	#region Delegates

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KLIB_HANDLE_CLEANUP_CB([In] IntPtr Handle, KLIB_HANDLE_TYPE HandleType, IntPtr UserContext);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KISO_ENUM_PACKETS_CB(int PacketIndex, [In] ref KISO_PACKET IsoPacket, IntPtr UserState);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KLST_ENUM_DEVINFO_CB([In] KLST_HANDLE DeviceList, [In] KLST_DEVINFO_HANDLE DeviceInfo, IntPtr Context);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_InitDelegate([Out] out KUSB_HANDLE InterfaceHandle, [In] KLST_DEVINFO_HANDLE DevInfo);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_FreeDelegate([In] KUSB_HANDLE InterfaceHandle);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ClaimInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte NumberOrIndex, bool IsIndex);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ReleaseInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte NumberOrIndex, bool IsIndex);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SetAltInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte NumberOrIndex, bool IsIndex, byte AltSettingNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetAltInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte NumberOrIndex, bool IsIndex, out byte AltSettingNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetDescriptorDelegate([In] KUSB_HANDLE InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, IntPtr Buffer, int BufferLength, out int LengthTransferred);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ControlTransferDelegate([In] KUSB_HANDLE InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SetPowerPolicyDelegate([In] KUSB_HANDLE InterfaceHandle, int PolicyType, int ValueLength, IntPtr Value);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetPowerPolicyDelegate([In] KUSB_HANDLE InterfaceHandle, int PolicyType, ref int ValueLength, IntPtr Value);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SetConfigurationDelegate([In] KUSB_HANDLE InterfaceHandle, byte ConfigurationNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetConfigurationDelegate([In] KUSB_HANDLE InterfaceHandle, out byte ConfigurationNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ResetDeviceDelegate([In] KUSB_HANDLE InterfaceHandle);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_InitializeDelegate(IntPtr DeviceHandle, [Out] out KUSB_HANDLE InterfaceHandle);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SelectInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte NumberOrIndex, bool IsIndex);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetAssociatedInterfaceDelegate([In] KUSB_HANDLE InterfaceHandle, byte AssociatedInterfaceIndex, [Out] out KUSB_HANDLE AssociatedInterfaceHandle);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_CloneDelegate([In] KUSB_HANDLE InterfaceHandle, [Out] out KUSB_HANDLE DstInterfaceHandle);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_QueryInterfaceSettingsDelegate([In] KUSB_HANDLE InterfaceHandle, byte AltSettingIndex, [Out] out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_QueryDeviceInformationDelegate([In] KUSB_HANDLE InterfaceHandle, int InformationType, ref int BufferLength, IntPtr Buffer);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SetCurrentAlternateSettingDelegate([In] KUSB_HANDLE InterfaceHandle, byte AltSettingNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetCurrentAlternateSettingDelegate([In] KUSB_HANDLE InterfaceHandle, out byte AltSettingNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_QueryPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte AltSettingNumber, byte PipeIndex, [Out] out WINUSB_PIPE_INFORMATION PipeInformation);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_SetPipePolicyDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, int PolicyType, int ValueLength, IntPtr Value);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetPipePolicyDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, int PolicyType, ref int ValueLength, IntPtr Value);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ReadPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_WritePipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_ResetPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_AbortPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_FlushPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_IsoReadPipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, IntPtr Buffer, int BufferLength, IntPtr Overlapped, [In] KISO_CONTEXT IsoContext);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_IsoWritePipeDelegate([In] KUSB_HANDLE InterfaceHandle, byte PipeID, IntPtr Buffer, int BufferLength, IntPtr Overlapped, [In] KISO_CONTEXT IsoContext);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetCurrentFrameNumberDelegate([In] KUSB_HANDLE InterfaceHandle, out int FrameNumber);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetOverlappedResultDelegate([In] KUSB_HANDLE InterfaceHandle, IntPtr Overlapped, out int lpNumberOfBytesTransferred, bool bWait);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate bool KUSB_GetPropertyDelegate([In] KUSB_HANDLE InterfaceHandle, KUSB_PROPERTY PropertyType, ref int PropertySize, IntPtr Value);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate void KHOT_PLUG_CB([In] KHOT_HANDLE HotHandle, [In] KLST_DEVINFO_HANDLE DeviceInfo, KLST_SYNC_FLAG PlugType);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KSTM_ERROR_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex, int ErrorCode);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KSTM_SUBMIT_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex, IntPtr Overlapped);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KSTM_STARTED_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KSTM_STOPPED_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate int KSTM_COMPLETE_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex, int ErrorCode);

	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi, SetLastError = true)]
	public delegate KSTM_COMPLETE_RESULT KSTM_BEFORE_COMPLETE_CB([In] KSTM_INFO StreamInfo, [In] KSTM_XFER_CONTEXT XferContext, int XferContextIndex, ref int ErrorCode);

	#endregion

	public class LstK : IDisposable
	{
		protected KLST_HANDLE mHandleStruct;
		protected bool mbDisposed;

		protected LstK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~LstK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.LstK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KLST_HANDLE(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Initializes a new usb device list containing all supported devices.</Summary>
		protected bool Init(KLST_FLAG Flags)
		{
			bool success = AllKFunctions.LstK_Init(out mHandleStruct, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		/// <Summary>Initializes a new usb device list containing only devices matching a specific class GUID.</Summary>
		protected bool InitEx(KLST_FLAG Flags, ref KLST_PATTERN_MATCH PatternMatch)
		{
			bool success = AllKFunctions.LstK_InitEx(out mHandleStruct, Flags, ref PatternMatch);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Public Members

		/// <Summary>Initializes a new usb device list containing all supported devices.</Summary>
		public LstK(KLST_FLAG Flags)
		{
			bool success = AllKFunctions.LstK_Init(out mHandleStruct, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Initializes a new usb device list containing only devices matching a specific class GUID.</Summary>
		public LstK(KLST_FLAG Flags, ref KLST_PATTERN_MATCH PatternMatch)
		{
			bool success = AllKFunctions.LstK_InitEx(out mHandleStruct, Flags, ref PatternMatch);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Counts the number of device info elements in a device list.</Summary>
		public virtual bool Count(ref int Count)
		{
			return AllKFunctions.LstK_Count(mHandleStruct, ref Count);
		}

		/// <Summary>Gets the \ref KLST_DEVINFO element for the current position.</Summary>
		public virtual bool Current(out KLST_DEVINFO_HANDLE DeviceInfo)
		{
			return AllKFunctions.LstK_Current(mHandleStruct, out DeviceInfo);
		}

		/// <Summary>Enumerates \ref KLST_DEVINFO elements of a \ref KLST_HANDLE.</Summary>
		public virtual bool Enumerate(KLST_ENUM_DEVINFO_CB EnumDevListCB, IntPtr Context)
		{
			return AllKFunctions.LstK_Enumerate(mHandleStruct, EnumDevListCB, Context);
		}

		/// <Summary>Find a device by vendor and product id</Summary>
		public virtual bool FindByVidPid(int Vid, int Pid, out KLST_DEVINFO_HANDLE DeviceInfo)
		{
			return AllKFunctions.LstK_FindByVidPid(mHandleStruct, Vid, Pid, out DeviceInfo);
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <summary>Gets the handle class structure.</summary>
		public KLST_HANDLE Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		/// <Summary>Advances the device list current \ref KLST_DEVINFO position.</Summary>
		public virtual bool MoveNext(out KLST_DEVINFO_HANDLE DeviceInfo)
		{
			return AllKFunctions.LstK_MoveNext(mHandleStruct, out DeviceInfo);
		}

		/// <Summary>Sets the device list to its initial position, which is before the first element in the list.</Summary>
		public virtual void MoveReset()
		{
			AllKFunctions.LstK_MoveReset(mHandleStruct);
		}

		#endregion
	}

	public class HotK : IDisposable
	{
		protected KHOT_HANDLE mHandleStruct;
		protected bool mbDisposed;

		protected HotK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~HotK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.HotK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KHOT_HANDLE(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Creates a new hot-plug handle for USB device arrival/removal event monitoring.</Summary>
		protected bool Init(ref KHOT_PARAMS InitParams)
		{
			bool success = AllKFunctions.HotK_Init(out mHandleStruct, ref InitParams);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Public Members

		/// <Summary>Creates a new hot-plug handle for USB device arrival/removal event monitoring.</Summary>
		public HotK(ref KHOT_PARAMS InitParams)
		{
			bool success = AllKFunctions.HotK_Init(out mHandleStruct, ref InitParams);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <Summary>Frees all hot-plug handles initialized with \ref HotK_Init.</Summary>
		public virtual void FreeAll()
		{
			AllKFunctions.HotK_FreeAll();
		}

		/// <summary>Gets the handle class structure.</summary>
		public KHOT_HANDLE Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		#endregion
	}

	public class UsbK : IDisposable
	{
		protected KUSB_DRIVER_API driverAPI;
		protected KUSB_HANDLE mHandleStruct;
		protected bool mbDisposed;

		protected UsbK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~UsbK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.UsbK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KUSB_HANDLE(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Creates/opens a libusbK interface handle from the device list. This is a preferred method.</Summary>
		protected bool Init(KLST_DEVINFO_HANDLE DevInfo)
		{
			bool success = AllKFunctions.LibK_LoadDriverAPI(out driverAPI, DevInfo.DriverID);

			if (!success) throw new Exception(string.Format("{0} failed loading Driver API. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			success = driverAPI.Init(out mHandleStruct, DevInfo);

			if (!success)
				throw new Exception(string.Format("{0} failed initializing usb device. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		/// <Summary>Creates a libusbK handle for the device specified by a file handle.</Summary>
		protected bool Initialize(IntPtr DeviceHandle, KUSB_DRVID driverID)
		{
			bool success = AllKFunctions.LibK_LoadDriverAPI(out driverAPI, (int)driverID);

			if (!success) throw new Exception(string.Format("{0} failed loading Driver API. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			success = driverAPI.Initialize(DeviceHandle, out mHandleStruct);

			if (!success)
				throw new Exception(string.Format("{0} failed initializing usb device. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Public Members

		/// <Summary>Creates/opens a libusbK interface handle from the device list. This is a preferred method.</Summary>
		public UsbK(KLST_DEVINFO_HANDLE DevInfo)
		{
			bool success = AllKFunctions.LibK_LoadDriverAPI(out driverAPI, DevInfo.DriverID);

			if (!success) throw new Exception(string.Format("{0} failed loading Driver API. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			success = driverAPI.Init(out mHandleStruct, DevInfo);

			if (!success)
				throw new Exception(string.Format("{0} failed initializing usb device. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Creates a libusbK handle for the device specified by a file handle.</Summary>
		public UsbK(IntPtr DeviceHandle, KUSB_DRVID driverID)
		{
			bool success = AllKFunctions.LibK_LoadDriverAPI(out driverAPI, (int)driverID);

			if (!success) throw new Exception(string.Format("{0} failed loading Driver API. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			success = driverAPI.Initialize(DeviceHandle, out mHandleStruct);

			if (!success)
				throw new Exception(string.Format("{0} failed initializing usb device. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Aborts all of the pending transfers for a pipe.</Summary>
		public virtual bool AbortPipe(byte PipeID)
		{
			return driverAPI.AbortPipe(mHandleStruct, PipeID);
		}

		/// <Summary>Claims the specified interface by number or index.</Summary>
		public virtual bool ClaimInterface(byte NumberOrIndex, bool IsIndex)
		{
			return driverAPI.ClaimInterface(mHandleStruct, NumberOrIndex, IsIndex);
		}

		/// <Summary>Clones the specified interface handle.</Summary>
		public virtual bool Clone(out KUSB_HANDLE DstInterfaceHandle)
		{
			return driverAPI.Clone(mHandleStruct, out DstInterfaceHandle);
		}

		/// <Summary>Transmits control data over a default control endpoint.</Summary>
		public virtual bool ControlTransfer(WINUSB_SETUP_PACKET SetupPacket, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.ControlTransfer(mHandleStruct, SetupPacket, Buffer, BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Transmits control data over a default control endpoint.</Summary>
		public virtual bool ControlTransfer(WINUSB_SETUP_PACKET SetupPacket, Array Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.ControlTransfer(mHandleStruct, SetupPacket, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Transmits control data over a default control endpoint.</Summary>
		public virtual bool ControlTransfer(WINUSB_SETUP_PACKET SetupPacket, Array Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.ControlTransfer(mHandleStruct, SetupPacket, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		/// <Summary>Transmits control data over a default control endpoint.</Summary>
		public virtual bool ControlTransfer(WINUSB_SETUP_PACKET SetupPacket, IntPtr Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.ControlTransfer(mHandleStruct, SetupPacket, Buffer, BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		/// <Summary>Discards any data that is cached in a pipe.</Summary>
		public virtual bool FlushPipe(byte PipeID)
		{
			return driverAPI.FlushPipe(mHandleStruct, PipeID);
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <Summary>Gets the alternate setting for the specified interface.</Summary>
		public virtual bool GetAltInterface(byte NumberOrIndex, bool IsIndex, out byte AltSettingNumber)
		{
			return driverAPI.GetAltInterface(mHandleStruct, NumberOrIndex, IsIndex, out AltSettingNumber);
		}

		/// <Summary>Retrieves a handle for an associated interface.</Summary>
		public virtual bool GetAssociatedInterface(byte AssociatedInterfaceIndex, out KUSB_HANDLE AssociatedInterfaceHandle)
		{
			return driverAPI.GetAssociatedInterface(mHandleStruct, AssociatedInterfaceIndex, out AssociatedInterfaceHandle);
		}

		/// <Summary>Gets the device current configuration number.</Summary>
		public virtual bool GetConfiguration(out byte ConfigurationNumber)
		{
			return driverAPI.GetConfiguration(mHandleStruct, out ConfigurationNumber);
		}

		/// <Summary>Gets the current alternate interface setting for an interface.</Summary>
		public virtual bool GetCurrentAlternateSetting(out byte AltSettingNumber)
		{
			return driverAPI.GetCurrentAlternateSetting(mHandleStruct, out AltSettingNumber);
		}

		/// <Summary>Retrieves the current USB frame number.</Summary>
		public virtual bool GetCurrentFrameNumber(out int FrameNumber)
		{
			return driverAPI.GetCurrentFrameNumber(mHandleStruct, out FrameNumber);
		}

		/// <Summary>Gets the requested descriptor. This is a synchronous operation.</Summary>
		public virtual bool GetDescriptor(byte DescriptorType, byte Index, int LanguageID, IntPtr Buffer, int BufferLength, out int LengthTransferred)
		{
			return driverAPI.GetDescriptor(mHandleStruct, DescriptorType, Index, (ushort)LanguageID, Buffer, BufferLength, out LengthTransferred);
		}

		/// <Summary>Gets the requested descriptor. This is a synchronous operation.</Summary>
		public virtual bool GetDescriptor(byte DescriptorType, byte Index, int LanguageID, Array Buffer, int BufferLength, out int LengthTransferred)
		{
			return driverAPI.GetDescriptor(mHandleStruct, DescriptorType, Index, (ushort)LanguageID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred);
		}

		/// <Summary>Retrieves the results of an overlapped operation on the specified libusbK handle.</Summary>
		public virtual bool GetOverlappedResult(IntPtr Overlapped, out int lpNumberOfBytesTransferred, bool bWait)
		{
			return driverAPI.GetOverlappedResult(mHandleStruct, Overlapped, out lpNumberOfBytesTransferred, bWait);
		}

		/// <Summary>Retrieves the results of an overlapped operation on the specified libusbK handle.</Summary>
		public virtual bool GetOverlappedResult(KOVL_HANDLE Overlapped, out int lpNumberOfBytesTransferred, bool bWait)
		{
			return driverAPI.GetOverlappedResult(mHandleStruct, Overlapped.Pointer, out lpNumberOfBytesTransferred, bWait);
		}

		/// <Summary>Gets the policy for a specific pipe (endpoint).</Summary>
		public virtual bool GetPipePolicy(byte PipeID, int PolicyType, ref int ValueLength, IntPtr Value)
		{
			return driverAPI.GetPipePolicy(mHandleStruct, PipeID, PolicyType, ref ValueLength, Value);
		}

		/// <Summary>Gets the policy for a specific pipe (endpoint).</Summary>
		public virtual bool GetPipePolicy(byte PipeID, int PolicyType, ref int ValueLength, Array Value)
		{
			return driverAPI.GetPipePolicy(mHandleStruct, PipeID, PolicyType, ref ValueLength, Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0));
		}

		/// <Summary>Gets the power policy for a device.</Summary>
		public virtual bool GetPowerPolicy(int PolicyType, ref int ValueLength, IntPtr Value)
		{
			return driverAPI.GetPowerPolicy(mHandleStruct, PolicyType, ref ValueLength, Value);
		}

		/// <Summary>Gets the power policy for a device.</Summary>
		public virtual bool GetPowerPolicy(int PolicyType, ref int ValueLength, Array Value)
		{
			return driverAPI.GetPowerPolicy(mHandleStruct, PolicyType, ref ValueLength, Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0));
		}

		/// <Summary>Gets a USB device (driver specific) property from usb handle.</Summary>
		public virtual bool GetProperty(KUSB_PROPERTY PropertyType, ref int PropertySize, IntPtr Value)
		{
			return driverAPI.GetProperty(mHandleStruct, PropertyType, ref PropertySize, Value);
		}

		/// <Summary>Gets a USB device (driver specific) property from usb handle.</Summary>
		public virtual bool GetProperty(KUSB_PROPERTY PropertyType, ref int PropertySize, Array Value)
		{
			return driverAPI.GetProperty(mHandleStruct, PropertyType, ref PropertySize, Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0));
		}

		/// <summary>Gets the handle class structure.</summary>
		public KUSB_HANDLE Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		/// <Summary>Reads from an isochronous pipe.</Summary>
		public virtual bool IsoReadPipe(byte PipeID, IntPtr Buffer, int BufferLength, IntPtr Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoReadPipe(mHandleStruct, PipeID, Buffer, BufferLength, Overlapped, IsoContext);
		}

		/// <Summary>Reads from an isochronous pipe.</Summary>
		public virtual bool IsoReadPipe(byte PipeID, Array Buffer, int BufferLength, IntPtr Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoReadPipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, Overlapped, IsoContext);
		}

		/// <Summary>Reads from an isochronous pipe.</Summary>
		public virtual bool IsoReadPipe(byte PipeID, Array Buffer, int BufferLength, KOVL_HANDLE Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoReadPipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, Overlapped.Pointer, IsoContext);
		}

		/// <Summary>Reads from an isochronous pipe.</Summary>
		public virtual bool IsoReadPipe(byte PipeID, IntPtr Buffer, int BufferLength, KOVL_HANDLE Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoReadPipe(mHandleStruct, PipeID, Buffer, BufferLength, Overlapped.Pointer, IsoContext);
		}

		/// <Summary>Writes to an isochronous pipe.</Summary>
		public virtual bool IsoWritePipe(byte PipeID, IntPtr Buffer, int BufferLength, IntPtr Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoWritePipe(mHandleStruct, PipeID, Buffer, BufferLength, Overlapped, IsoContext);
		}

		/// <Summary>Writes to an isochronous pipe.</Summary>
		public virtual bool IsoWritePipe(byte PipeID, Array Buffer, int BufferLength, IntPtr Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoWritePipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, Overlapped, IsoContext);
		}

		/// <Summary>Writes to an isochronous pipe.</Summary>
		public virtual bool IsoWritePipe(byte PipeID, Array Buffer, int BufferLength, KOVL_HANDLE Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoWritePipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, Overlapped.Pointer, IsoContext);
		}

		/// <Summary>Writes to an isochronous pipe.</Summary>
		public virtual bool IsoWritePipe(byte PipeID, IntPtr Buffer, int BufferLength, KOVL_HANDLE Overlapped, KISO_CONTEXT IsoContext)
		{
			return driverAPI.IsoWritePipe(mHandleStruct, PipeID, Buffer, BufferLength, Overlapped.Pointer, IsoContext);
		}

		/// <Summary>Retrieves information about the physical device that is associated with a libusbK handle.</Summary>
		public virtual bool QueryDeviceInformation(int InformationType, ref int BufferLength, IntPtr Buffer)
		{
			return driverAPI.QueryDeviceInformation(mHandleStruct, InformationType, ref BufferLength, Buffer);
		}

		/// <Summary>Retrieves the interface descriptor for the specified alternate interface settings for a particular interface handle.</Summary>
		public virtual bool QueryInterfaceSettings(byte AltSettingIndex, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor)
		{
			return driverAPI.QueryInterfaceSettings(mHandleStruct, AltSettingIndex, out UsbAltInterfaceDescriptor);
		}

		/// <Summary>Retrieves information about a pipe that is associated with an interface.</Summary>
		public virtual bool QueryPipe(byte AltSettingNumber, byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation)
		{
			return driverAPI.QueryPipe(mHandleStruct, AltSettingNumber, PipeIndex, out PipeInformation);
		}

		/// <Summary>Reads data from the specified pipe.</Summary>
		public virtual bool ReadPipe(byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.ReadPipe(mHandleStruct, PipeID, Buffer, BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Reads data from the specified pipe.</Summary>
		public virtual bool ReadPipe(byte PipeID, Array Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.ReadPipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Reads data from the specified pipe.</Summary>
		public virtual bool ReadPipe(byte PipeID, Array Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.ReadPipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		/// <Summary>Reads data from the specified pipe.</Summary>
		public virtual bool ReadPipe(byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.ReadPipe(mHandleStruct, PipeID, Buffer, BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		/// <Summary>Releases the specified interface by number or index.</Summary>
		public virtual bool ReleaseInterface(byte NumberOrIndex, bool IsIndex)
		{
			return driverAPI.ReleaseInterface(mHandleStruct, NumberOrIndex, IsIndex);
		}

		/// <Summary>Resets the usb device of the specified interface handle. (port cycle).</Summary>
		public virtual bool ResetDevice()
		{
			return driverAPI.ResetDevice(mHandleStruct);
		}

		/// <Summary>Resets the data toggle and clears the stall condition on a pipe.</Summary>
		public virtual bool ResetPipe(byte PipeID)
		{
			return driverAPI.ResetPipe(mHandleStruct, PipeID);
		}

		/// <Summary>Selects the specified interface by number or index as the current interface.</Summary>
		public virtual bool SelectInterface(byte NumberOrIndex, bool IsIndex)
		{
			return driverAPI.SelectInterface(mHandleStruct, NumberOrIndex, IsIndex);
		}

		/// <Summary>Sets the alternate setting of the specified interface.</Summary>
		public virtual bool SetAltInterface(byte NumberOrIndex, bool IsIndex, byte AltSettingNumber)
		{
			return driverAPI.SetAltInterface(mHandleStruct, NumberOrIndex, IsIndex, AltSettingNumber);
		}

		/// <Summary>Sets the device configuration number.</Summary>
		public virtual bool SetConfiguration(byte ConfigurationNumber)
		{
			return driverAPI.SetConfiguration(mHandleStruct, ConfigurationNumber);
		}

		/// <Summary>Sets the alternate setting of an interface.</Summary>
		public virtual bool SetCurrentAlternateSetting(byte AltSettingNumber)
		{
			return driverAPI.SetCurrentAlternateSetting(mHandleStruct, AltSettingNumber);
		}

		/// <Summary>Sets the policy for a specific pipe associated with an endpoint on the device. This is a synchronous operation.</Summary>
		public virtual bool SetPipePolicy(byte PipeID, int PolicyType, int ValueLength, IntPtr Value)
		{
			return driverAPI.SetPipePolicy(mHandleStruct, PipeID, PolicyType, ValueLength, Value);
		}

		/// <Summary>Sets the policy for a specific pipe associated with an endpoint on the device. This is a synchronous operation.</Summary>
		public virtual bool SetPipePolicy(byte PipeID, int PolicyType, int ValueLength, Array Value)
		{
			return driverAPI.SetPipePolicy(mHandleStruct, PipeID, PolicyType, ValueLength, Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0));
		}

		/// <Summary>Sets the power policy for a device.</Summary>
		public virtual bool SetPowerPolicy(int PolicyType, int ValueLength, IntPtr Value)
		{
			return driverAPI.SetPowerPolicy(mHandleStruct, PolicyType, ValueLength, Value);
		}

		/// <Summary>Sets the power policy for a device.</Summary>
		public virtual bool SetPowerPolicy(int PolicyType, int ValueLength, Array Value)
		{
			return driverAPI.SetPowerPolicy(mHandleStruct, PolicyType, ValueLength, Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0));
		}

		/// <Summary>Writes data to a pipe.</Summary>
		public virtual bool WritePipe(byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.WritePipe(mHandleStruct, PipeID, Buffer, BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Writes data to a pipe.</Summary>
		public virtual bool WritePipe(byte PipeID, Array Buffer, int BufferLength, out int LengthTransferred, IntPtr Overlapped)
		{
			return driverAPI.WritePipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped);
		}

		/// <Summary>Writes data to a pipe.</Summary>
		public virtual bool WritePipe(byte PipeID, Array Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.WritePipe(mHandleStruct, PipeID, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		/// <Summary>Writes data to a pipe.</Summary>
		public virtual bool WritePipe(byte PipeID, IntPtr Buffer, int BufferLength, out int LengthTransferred, KOVL_HANDLE Overlapped)
		{
			return driverAPI.WritePipe(mHandleStruct, PipeID, Buffer, BufferLength, out LengthTransferred, Overlapped.Pointer);
		}

		#endregion
	}

	public class OvlK : IDisposable
	{
		protected KOVL_POOL_HANDLE mHandleStruct;
		protected bool mbDisposed;

		protected OvlK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~OvlK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.OvlK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KOVL_POOL_HANDLE(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Creates a new overlapped pool.</Summary>
		protected bool Init(KUSB_HANDLE UsbHandle, int MaxOverlappedCount, KOVL_POOL_FLAG Flags)
		{
			bool success = AllKFunctions.OvlK_Init(out mHandleStruct, UsbHandle, MaxOverlappedCount, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Public Members

		/// <Summary>Creates a new overlapped pool.</Summary>
		public OvlK(KUSB_HANDLE UsbHandle, int MaxOverlappedCount, KOVL_POOL_FLAG Flags)
		{
			bool success = AllKFunctions.OvlK_Init(out mHandleStruct, UsbHandle, MaxOverlappedCount, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Gets a preallocated \c OverlappedK structure from the specified/default pool.</Summary>
		public virtual bool Acquire(out KOVL_HANDLE OverlappedK)
		{
			return AllKFunctions.OvlK_Acquire(out OverlappedK, mHandleStruct);
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <Summary>Returns the internal event handle used to signal IO operations.</Summary>
		public virtual IntPtr GetEventHandle(KOVL_HANDLE OverlappedK)
		{
			return AllKFunctions.OvlK_GetEventHandle(OverlappedK);
		}

		/// <summary>Gets the handle class structure.</summary>
		public KOVL_POOL_HANDLE Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		/// <Summary>Checks for i/o completion; returns immediately. (polling)</Summary>
		public virtual bool IsComplete(KOVL_HANDLE OverlappedK)
		{
			return AllKFunctions.OvlK_IsComplete(OverlappedK);
		}

		/// <Summary>Initializes an overlappedK for re-use. The overlappedK is not return to its pool.</Summary>
		public virtual bool ReUse(KOVL_HANDLE OverlappedK)
		{
			return AllKFunctions.OvlK_ReUse(OverlappedK);
		}

		/// <Summary>Returns an \c OverlappedK structure to it's pool.</Summary>
		public virtual bool Release(KOVL_HANDLE OverlappedK)
		{
			return AllKFunctions.OvlK_Release(OverlappedK);
		}

		/// <Summary>Waits for overlapped I/O completion, and performs actions specified in \c WaitFlags.</Summary>
		public virtual bool Wait(KOVL_HANDLE OverlappedK, int TimeoutMS, KOVL_WAIT_FLAG WaitFlags, out int TransferredLength)
		{
			return AllKFunctions.OvlK_Wait(OverlappedK, TimeoutMS, WaitFlags, out TransferredLength);
		}

		/// <Summary>Waits for overlapped I/O completion, cancels on a timeout error and always releases the OvlK handle back to its pool.</Summary>
		public virtual bool WaitAndRelease(KOVL_HANDLE OverlappedK, int TimeoutMS, out int TransferredLength)
		{
			return AllKFunctions.OvlK_WaitAndRelease(OverlappedK, TimeoutMS, out TransferredLength);
		}

		/// <Summary>Waits for overlapped I/O completion on the oldest acquired OverlappedK handle and performs actions specified in \c WaitFlags.</Summary>
		public virtual bool WaitOldest(out KOVL_HANDLE OverlappedK, int TimeoutMS, KOVL_WAIT_FLAG WaitFlags, out int TransferredLength)
		{
			return AllKFunctions.OvlK_WaitOldest(mHandleStruct, out OverlappedK, TimeoutMS, WaitFlags, out TransferredLength);
		}

		/// <Summary>Waits for overlapped I/O completion, cancels on a timeout error.</Summary>
		public virtual bool WaitOrCancel(KOVL_HANDLE OverlappedK, int TimeoutMS, out int TransferredLength)
		{
			return AllKFunctions.OvlK_WaitOrCancel(OverlappedK, TimeoutMS, out TransferredLength);
		}

		#endregion
	}

	public class StmK : IDisposable
	{
		protected KSTM_HANDLE mHandleStruct;
		protected bool mbDisposed;

		protected StmK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~StmK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.StmK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KSTM_HANDLE(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Initializes a new uni-directional pipe stream.</Summary>
		protected bool Init(KUSB_HANDLE UsbHandle, byte PipeID, int MaxTransferSize, int MaxPendingTransfers, int MaxPendingIO, ref KSTM_CALLBACK Callbacks, KSTM_FLAG Flags)
		{
			bool success = AllKFunctions.StmK_Init(out mHandleStruct, UsbHandle, PipeID, MaxTransferSize, MaxPendingTransfers, MaxPendingIO, ref Callbacks, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Public Members

		/// <Summary>Initializes a new uni-directional pipe stream.</Summary>
		public StmK(KUSB_HANDLE UsbHandle, byte PipeID, int MaxTransferSize, int MaxPendingTransfers, int MaxPendingIO, ref KSTM_CALLBACK Callbacks, KSTM_FLAG Flags)
		{
			bool success = AllKFunctions.StmK_Init(out mHandleStruct, UsbHandle, PipeID, MaxTransferSize, MaxPendingTransfers, MaxPendingIO, ref Callbacks, Flags);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <summary>Gets the handle class structure.</summary>
		public KSTM_HANDLE Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		/// <Summary>Reads data from the stream buffer.</Summary>
		public virtual bool Read(IntPtr Buffer, int Offset, int Length, out int TransferredLength)
		{
			return AllKFunctions.StmK_Read(mHandleStruct, Buffer, Offset, Length, out TransferredLength);
		}

		/// <Summary>Reads data from the stream buffer.</Summary>
		public virtual bool Read(Array Buffer, int Offset, int Length, out int TransferredLength)
		{
			return AllKFunctions.StmK_Read(mHandleStruct, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), Offset, Length, out TransferredLength);
		}

		/// <Summary>Starts the internal stream thread.</Summary>
		public virtual bool Start()
		{
			return AllKFunctions.StmK_Start(mHandleStruct);
		}

		/// <Summary>Stops the internal stream thread.</Summary>
		public virtual bool Stop(int TimeoutCancelMS)
		{
			return AllKFunctions.StmK_Stop(mHandleStruct, TimeoutCancelMS);
		}

		/// <Summary>Writes data to the stream buffer.</Summary>
		public virtual bool Write(IntPtr Buffer, int Offset, int Length, out int TransferredLength)
		{
			return AllKFunctions.StmK_Write(mHandleStruct, Buffer, Offset, Length, out TransferredLength);
		}

		/// <Summary>Writes data to the stream buffer.</Summary>
		public virtual bool Write(Array Buffer, int Offset, int Length, out int TransferredLength)
		{
			return AllKFunctions.StmK_Write(mHandleStruct, Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0), Offset, Length, out TransferredLength);
		}

		#endregion
	}

	public class IsoK : IDisposable
	{
		protected KISO_CONTEXT mHandleStruct;
		protected bool mbDisposed;

		protected IsoK()
		{
		}

		#region IDisposable Members

		/// <summary>Explicitly closes and frees the handle.</summary>
		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~IsoK()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!mbDisposed)
			{
				if (mHandleStruct.Pointer != IntPtr.Zero)
				{
					AllKFunctions.IsoK_Free(mHandleStruct);
					Debug.Print("{0} Dispose: Freed Handle:{1:X16}h Explicit:{2}", GetType().Name, mHandleStruct.Pointer.ToInt64(), disposing);
				}
				else
					Debug.Print("{0} Dispose: [WARNING] Handle is null", GetType().Name);

				mHandleStruct = new KISO_CONTEXT(IntPtr.Zero);
				mbDisposed = true;
			}
		}

		/// <Summary>Creates a new isochronous transfer context.</Summary>
		protected bool Init(int NumberOfPackets, int StartFrame)
		{
			bool success = AllKFunctions.IsoK_Init(out mHandleStruct, NumberOfPackets, StartFrame);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
			return true;
		}

		#region Nested Structs

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		private struct KISO_CONTEXT_MAP
		{
			/// <Summary>Additional ISO transfer flags. See \ref KISO_FLAG.</Summary>
			private readonly KISO_FLAG Flags;

			/// <Summary>Specifies the frame number that the transfer should begin on (0 for ASAP).</Summary>
			private readonly int StartFrame;

			/// <Summary>Contains the number of packets that completed with an error condition on return from the host controller driver.</Summary>
			private readonly short ErrorCount;

			/// <Summary>Specifies the number of packets that are described by the variable-length array member \c IsoPacket.</Summary>
			private readonly short NumberOfPackets;

			/// <Summary>Contains the URB Hdr.Status value on return from the host controller driver.</Summary>
			private readonly int UrbHdrStatus;
		};

		#endregion

		#region Public Members

		/// <Summary>Creates a new isochronous transfer context.</Summary>
		public IsoK(int NumberOfPackets, int StartFrame)
		{
			bool success = AllKFunctions.IsoK_Init(out mHandleStruct, NumberOfPackets, StartFrame);

			if (!success) throw new Exception(string.Format("{0} failed initializing. ErrorCode={1:X8}h", GetType().Name, Marshal.GetLastWin32Error()));

			Debug.Print("{0} Init: handle 0x{1:X16}", GetType().Name, mHandleStruct.Pointer.ToInt64());
		}

		/// <Summary>Convenience function for enumerating ISO packets of an isochronous transfer context.</Summary>
		public virtual bool EnumPackets(KISO_ENUM_PACKETS_CB EnumPackets, int StartPacketIndex, IntPtr UserState)
		{
			return AllKFunctions.IsoK_EnumPackets(mHandleStruct, EnumPackets, StartPacketIndex, UserState);
		}

		/// <Summary>Contains the number of packets that completed with an error condition on return from the host controller driver.</Summary>
		public short ErrorCount
		{
			get
			{
				return Marshal.ReadInt16(mHandleStruct.Pointer, ofsErrorCount);
			}
			set
			{
				Marshal.WriteInt16(mHandleStruct.Pointer, ofsErrorCount, value);
			}
		}

		/// <Summary>Additional ISO transfer flags. See \ref KISO_FLAG.</Summary>
		public KISO_FLAG Flags
		{
			get
			{
				return (KISO_FLAG)Marshal.ReadInt32(mHandleStruct.Pointer, ofsFlags);
			}
			set
			{
				Marshal.WriteInt32(mHandleStruct.Pointer, ofsFlags, (int)value);
			}
		}

		/// <summary>Calls the dispose method.</summary>
		public virtual void Free()
		{
			Dispose();
		}

		/// <Summary>Convenience function for getting all fields of a \ref KISO_PACKET.</Summary>
		public virtual bool GetPacket(int PacketIndex, out KISO_PACKET IsoPacket)
		{
			return AllKFunctions.IsoK_GetPacket(mHandleStruct, PacketIndex, out IsoPacket);
		}

		/// <summary>Gets the handle class structure.</summary>
		public KISO_CONTEXT Handle
		{
			get
			{
				return mHandleStruct;
			}
		}

		/// <Summary>Specifies the number of packets that are described by the variable-length array member \c IsoPacket.</Summary>
		public short NumberOfPackets
		{
			get
			{
				return Marshal.ReadInt16(mHandleStruct.Pointer, ofsNumberOfPackets);
			}
			set
			{
				Marshal.WriteInt16(mHandleStruct.Pointer, ofsNumberOfPackets, value);
			}
		}

		/// <Summary>Convenience function for re-using an isochronous transfer context in a subsequent request.</Summary>
		public virtual bool ReUse()
		{
			return AllKFunctions.IsoK_ReUse(mHandleStruct);
		}

		/// <Summary>Convenience function for setting all fields of a \ref KISO_PACKET.</Summary>
		public virtual bool SetPacket(int PacketIndex, ref KISO_PACKET IsoPacket)
		{
			return AllKFunctions.IsoK_SetPacket(mHandleStruct, PacketIndex, ref IsoPacket);
		}

		/// <Summary>Convenience function for setting the offset of all ISO packets of an isochronous transfer context.</Summary>
		public virtual bool SetPackets(int PacketSize)
		{
			return AllKFunctions.IsoK_SetPackets(mHandleStruct, PacketSize);
		}

		/// <Summary>Specifies the frame number that the transfer should begin on (0 for ASAP).</Summary>
		public int StartFrame
		{
			get
			{
				return Marshal.ReadInt32(mHandleStruct.Pointer, ofsStartFrame);
			}
			set
			{
				Marshal.WriteInt32(mHandleStruct.Pointer, ofsStartFrame, value);
			}
		}

		/// <Summary>Contains the URB Hdr.Status value on return from the host controller driver.</Summary>
		public int UrbHdrStatus
		{
			get
			{
				return Marshal.ReadInt32(mHandleStruct.Pointer, ofsUrbHdrStatus);
			}
			set
			{
				Marshal.WriteInt32(mHandleStruct.Pointer, ofsUrbHdrStatus, value);
			}
		}

		#endregion

		#region Private Members

		private static readonly int ofsErrorCount = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "ErrorCount").ToInt32();
		private static readonly int ofsFlags = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "Flags").ToInt32();
		private static readonly int ofsNumberOfPackets = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "NumberOfPackets").ToInt32();
		private static readonly int ofsStartFrame = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "StartFrame").ToInt32();
		private static readonly int ofsUrbHdrStatus = Marshal.OffsetOf(typeof(KISO_CONTEXT_MAP), "UrbHdrStatus").ToInt32();

		#endregion
	}
}