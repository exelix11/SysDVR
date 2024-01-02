using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LibUsbDotNet.LibUsb;

public partial class UsbEndpointWriter
{
    /// <summary>
    /// Writes data asynchronously to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <returns>
    /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
    /// </returns>
    public virtual async Task<(Error error, int transferLength)> WriteAsync(byte[] buffer, int timeout) => 
        await WriteAsync(buffer, 0, buffer.Length, timeout).ConfigureAwait(false);

    /// <summary>
    /// Writes data asynchronously to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="offset">The position in buffer to start writing the data from.</param>
    /// <param name="length">The number of bytes to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <returns>
    /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
    /// </returns>
    public virtual async Task<(Error error, int transferLength)> WriteAsync(byte[] buffer, int offset, int length, int timeout) => 
        await TransferAsync(buffer, offset, length, timeout).ConfigureAwait(false);
    
    /// <summary>
    /// Writes data asynchronously to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <returns>
    /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
    /// </returns>
    public virtual async Task<(Error error, int transferLength)> WriteAsync(Memory<byte> buffer, int timeout) => 
        await WriteAsync(buffer, 0, buffer.Length, timeout).ConfigureAwait(false);

    /// <summary>
    /// Writes data asynchronously to the current <see cref="UsbEndpointWriter"/>.
    /// </summary>
    /// <param name="buffer">The buffer storing the data to write.</param>
    /// <param name="offset">The position in buffer to start writing the data from.</param>
    /// <param name="length">The number of bytes to write.</param>
    /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
    /// <returns>
    /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
    /// </returns>
    public virtual async Task<(Error error, int transferLength)> WriteAsync(Memory<byte> buffer, int offset, int length, int timeout) => 
        await TransferAsync(buffer, offset, length, timeout).ConfigureAwait(false);
}