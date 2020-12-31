using System.Buffers;

namespace Matroska
{
    public enum SimpleBlockLacing
	{
		None,
		Xiph,
		Ebml,
		FixedSize
	}

	public struct SimpleBlock
	{
		public int TrackNumber;
		public int TimeCode;
		public bool KeyFrame;
		public bool Invisible;
		public SimpleBlockLacing Lacing;
		public bool Discardable;

		public static bool TryParse(ref SequenceReader<byte> data, out SimpleBlock block)
		{
			block = default;
			long start = data.Consumed;
			bool done = false;
			try
			{
				if (!data.TryReadEbmlVInt(4, out VInt v))
					return false;

				block.TrackNumber = unchecked((int)v.Value);

				if (!data.TryReadBigEndian(out short tc))
					return false;

				block.TimeCode = tc;
				if (!data.TryRead(out byte b))
					return false;

				block.KeyFrame = (b & 0x80) != 0;
				block.Invisible = (b & 0x08) != 0;
				block.Lacing = (SimpleBlockLacing)((b & 0x06) >> 1);
				block.Discardable = (b & 0x01) != 0;
				done = true;
				return true;
			}
			finally
			{
				if (!done)
				{
					data.Rewind(data.Consumed - start);
				}
			}
		}
	}
}