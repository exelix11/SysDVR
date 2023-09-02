using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SysDVR.Client.Sources
{
    // Making UsbContext non static will remove requirement on libusb, it will be loaded only if actually using USB, this is useful on architectures where it's not supported
    // This class should be used as a singleton
    abstract class DvrUsbDevice : IDisposable
    {
        protected readonly DvrUsbContext Context;
        public readonly DeviceInfo Info;
        
        public IUsbDevice DeviceHandle { get; protected set; }

        protected DvrUsbDevice(DvrUsbContext context, DeviceInfo deviceInfo)
        {            
            Context = context;
            Info = deviceInfo;
        }

        public virtual (UsbEndpointReader, UsbEndpointWriter) Open()
        {
            var (epIn, epOut) = (ReadEndpointID.Ep01, WriteEndpointID.Ep01);

            var reader = DeviceHandle.OpenEndpointReader(epIn, PacketHeader.MaxTransferSize, EndpointType.Bulk);
            var writer = DeviceHandle.OpenEndpointWriter(epOut, EndpointType.Bulk);

            return (reader, writer);
        }

        // Should call close and then initialize a new device handle
        public abstract bool TryReconnect();

        public virtual void Close()
        {
            DeviceHandle?.Dispose();
            DeviceHandle = null;
        }

        public virtual void Dispose()
        {
            Close();
        }
    }

    class DvrUsbContext
    {
        const ushort SysDVRVid = 0x18D1;
        const ushort SysDVRPid = 0x4EE0;

        static UsbContext LibUsbCtx = null;

        public UsbContext Libusb => LibUsbCtx;

        private UsbLogLevel _debugLevel;
        public UsbLogLevel DebugLevel
        {
            set
            {
                _debugLevel = value;
                LibUsbCtx.SetDebugLevel(value switch
                {
                    UsbLogLevel.Error => LibUsbDotNet.LogLevel.Error,
                    UsbLogLevel.Warning => LibUsbDotNet.LogLevel.Info,
                    UsbLogLevel.Debug => LibUsbDotNet.LogLevel.Debug,
                    _ => LibUsbDotNet.LogLevel.None,
                });
            }
            get => _debugLevel;
        }

        public DvrUsbContext(UsbLogLevel logLevel = UsbLogLevel.Error)
        {
            //Program.Instance.BugCheckThreadId();

            if (LibUsbCtx == null)
            {
#if ANDROID_LIB
                if (!Program.Native.PlatformSupportsUsb)
                    throw new Exception("This platform doesn't support USB");

                LibUsbDotNet.LibUsb.UsbContext.SetGlobalOption(LibUsbDotNet.LibUsbOption.NO_DEVICE_DISCOVERY, 0);
#endif
                LibUsbCtx = new LibUsbDotNet.LibUsb.UsbContext();
            }

            DebugLevel = logLevel;
        }

#if ANDROID_LIB
        public IReadOnlyList<DvrUsbDevice> FindSysdvrDevices()
        {            
            if (!Program.Native.UsbAcquireSnapshot(SysDVRVid, SysDVRPid, out var count))
                throw new Exception(Program.Native.UsbGetLastError());

            List<DvrUsbDevice> res = new List<DvrUsbDevice>();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var serial = Program.Native.UsbGetSnapshotDeviceSerial(i);
                    Console.WriteLine($"Device: {i} - {serial}");
                    var parsed = DeviceInfo.TryParse(ConnectionType.Usb, serial, serial);
                    if (parsed is not null)
                    {
                        var dev = new AndroidUsbDevice(this, parsed);
                        parsed.ConnectionHandle = dev;
                        res.Add(dev);
                    }
                }
            }
            finally
            {
                Program.Native.UsbReleaseSnapshot();
            }

            return res;
        }
#else
        static bool MatchSysdvrDevice(IUsbDevice device)
        {
            try
            {
                return device.VendorId == SysDVRVid && device.ProductId == SysDVRPid;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: failed to query device ID " + ex);
                return false;
            }
        }

        public IReadOnlyList<DvrUsbDevice> FindSysdvrDevices()
        {
            // THis is hacky but libusb can't seem to get the device serial without opening it first
            // If the device is already opened by another instance of sysdvr it will print an error, suppress it by temporarily changing the log level 
            var old = DebugLevel;
            DebugLevel = UsbLogLevel.None;

            var res = LibUsbCtx.FindAll(MatchSysdvrDevice).Select(x =>
            {
                try
                {
                    if (!x.TryOpen())
                        return null;

                    var serial = x.Info.SerialNumber.Trim();
                    x.Close();

                    var dev = DeviceInfo.TryParse(ConnectionType.Usb, serial, "usb device");
                    if (dev == null)
                        return null;

                    return new DesktopUsbDevice(this, dev, x);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: failed to query device serial " + ex);
                    return null;
                }
            }).Where(x => x != null).ToArray();

            DebugLevel = old;
            return res;
        }
#endif
    }

#if ANDROID_LIB
    class AndroidUsbDevice : DvrUsbDevice
    {
        nint NativeHandle = 0;

        public AndroidUsbDevice(DvrUsbContext context, DeviceInfo deviceInfo) : base(context, deviceInfo)
        {
            
        }

        public override (UsbEndpointReader, UsbEndpointWriter) Open()
        {
            if (!Program.Native.UsbOpenHandle(Info.ConnectionString, out NativeHandle))
                throw new Exception(Program.Native.UsbGetLastError());

            Console.WriteLine($"Obtained handle {NativeHandle}");

            DeviceHandle = Context.Libusb.WrapNativeDeviceHandle(NativeHandle);
            
            return base.Open();
        }

        public override void Close()
        {
            if (NativeHandle == 0)
                return;

            base.Close();
            Program.Native.UsbCloseHandle(NativeHandle);
            NativeHandle = 0;
        }

        public override bool TryReconnect()
        {
            Close();
            var dev = Context.FindSysdvrDevices().Where(X => X.Info.Serial == Info.Serial).FirstOrDefault();
            
            if (dev == null) // the device doesn't exist anymore
                return false;

            // The device exists, next open() may work
            return true;
        }
    }
#else
    class DesktopUsbDevice : DvrUsbDevice
    {
        public DesktopUsbDevice(DvrUsbContext ctx, DeviceInfo info, IUsbDevice dev) : base(ctx, info)
        {
            DeviceHandle = dev;
        }

        public override (UsbEndpointReader, UsbEndpointWriter) Open()
        {
            DeviceHandle.Open();

            if (!DeviceHandle.ClaimInterface(0))
                throw new Exception($"Couldn't claim device interface");
         
            return base.Open();
        }

        public override bool TryReconnect()
        {
            Close();
            var dev = Context.FindSysdvrDevices().Where(X => X.Info.Serial == Info.Serial).FirstOrDefault();
            if (dev == null)
                return false;

            DeviceHandle = dev.DeviceHandle;
            return true;
        }
    }
#endif
}
