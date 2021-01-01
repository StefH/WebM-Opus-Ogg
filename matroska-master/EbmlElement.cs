using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Matroska
{
    /// <summary>
    /// Represents an EBML element.
    /// </summary>
    public class EbmlElement : Stream
    {
        #region Fields

        public const long UnknownLength = -1;

        private const int bufSize = 65536;
        private const int maxHeaderSize = 16;

        private readonly Stream stream;
        private long position;
        private long length;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the element ID.
        /// </summary>
        public uint ID { get; }

        // public string Name => MatroskaSpecificationEbml.Elements[ID].Name;

        /// <summary>
        /// Gets the length of the element. Can be -1 (<see cref="UnknownLength"/>) in case the length is unknown.
        /// </summary>
        public override long Length => length;

        /// <summary>
        /// Gets or sets the position within the element.
        /// </summary>
        public override long Position
        {
            get => position;
            set
            {
                if (!CanSeek)
                    throw new NotSupportedException();

                long diff = value - position;
                stream.Seek(diff, SeekOrigin.Current);
                position = value;
            }
        }

        public override bool CanRead => stream.CanRead;
        public override bool CanSeek => stream.CanSeek;
        public override bool CanWrite => stream.CanWrite;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new EBML element based on the specified stream.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="length"></param>
        /// <param name="stream"></param>
        public EbmlElement(uint id, long length, Stream stream)
        {
            ID = id;
            this.length = length;
            this.stream = stream;
        }

        #endregion

        #region Methods

        #region Stream implementation

        /// <inheritdoc/>
        public override int Read(Span<byte> buffer)
        {
            int count = buffer.Length;
            if (Length >= 0)
            {
                long remaining = Length - position;
                if (count > remaining)
                    count = (int)remaining;
            }

            if (count == 0)
                return 0;

            int result = stream.Read(buffer.Slice(0, count));
            position += result;
            return result;
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        /// <inheritdoc/>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
        {
            int count = buffer.Length;
            if (Length >= 0)
            {
                long remaining = Length - position;
                if (count > remaining)
                    count = (int)remaining;
            }

            if (count == 0)
                return 0;

            int result = await stream.ReadAsync(buffer.Slice(0, count), cancellationToken);
            position += result;
            return result;
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();

        /// <summary>
        /// Reads to the end of the element's contents.
        /// </summary>
        public void ReadToEnd()
        {
            if (CanSeek)
            {
                Seek(0, SeekOrigin.End);
            }
            else
            {
                Span<byte> temp = stackalloc byte[4096];

                while (Read(temp) != 0)
                {

                }
            }
        }

        /// <summary>
        /// Reads to the end of the element's contents.
        /// </summary>
        public async ValueTask ReadToEndAsync(CancellationToken cancellationToken = default)
        {
            if (CanSeek)
            {
                Seek(0, SeekOrigin.End);
            }
            else
            {
                using IMemoryOwner<byte> mem = MemoryPool<byte>.Shared.Rent(bufSize);

                while (await ReadAsync(mem.Memory, cancellationToken) != 0)
                {

                }
            }
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            Span<byte> temp = stackalloc byte[1];
            int n = Read(temp);
            if (n == 0)
                return -1;
            return temp[0];
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => Write(new ReadOnlySpan<byte>(buffer, offset, count));

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (Length >= 0 && position + buffer.Length > Length)
                throw new NotSupportedException("Can't write past the end of the stream.");

            stream.Write(buffer);
            position += buffer.Length;
        }

        public override void WriteByte(byte value)
        {
            Span<byte> temp = stackalloc byte[1];
            temp[0] = value;
            Write(temp);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            stream.Flush();
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return stream.FlushAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.End && Length < 0)
                throw new NotSupportedException("Can't seek relative to the end of the element because the size of this element is not known.");

            if (origin == SeekOrigin.End)
                offset = Length - offset;
            else if (origin == SeekOrigin.Current)
                offset = position + offset;

            if (offset < 0)
                offset = 0;
            else if (Length >= 0 && offset > Length)
                offset = Length;

            Position = offset;
            return position;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            length = value;
            //throw new NotSupportedException();
        }

        #endregion

        #region EBML implementation

        /// <summary>
        /// Reads an EBML element from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static EbmlElement ReadElement(Stream stream)
        {
            uint? id = Ebml.TryReadElementID(stream);
            if (id == null)
                return null;

            long length = Ebml.ReadElementLength(stream);
            return new EbmlElement(id.Value, length, stream);
        }

        /// <summary>
        /// Reads an EBML element from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async ValueTask<EbmlElement> ReadElementAsync(Stream stream)
        {
            uint? id = await Ebml.TryReadElementIDAsync(stream);
            if (id == null)
                return null;

            long length = await Ebml.ReadElementLengthAsync(stream);
            return new EbmlElement(id.Value, length, stream);
        }

        /// <summary>
        /// Reads all EBML elements from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<EbmlElement> ReadElements(Stream stream)
        {
            EbmlElement element;

            while ((element = ReadElement(stream)) != null)
            {
                yield return element;

                element.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads all EBML elements from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<EbmlElement> ReadElementsAsync(Stream stream)
        {
            EbmlElement element;

            while ((element = await ReadElementAsync(stream)) != null)
            {
                yield return element;

                await element.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Creates a new element and writes the header to the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="id"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static EbmlElement WriteElement(Stream stream, uint id, long length)
        {
            Span<byte> header = stackalloc byte[maxHeaderSize];
            int len = Ebml.WriteHeader(header, id, length);
            stream.Write(header.Slice(0, len));
            return new EbmlElement(id, length, stream);
        }

        /// <summary>
        /// Creates a new element and writes the header to the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="id"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static async ValueTask<EbmlElement> WriteElementAsync(Stream stream, uint id, long length)
        {
            using IMemoryOwner<byte> temp = MemoryPool<byte>.Shared.Rent(maxHeaderSize);
            int len = Ebml.WriteHeader(temp.Memory.Span, id, length);
            await stream.WriteAsync(temp.Memory.Slice(0, len));
            return new EbmlElement(id, length, stream);
        }

        /// <summary>
        /// Copies just the contents of this element to the specified stream.
        /// </summary>
        /// <param name="dest">The destination stream.</param>
        /// <param name="fromBeginning">Whether to start copying from the beginning (true) or from the current position (false).</param>
        public void CopyContents(Stream dest, bool fromBeginning = true)
        {
            long posBackup = -1;

            if (fromBeginning && Position != 0)
            {
                if (!CanSeek)
                    throw new InvalidOperationException();

                posBackup = Position;
                Position = 0;
            }

            CopyTo(dest, bufSize);

            if (posBackup != -1)
            {
                Position = posBackup;
            }
        }

        /// <summary>
        /// Copies just the contents of this element to the specified stream.
        /// </summary>
        /// <param name="dest">The destination stream.</param>
        /// <param name="fromBeginning">Whether to start copying from the beginning (true) or from the current position (false).</param>
        /// <param name="cancellationToken"></param>
        public async ValueTask CopyContentsAsync(Stream dest, bool fromBeginning = true, CancellationToken cancellationToken = default)
        {
            long posBackup = -1;

            if (fromBeginning && Position != 0)
            {
                if (!CanSeek)
                    throw new InvalidOperationException();

                posBackup = Position;
                Position = 0;
            }

            await CopyToAsync(dest, bufSize, cancellationToken);

            if (posBackup != -1)
            {
                Position = posBackup;
            }
        }

        /// <summary>
        /// Copies this element (including its header) to the specified stream.
        /// </summary>
        /// <param name="dest">The destination stream.</param>
        /// <param name="toUnknownLength">Specifies whether an unknown length should be used for the new element instead of the actual length.</param>
        public void CopyElement(Stream dest, bool toUnknownLength = false)
        {
            Span<byte> header = stackalloc byte[maxHeaderSize];
            int len = Ebml.WriteHeader(header, ID, toUnknownLength ? UnknownLength : Length);
            dest.Write(header.Slice(0, len));
            CopyContents(dest, true);
        }

        /// <summary>
        /// Copies this element (including its header) to the specified stream.
        /// </summary>
        /// <param name="dest">The destination stream.</param>
        /// <param name="toUnknownLength">Specifies whether an unknown length should be used for the new element instead of the actual length.</param>
        /// <param name="cancellationToken"></param>
        public async ValueTask CopyElementAsync(Stream dest, bool toUnknownLength = false, CancellationToken cancellationToken = default)
        {
            using IMemoryOwner<byte> temp = MemoryPool<byte>.Shared.Rent(maxHeaderSize);
            int len = Ebml.WriteHeader(temp.Memory.Span, ID, toUnknownLength ? UnknownLength : Length);
            await dest.WriteAsync(temp.Memory.Slice(0, len), cancellationToken);
            await CopyContentsAsync(dest, true, cancellationToken);
        }

        /// <summary>
        /// Creates an in-memory clone of the EBML element.
        /// </summary>
        /// <param name="toUnknownLength">Specifies whether an unknown length should be used for the new element instead of the actual length.</param>
        /// <returns></returns>
        public EbmlElement CloneToMemory(bool toUnknownLength = false)
        {
            if (stream is CowStream cow)
                return new EbmlElement(ID, toUnknownLength ? UnknownLength : cow.Length, new CowStream(cow));

            MemoryStream temp = new MemoryStream();
            CopyContents(temp, true);

            Memory<byte> mem = new Memory<byte>(temp.GetBuffer(), 0, (int)temp.Length);
            return new EbmlElement(ID, toUnknownLength ? UnknownLength : length, new CowStream(mem));
        }

        /// <summary>
        /// Creates an in-memory clone of the EBML element.
        /// </summary>
        /// <param name="toUnknownLength">Specifies whether an unknown length should be used for the new element instead of the actual length.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<EbmlElement> CloneToMemoryAsync(bool toUnknownLength = false, CancellationToken cancellationToken = default)
        {
            if (stream is CowStream cow)
                return new EbmlElement(ID, toUnknownLength ? UnknownLength : cow.Length, new CowStream(cow));

            MemoryStream temp = new MemoryStream();
            await CopyContentsAsync(temp, true, cancellationToken);

            Memory<byte> mem = new Memory<byte>(temp.GetBuffer(), 0, (int)temp.Length);
            return new EbmlElement(ID, toUnknownLength ? UnknownLength : length, new CowStream(mem));
        }

        #endregion

        public override string ToString()
        {
            return $"0x{ID:X8} {Length} bytes";
        }

        #endregion
    }
}
