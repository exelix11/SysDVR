using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public class ConstCharPtrMarshaler : ICustomMarshaler
{
    private static readonly ConstCharPtrMarshaler Instance = new();
    public object MarshalNativeToManaged(IntPtr pNativeData) => Marshal.PtrToStringAnsi(pNativeData);

    public IntPtr MarshalManagedToNative(object managedObj) => IntPtr.Zero;

    public void CleanUpNativeData(IntPtr pNativeData)
    {
    }

    public void CleanUpManagedData(object managedObj)
    {
    }

    public int GetNativeDataSize() => IntPtr.Size;

    public static ICustomMarshaler GetInstance(string cookie) => Instance;
}
