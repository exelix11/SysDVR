using System;
using System.Threading.Tasks;

namespace LibUsbDotNet.LibUsb;

public partial class UsbEndpointReader
{
        /// <summary>
        /// Reads data asynchronously from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the received data in.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
        /// </returns>
        public virtual async Task<(Error error, int transferLength)> ReadAsync(byte[] buffer, int timeout) => 
            await ReadAsync(buffer, 0, buffer.Length, timeout).ConfigureAwait(false);

        /// <summary>
        /// Reads data asynchronously from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the received data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="length">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
        /// </returns>
        public virtual async Task<(Error error, int transferLength)> ReadAsync(byte[] buffer, int offset, int length, int timeout) => 
            await TransferAsync(buffer, offset, length, timeout).ConfigureAwait(false);
        
        /// <summary>
        /// Reads data asynchronously from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the received data in.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
        /// </returns>
        public virtual async Task<(Error error, int transferLength)> ReadAsync(Memory<byte> buffer, int timeout) => 
            await ReadAsync(buffer, 0, buffer.Length, timeout).ConfigureAwait(false);
        
        /// <summary>
        /// Reads data asynchronously from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the received data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="length">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Tuple of (<see cref="Error"/> error, <see cref="int"/> transferLength). error is <see cref="Error.Success"/> on success.
        /// </returns>
        public virtual async Task<(Error error, int transferLength)> ReadAsync(Memory<byte> buffer, int offset, int length, int timeout) => 
            await TransferAsync(buffer, offset, length, timeout).ConfigureAwait(false);

        /// <summary>
        /// Reads/discards data asynchronously from the endpoint until no more data is available.
        /// </summary>
        /// <returns>Always returns <see cref="Error.Success"/> </returns>
        public virtual async Task<Error> ReadFlushAsync()
        {
            byte[] bufDummy = new byte[64];
            int iBufCount = 0;

            Error error = Error.Success;
            
            while (error == Error.Success && iBufCount < 128)
            {
                (error, _) = await ReadAsync(bufDummy, 10).ConfigureAwait(false);
                iBufCount++;
            }

            return Error.Success;
        }
}