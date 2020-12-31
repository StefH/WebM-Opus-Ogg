using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Matroska
{
	/// <summary>
	/// Implements EBML primitives.
	/// </summary>
	public static class Ebml
	{
		/// <summary>
		/// Tries to read an EBML variable-length int from the specified <see cref="SequenceReader{T}"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="maxLength"></param>
		/// <param name="vint"></param>
		/// <returns></returns>
		public static bool TryReadEbmlVInt(this ref SequenceReader<byte> reader, int maxLength, out VInt vint)
		{
			vint = default;
			if (!reader.TryRead(out byte b1))
				return false;

			ulong raw = b1;
			uint mask = 0xFF00;

			for (int i = 0; i < maxLength; ++i)
			{
				mask >>= 1;

				if ((b1 & mask) != 0)
				{
					ulong value = raw & ~mask;

					for (int j = 0; j < i; ++j)
					{
						if (!reader.TryRead(out byte b))
						{
							reader.Rewind(j + 1);
							return false;
						}

						raw = (raw << 8) | b;
						value = (value << 8) | b;
					}

					vint = new VInt(i + 1, raw, value);
					return true;
				}
			}

			throw new InvalidDataException("Invalid variable int.");
		}

		/// <summary>
		/// Tries to read an EBML element ID from the specified <see cref="SequenceReader{T}"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool TryReadEbmlId(this ref SequenceReader<byte> reader, out uint id)
		{
			id = 0;
			if (!reader.TryReadEbmlVInt(4, out VInt v))
				return false;

			id = unchecked((uint)v.Raw);
			return true;
		}

		/// <summary>
		/// Tries to read an EBML element length from the specified <see cref="SequenceReader{T}"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static bool TryReadEbmlLength(this ref SequenceReader<byte> reader, out long length)
		{
			length = 0;
			if (!reader.TryReadEbmlVInt(8, out VInt v))
				return false;

			// Check for special "unknown size" encoding.
			if (v.Value + 1 == 1ul << (7 * v.Length))
				length = -1;
			else
				length = unchecked((long)v.Value);

			return true;
		}

		/// <summary>
		/// Tries to read an <see cref="EbmlHeader"/> from the specified <see cref="SequenceReader{T}"/>.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="header"></param>
		/// <returns></returns>
		public static bool TryReadEbmlHeader(this ref SequenceReader<byte> reader, out EbmlHeader header)
		{
			header = default;

			long start = reader.Consumed;
			
			if (!reader.TryReadEbmlId(out uint id))
				return false;

			if (!reader.TryReadEbmlLength(out long length))
			{
				reader.Rewind(reader.Consumed - start);
				return false;
			}

			header = new EbmlHeader(id, length);
			return true;
		}

		/// <summary>
		/// Decodes a signed integer as found in EBML element content.
		/// </summary>
		/// <param name="buf"></param>
		/// <returns></returns>
		public static long DecodeSInt(ReadOnlySpan<byte> buf)
		{
			bool negative = false;
			ulong value = 0;
			for (int i = 0; i < buf.Length; ++i)
			{
				value = (value << 8) | buf[i];

				if (i == 0)
					negative = (buf[i] & 0x80) != 0;
			}

			if (negative)
			{
				// Extend sign.
				for (int i = buf.Length; i < 8; ++i)
				{
					value |= 0xFFul << (i * 8);
				}
			}

			return (long)value;
		}

		/// <summary>
		/// Decodes an unsigned integer as found in EBML element content.
		/// </summary>
		/// <param name="buf"></param>
		/// <returns></returns>
		public static ulong DecodeUInt(ReadOnlySpan<byte> buf)
		{
			ulong value = 0;
			for (int i = 0; i < buf.Length; ++i)
			{
				value = (value << 8) | buf[i];
			}

			return value;
		}

		private static readonly DateTime zeroDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Decodes a date as found in EBML element content.
		/// </summary>
		/// <param name="buf"></param>
		/// <returns></returns>
		public static DateTime DecodeDate(ReadOnlySpan<byte> buf)
		{
			return DecodeDate(DecodeSInt(buf));
		}

		/// <summary>
		/// Decodes a date as found in EBML element content.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DateTime DecodeDate(long value)
		{
			return zeroDate + new TimeSpan(value / 100);
		}

		/// <summary>
		/// Tries to read an EBML VInt from the stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="maxLength">The maximum number of bytes possible.</param>
		/// <returns>The number of bytes, or 0 if an error occured.</returns>
		public static VInt ReadVInt(Stream stream, int maxLength)
		{
			Span<byte> temp = stackalloc byte[1];

			int n = stream.Read(temp);
			if (n != 1)
				return default;

			uint b1 = temp[0];
			ulong raw = b1;
			uint mask = 0xFF00;

			for (int i = 0; i < maxLength; ++i)
			{
				mask >>= 1;

				if ((b1 & mask) != 0)
				{
					ulong value = raw & ~mask;

					for (int j = 0; j < i; ++j)
					{
						n = stream.Read(temp);
						if (n != 1)
							throw new EndOfStreamException();

						byte b = temp[0];
						raw = (raw << 8) | b;
						value = (value << 8) | b;
					}

					return new VInt(i + 1, raw, value);
				}
			}

			throw new InvalidDataException("Invalid variable int.");
		}

		/// <summary>
		/// Tries to read an EBML VInt from the stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="maxLength">The maximum number of bytes possible.</param>
		/// <returns>The number of bytes, or 0 if an error occured.</returns>
		public static async ValueTask<VInt> ReadVIntAsync(Stream stream, int maxLength)
		{
			using IMemoryOwner<byte> mem = MemoryPool<byte>.Shared.Rent(1);
			Memory<byte> temp = mem.Memory.Slice(0, 1);

			int n = await stream.ReadAsync(temp);
			if (n != 1)
				return default;

			uint b1 = temp.Span[0];
			ulong raw = b1;
			uint mask = 0xFF00;

			for (int i = 0; i < maxLength; ++i)
			{
				mask >>= 1;

				if ((b1 & mask) != 0)
				{
					ulong value = raw & ~mask;

					for (int j = 0; j < i; ++j)
					{
						n = await stream.ReadAsync(temp);
						if (n != 1)
							throw new EndOfStreamException();

						byte b = temp.Span[0];
						raw = (raw << 8) | b;
						value = (value << 8) | b;
					}

					return new VInt(i + 1, raw, value);
				}
			}

			throw new InvalidDataException("Invalid variable int.");
		}

		/// <summary>
		/// Tries to read an EBML VInt from the stream.
		/// </summary>
		/// <param name="data">The stream to read from.</param>
		/// <param name="maxLength">The maximum number of bytes possible.</param>
		/// <returns>The number of bytes, or 0 if an error occured.</returns>
		public static VInt ReadVInt(ref ReadOnlySpan<byte> data, int maxLength)
		{
			if (data.Length == 0)
				return default;

			uint b1 = data[0];
			ulong raw = b1;
			uint mask = 0xFF00;

			data = data.Slice(1);

			for (int i = 0; i < maxLength; ++i)
			{
				mask >>= 1;

				if ((b1 & mask) != 0)
				{
					ulong value = raw & ~mask;

					for (int j = 0; j < i; ++j)
					{
						if (data.Length == 0)
							throw new EndOfStreamException();

						byte b = data[0];
						raw = (raw << 8) | b;
						value = (value << 8) | b;
					}

					return new VInt(i + 1, raw, value);
				}
			}

			throw new InvalidDataException("Invalid variable int.");
		}

		/// <summary>
		/// Reads an element ID from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static uint? TryReadElementID(Stream stream)
		{
			VInt v = ReadVInt(stream, 4);

			if (v.Length == 0)
				return null;

			return (uint)v.Raw;
		}

		/// <summary>
		/// Reads an element ID from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static async ValueTask<uint?> TryReadElementIDAsync(Stream stream)
		{
			VInt v = await ReadVIntAsync(stream, 4);

			if (v.Length == 0)
				return null;

			return (uint)v.Raw;
		}

		/// <summary>
		/// Reads an element length from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static long ReadElementLength(Stream stream)
		{
			VInt v = ReadVInt(stream, 8);

			if (v.Length == 0)
				throw new EndOfStreamException();

			// Check for special "unknown size" encoding.
			if (v.Value + 1 == 1ul << (7 * v.Length))
				return -1;

			return (long)v.Value;
		}

		/// <summary>
		/// Reads an element length from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static async ValueTask<long> ReadElementLengthAsync(Stream stream)
		{
			VInt v = await ReadVIntAsync(stream, 8);

			if (v.Length == 0)
				throw new EndOfStreamException();

			// Check for special "unknown size" encoding.
			if (v.Value + 1 == 1ul << (7 * v.Length))
				return -1;

			return (long)v.Value;
		}

		/// <summary>
		/// Writes a VInt to the specified span.
		/// </summary>
		/// <returns>The number of bytes written.</returns>
		public static int WriteVInt(Span<byte> buf, ulong value)
		{
			int pos = 0;
			int size = VInt.GetSize(value);

			value |= 1UL << (7 * size);
			for (int i = size - 1; i >= 0; --i)
			{
				buf[pos++] = (byte)(value >> (8 * i));
			}

			return pos;
		}

		/// <summary>
		/// Writes an unknown size to the specified span.
		/// </summary>
		/// <returns>The number of bytes written.</returns>
		public static int WriteUnknownLength(Span<byte> buf, int numBytes)
		{
			int pos = 0;

			buf[pos++] = (byte)(0x1FF >> numBytes);
			while (--numBytes > 0)
				buf[pos++] = 0xFF;

			return pos;
		}

		/// <summary>
		/// Writes the length of an element to the span.
		/// </summary>
		/// <returns>The number of bytes written.</returns>
		public static int WriteElementLength(Span<byte> buf, long length)
		{
			if (length == EbmlElement.UnknownLength)
				return WriteUnknownLength(buf, 1);

			return WriteVInt(buf, (ulong)length);
		}

		/// <summary>
		/// Writes the ID of an element to the span.
		/// </summary>
		/// <returns>The number of bytes written.</returns>
		public static int WriteElementId(Span<byte> buf, uint id)
		{
			if ((id & 0xFF000000u) != 0)
			{
				buf[0] = (byte)(id >> 24);
				buf[1] = (byte)(id >> 16);
				buf[2] = (byte)(id >> 8);
				buf[3] = (byte)(id);
				return 4;
			}

			if ((id & 0x00FF0000u) != 0)
			{
				buf[0] = (byte)(id >> 16);
				buf[1] = (byte)(id >> 8);
				buf[2] = (byte)(id);
				return 3;
			}

			if ((id & 0x0000FF00u) != 0)
			{
				buf[0] = (byte)(id >> 8);
				buf[1] = (byte)(id);
				return 2;
			}

			if ((id & 0x00000080u) != 0)
			{
				buf[0] = (byte)(id);
				return 1;
			}

			throw new ArgumentException("Invalid ID.");
		}

		/// <summary>
		/// Writes an EBML element header to a span.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="id"></param>
		/// <param name="length"></param>
		/// <returns>The number of bytes written.</returns>
		public static int WriteHeader(Span<byte> buf, uint id, long length)
		{
			int pos = WriteElementId(buf, id);
			pos += WriteElementLength(buf.Slice(pos), length);
			return pos;
		}
	}
}