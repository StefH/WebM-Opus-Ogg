using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Matroska
{
    public static class EbmlExtensions
    {
        private static void ReadExactly(Stream stream, Span<byte> buffer)
        {
            while (buffer.Length > 0)
            {
                int numBytes = stream.Read(buffer);
                if (numBytes == 0)
                {
                    throw new EndOfStreamException();
                }

                buffer = buffer.Slice(numBytes);
            }
        }

        private static async ValueTask readExactlyAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
        {
            while (buffer.Length > 0)
            {
                int numBytes = await stream.ReadAsync(buffer, cancellationToken);
                if (numBytes == 0)
                    throw new EndOfStreamException();

                buffer = buffer.Slice(numBytes);
            }
        }

        /// <summary>
        /// Reads the element's contents and interprets them as signed integer.
        /// </summary>
        /// <returns></returns>
        public static long ReadSignedInteger(this Stream stream)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0;

            if (len > 8)
                throw new InvalidDataException("Invalid length. Integer must not have more than 8 bytes.");

            Span<byte> buf = stackalloc byte[(int)len];
            ReadExactly(stream, buf);
            return Ebml.DecodeSInt(buf);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as signed integer.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<long> ReadSignedIntegerAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0;

            if (len > 8)
                throw new InvalidDataException("Invalid length. Integer must not have more than 8 bytes.");

            using IMemoryOwner<byte> temp = MemoryPool<byte>.Shared.Rent((int)len);
            Memory<byte> buf = temp.Memory.Slice(0, (int)len);
            await readExactlyAsync(stream, buf, cancellationToken);
            return Ebml.DecodeSInt(buf.Span);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as unsigned integer.
        /// </summary>
        /// <returns></returns>
        public static ulong ReadUnsignedInteger(this Stream stream)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0;

            if (len > 8)
                throw new InvalidDataException("Invalid length. Integer must not have more than 8 bytes.");

            Span<byte> buf = stackalloc byte[(int)len];
            ReadExactly(stream, buf);
            return Ebml.DecodeUInt(buf);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as unsigned integer.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<ulong> ReadUnsignedIntegerAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0;

            if (len > 8)
                throw new InvalidDataException("Invalid length. Integer must not have more than 8 bytes.");

            using IMemoryOwner<byte> temp = MemoryPool<byte>.Shared.Rent((int)len);
            Memory<byte> buf = temp.Memory.Slice(0, (int)len);
            await readExactlyAsync(stream, buf, cancellationToken);
            return Ebml.DecodeUInt(buf.Span);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as floating-point number.
        /// </summary>
        /// <returns></returns>
        public static double ReadFloat(this Stream stream)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0.0;

            if (len != 4 && len != 8)
                throw new InvalidDataException("Invalid length. Floats must have 0, 4 or 8 bytes.");

            Span<byte> buf = stackalloc byte[(int)len];
            ReadExactly(stream, buf);

            if (BitConverter.IsLittleEndian)
                buf.Reverse();

            if (len == 4)
                return BitConverter.ToSingle(buf);

            return BitConverter.ToDouble(buf);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as floating-point number.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<double> ReadFloatAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            long len = stream.Length;

            if (len == 0)
                return 0.0;

            if (len != 4 && len != 8)
                throw new InvalidDataException("Invalid length. Floats must have 0, 4 or 8 bytes.");

            using IMemoryOwner<byte> temp = MemoryPool<byte>.Shared.Rent((int)len);
            Memory<byte> buf = temp.Memory.Slice(0, (int)len);
            await readExactlyAsync(stream, buf, cancellationToken);

            if (BitConverter.IsLittleEndian)
                buf.Span.Reverse();

            if (len == 4)
                return BitConverter.ToSingle(buf.Span);

            return BitConverter.ToDouble(buf.Span);
        }

        private const int stringBufSize = 256;

        private static string decodeString(this Stream stream, Encoding encoding)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            Span<byte> buffer = stackalloc byte[stringBufSize];
            Span<char> chars = stackalloc char[stringBufSize];
            StringBuilder str = new StringBuilder();
            Decoder decoder = encoding.GetDecoder();

            for (; ; )
            {
                int numBytes = stream.Read(buffer);
                int numChars = decoder.GetChars(buffer.Slice(0, numBytes), chars, numBytes == 0);
                str.Append(chars.Slice(0, numChars));

                if (numBytes == 0)
                    break;
            }

            return str.ToString();
        }

        private static async ValueTask<string> decodeStringAsync(this Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            using IMemoryOwner<byte> temp1 = MemoryPool<byte>.Shared.Rent(stringBufSize);
            using IMemoryOwner<char> temp2 = MemoryPool<char>.Shared.Rent(stringBufSize);

            Memory<byte> buffer = temp1.Memory.Slice(0, stringBufSize);
            Memory<char> chars = temp2.Memory.Slice(0, stringBufSize);

            StringBuilder str = new StringBuilder();
            Decoder decoder = encoding.GetDecoder();

            for (; ; )
            {
                int numBytes = await stream.ReadAsync(buffer, cancellationToken);
                int numChars = decoder.GetChars(buffer.Span.Slice(0, numBytes), chars.Span, numBytes == 0);
                str.Append(chars.Span.Slice(0, numChars));

                if (numBytes == 0)
                    break;
            }

            return str.ToString();
        }

        /// <summary>
        /// Reads the element's contents and interprets them as ASCII string.
        /// </summary>
        /// <returns></returns>
        public static string ReadString(this Stream stream)
        {
            return stream.decodeString(Encoding.ASCII);
        }


        /// <summary>
        /// Reads the element's contents and interprets them as ASCII string.
        /// </summary>
        /// <returns></returns>
        public static ValueTask<string> ReadStringAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            return stream.decodeStringAsync(Encoding.ASCII, cancellationToken);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as UTF-8 string.
        /// </summary>
        /// <returns></returns>
        public static string ReadUTF8(this Stream stream)
        {
            return stream.decodeString(Encoding.UTF8);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as UTF-8 string.
        /// </summary>
        /// <returns></returns>
        public static ValueTask<string> ReadUTF8Async(this Stream stream, CancellationToken cancellationToken = default)
        {
            return stream.decodeStringAsync(Encoding.UTF8, cancellationToken);
        }

        private static readonly DateTime zeroDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Reads the element's contents and interprets them as date.
        /// </summary>
        /// <returns></returns>
        public static DateTime ReadDate(this Stream stream)
        {
            long value = stream.ReadSignedInteger();
            return zeroDate + new TimeSpan(value / 100);
        }

        /// <summary>
        /// Reads the element's contents and interprets them as date.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<DateTime> ReadDateAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            long value = await stream.ReadSignedIntegerAsync(cancellationToken);
            return zeroDate + new TimeSpan(value / 100);
        }
    }
}
