using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Matroska
{
    /// <summary>
    /// Implements a low-level EBML reader.
    /// </summary>
    public class EbmlReader
    {
        #region Fields

        private readonly Stack<StackItem> stack;
        private StackItem current;
        private Decoder utf8Decoder;
        private long position;

        #endregion

        #region Properties

        public int Depth => stack.Count;

        private StackItem top
        {
            get
            {
                if (stack.TryPeek(out StackItem item))
                    return item;
                return default;
            }
        }

        public long Consumed
        {
            get
            {
                if (current.IsNull)
                    return 0;

                return current.Consumed;
            }
        }

        public long Remaining
        {
            get
            {
                if (current.IsNull)
                    return 0;

                return current.Remaining;
            }
        }

        public bool EndOfParent
        {
            get
            {
                StackItem parent = top;

                if (parent.IsNull)
                    return false;

                return parent.Remaining == 0;
            }
        }

        #endregion

        #region Constructors

        public EbmlReader()
        {
            stack = new Stack<StackItem>();
        }

        #endregion

        #region Methods

        public void Advance(ref SequenceReader<byte> reader, long count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (!current.IsNull && position + count > current.End)
                throw new InvalidOperationException("Can't advance beyond the end of the element.");

            reader.Advance(count);
            position += count;
        }

        private bool SkipCurrent(ref SequenceReader<byte> reader)
        {
            if (!current.IsNull && current.Remaining > 0)
            {
                long count = Math.Min(reader.Remaining, current.Remaining);
                Advance(ref reader, count);

                if (current.Remaining > 0)
                    return false;
            }

            current = default;
            return true;
        }

        public int GetHeader(in SequenceReader<byte> reader, out EbmlHeader header)
        {
            SequenceReader<byte> temp = reader;

            if (!temp.TryReadEbmlHeader(out header))
                return 0;

            StackItem parent = top;
            if (!parent.IsNull)
            {
                long headerSize = temp.Consumed - reader.Consumed;
                if (parent.Remaining < headerSize)
                    return 0;
            }

            return (int)(temp.Consumed - reader.Consumed);
        }

        public int GetBinary(in SequenceReader<byte> reader, Span<byte> dest)
        {
            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            int numBytes = (int)Math.Min(dest.Length, current.Remaining);
            numBytes = Math.Min(numBytes, (int)reader.Remaining);

            if (!reader.TryCopyTo(dest[..numBytes]))
                return 0;

            return numBytes;
        }

        public int GetString(in SequenceReader<byte> reader, Span<char> dest)
        {
            Span<byte> bytes = MemoryMarshal.AsBytes(dest).Slice(0, dest.Length);
            int numBytes = GetBinary(reader, bytes);
            for (int i = dest.Length - 1; i >= 0; --i)
            {
                dest[i] = (char)bytes[i];
            }

            return numBytes;
        }

        public int GetUtf8(in SequenceReader<byte> reader, Span<char> dest, out int charCount)
        {
            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            utf8Decoder ??= Encoding.UTF8.GetDecoder();

            int totalBytes = 0;
            charCount = 0;

            SequenceReader<byte> temp = reader;
            Span<byte> buf = stackalloc byte[Math.Min(4096, dest.Length)];
            long remaining = current.Remaining;

            while (dest.Length > 0)
            {
                int numBytes = (int)Math.Min(buf.Length, remaining);
                numBytes = Math.Min(numBytes, (int)temp.Remaining);
                if (!temp.TryCopyTo(buf[..numBytes]))
                    break;

                utf8Decoder.Convert(buf[..numBytes], dest, false, out numBytes, out int numChars, out bool completed);
                if (numChars == 0)
                    break;

                dest = dest[numChars..];

                totalBytes += numBytes;
                charCount += numChars;
                remaining -= numBytes;
                temp.Advance(numBytes);
            }

            return totalBytes;
        }

        public bool GetSignedInteger(in SequenceReader<byte> reader, out long value)
        {
            value = 0;

            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            if (current.Remaining <= 0 || current.Remaining > 8)
                throw new InvalidOperationException("Current element does not contain an integer.");

            Span<byte> contents = stackalloc byte[(int)current.Remaining];
            if (!reader.TryCopyTo(contents))
                return false;

            value = Ebml.DecodeSInt(contents);
            return true;
        }

        public bool ReadSignedInteger(ref SequenceReader<byte> reader, out long value)
        {
            if (!GetSignedInteger(reader, out value))
                return false;

            Advance(ref reader, current.Remaining);
            return true;
        }

        public bool GetUnsignedInteger(in SequenceReader<byte> reader, out ulong value)
        {
            value = 0;

            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            if (current.Remaining <= 0 || current.Remaining > 8)
                throw new InvalidOperationException("Current element does not contain an integer.");

            Span<byte> contents = stackalloc byte[(int)current.Remaining];
            if (!reader.TryCopyTo(contents))
                return false;

            value = Ebml.DecodeUInt(contents);
            return true;
        }

        public bool ReadUnsignedInteger(ref SequenceReader<byte> reader, out ulong value)
        {
            if (!GetUnsignedInteger(reader, out value))
                return false;

            Advance(ref reader, current.Remaining);
            return true;
        }

        public bool GetFloat(in SequenceReader<byte> reader, out float value)
        {
            value = 0;

            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            if (current.Remaining != 4 && current.Remaining != 8)
                throw new InvalidOperationException("Current element does not contain a float.");

            Span<byte> contents = stackalloc byte[(int)current.Remaining];
            if (!reader.TryCopyTo(contents))
                return false;

            if (current.Remaining == 4)
                value = BitConverter.ToSingle(contents);
            else
                value = (float)BitConverter.ToDouble(contents);

            return true;
        }

        public bool ReadFloat(ref SequenceReader<byte> reader, out float value)
        {
            if (!GetFloat(reader, out value))
                return false;

            Advance(ref reader, current.Remaining);
            return true;
        }

        public bool GetFloat(in SequenceReader<byte> reader, out double value)
        {
            value = 0;

            if (current.IsNull)
                throw new InvalidOperationException("No element has been read.");

            if (current.Remaining != 4 && current.Remaining != 8)
                throw new InvalidOperationException("Current element does not contain a float.");

            Span<byte> contents = stackalloc byte[(int)current.Remaining];
            if (!reader.TryCopyTo(contents))
                return false;

            if (current.Remaining == 4)
                value = BitConverter.ToSingle(contents);
            else
                value = BitConverter.ToDouble(contents);

            return true;
        }

        public bool ReadFloat(ref SequenceReader<byte> reader, out double value)
        {
            if (!GetFloat(reader, out value))
                return false;

            Advance(ref reader, current.Remaining);
            return true;
        }

        public bool GetDate(in SequenceReader<byte> reader, out DateTime value)
        {
            value = default;

            if (!GetSignedInteger(reader, out long integer))
                return false;

            value = Ebml.DecodeDate(integer);
            return true;
        }

        public bool ReadDate(ref SequenceReader<byte> reader, out DateTime value)
        {
            if (!GetDate(reader, out value))
                return false;

            Advance(ref reader, current.Remaining);
            return true;
        }

        public bool AdvanceToNextElement(ref SequenceReader<byte> reader, out EbmlHeader header)
        {
            header = default;
            if (!SkipCurrent(ref reader))
                return false;

            StackItem parent = top;
            if (!parent.IsNull && parent.Remaining == 0)
                return false;

            int headerSize = GetHeader(reader, out header);
            if (headerSize == 0)
                return false;

            Advance(ref reader, headerSize);

            current = new StackItem(this, header);
            return true;
        }

        public void MoveToChildren()
        {
            if (current.IsNull)
                throw new InvalidOperationException();

            stack.Push(current);
            current = default;
        }

        public bool MoveToParent(ref SequenceReader<byte> reader)
        {
            StackItem parent = stack.Pop();

            if (parent.Remaining > 0)
            {
                long count = Math.Min(reader.Remaining, current.Remaining);
                Advance(ref reader, count);

                if (parent.Remaining > 0)
                {
                    stack.Push(parent);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Nested Types

        private readonly struct StackItem
        {
            private readonly EbmlReader owner;
            public readonly EbmlHeader Header;
            public readonly long Start;
            public long End => Start + Header.Length;
            public long Consumed => owner.position - Start;
            public long Remaining => End - owner.position;
            public bool IsNull => owner == null;

            public StackItem(EbmlReader owner, EbmlHeader header)
            {
                this.owner = owner;
                Header = header;
                Start = owner.position;
            }

            public override string ToString()
            {
                return $"{Header}, {Consumed} consumed";
            }
        }
        #endregion
    }
}