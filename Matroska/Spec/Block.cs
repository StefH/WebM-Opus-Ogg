using System;
using System.Buffers;
using System.IO;

namespace Matroska.Spec
{
    /*
     * 5-6	must	Lacing
    421
    000 : no lacing = 
    010 : Xiph lacing = 
    110 : EBML lacing = 
    100 : fixed-size lacing = 
     */

    [Flags]
    public enum Lacing : byte
    {
        No = 0x0,
        Xiph = 0x2,
        EBML = 0x6,
        FixedSize = 0x4,
        Any = 0x6
    }

    public class Block
    {
        /// <summary>
        /// Track Number (Track Entry)
        /// </summary>
        public ulong TrackNumber { get; private set; }

        /// <summary>
        /// Number of frames in the lace-1 (uint8)
        /// </summary>
        public int NumFrames { get; private set; }

        /// <summary>
        /// Timecode (relative to Cluster timecode, signed int16)
        /// </summary>
        public short TimeCode { get; private set; }

        /// <summary>
        /// Keyframe, set when the Block contains only keyframes
        /// </summary>
        public bool IsKeyFrame { get; private set; }

        /// <summary>
        /// Discardable, the frames of the Block can be discarded during playing if needed
        /// </summary>
        public bool IsDiscardable { get; private set; }

        /// <summary>
        /// Invisible, the codec should decode this frame but not display it
        /// </summary>
        public bool IsInvisible { get; private set; }

        /// <summary>
        /// Lacing
        /// </summary>
        public Lacing Lacing { get; private set; }

        public byte[]? Data { get; private set; }

        //static bool is_bit_set(byte value, int bitindex)
        //{
        //    return (value & (1 << bitindex)) != 0;
        //}

        public static Block Parse(MemoryStream stream)
        {
            if (stream.Length > 7)
            {
                int y = 0;
            }

            using var bn = new BinaryReader(stream);

            //VInt trackNumberAsVInt; // = Ebml.ReadVInt()

            //Ebml.TryReadEbmlVInt(Span<byte> reader, int maxLength, out VInt vint)

            var buf = ArrayPool<byte>.Shared.Rent(8);
            var trackNumberAsVInt = NEbml.Core.VInt.Read(stream, 8, buf);

            //stream.Position += trackNumberAsVInt.Length;

            var timeCode = bn.ReadInt16();
            var flags = bn.ReadByte();

            var lacing = (Lacing)(flags & (byte)Lacing.Any);
            var numFrames = 0;
            int laceCodedSizeOfEachFrame = 0;
            if (lacing != Lacing.No)
            {
                numFrames = bn.ReadByte();

                var fixed_sizeLacing = (flags & 0x04) > 0;
                if (lacing != Lacing.FixedSize)
                {
                    laceCodedSizeOfEachFrame = bn.ReadByte();
                }
            }

            /*
            5-6	must	Lacing

    00 : no lacing
    01 : Xiph lacing
    11 : EBML lacing
    10 : fixed-size lacing


             * if (name === 'SimpleBlock') {
        keyframe = Boolean(data[length + 2] & 0x80);
        discardable = Boolean(data[length + 2] & 0x01);
      }
             */

            var pos = (int)stream.Position;

            return new Block
            {
                TrackNumber = trackNumberAsVInt.Value,
                TimeCode = timeCode,
                NumFrames = numFrames,
                IsKeyFrame = (flags & 0x80) > 0,
                IsDiscardable = (flags & 0x01) > 0,
                IsInvisible = (flags & 0x08) > 0,
                Data = stream.ToArray().AsSpan().Slice(pos).ToArray()
            };
        }
    }
}