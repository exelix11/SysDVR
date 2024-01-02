// Copyright © 2006-2009 Travis Robinson. All rights reserved.
//
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or
// visit www.gnu.org.
//
//

using LibUsbDotNet.LibUsb;
using System;
using System.IO;

namespace LibUsbDotNet.Main;

/// <summary>
/// A stream which reads and writes of an USB connection.
/// </summary>
public class UsbStream : Stream
{
    private readonly UsbEndpointWriter writer;
    private readonly UsbEndpointReader reader;
    private readonly byte[] readBuffer = new byte[4096];
    private int readBufferOffset = 0;
    private int readBufferLength = 0;
    private long position = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsbStream"/> class.
    /// </summary>
    /// <param name="writer">
    /// A <see cref="UsbEndpointWriter"/> to which to write data.
    /// </param>
    /// <param name="reader">
    /// A <see cref="UsbEndpointReader"/> from which to read data.
    /// </param>
    public UsbStream(UsbEndpointWriter writer, UsbEndpointReader reader)
    {
        if (writer == null && reader == null)
        {
            throw new ArgumentException("You must provide at least a reader or a writer");
        }

        this.writer = writer;
        this.reader = reader;
    }

    /// <inheritdoc/>
    public override bool CanRead => this.reader != null;

    /// <inheritdoc/>
    public override bool CanSeek => false;

    /// <inheritdoc/>
    public override bool CanWrite => this.writer != null;

    /// <inheritdoc/>
    public override long Length => throw new NotSupportedException();

    /// <inheritdoc/>
    public override long Position
    {
        get => this.position;
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        // Don't block on calls to read 0 bytes of data.
        if (count == 0)
        {
            return 0;
        }

        // If the buffer has been exhausted, fetch new data
        if (this.readBufferOffset >= this.readBufferLength)
        {
            this.readBufferOffset = 0;
            this.reader.Read(this.readBuffer, this.readBufferOffset, this.readBuffer.Length, -1, out int transferLength).ThrowOnError();

            this.readBufferLength = transferLength;
        }

        // Read data from the buffer and return that to the caller.
        int bytesAvailable = this.readBufferLength - this.readBufferOffset;
        int read = Math.Min(bytesAvailable, count);

        Array.Copy(this.readBuffer, this.readBufferOffset, buffer, offset, read);

        this.readBufferOffset += read;

        this.position += read;
        return read;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        this.writer.Write(buffer, offset, count, timeout: 1000, transferLength: out int transferLength).ThrowOnError();
    }
}