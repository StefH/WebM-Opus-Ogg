namespace Matroska
{
	/// <summary>
	/// Represents the header of an EBML element.
	/// </summary>
	public readonly struct EbmlHeader
	{
		/// <summary>
		/// The ID of the element.
		/// </summary>
		public readonly uint ID;

		/// <summary>
		/// The length of the element.
		/// </summary>
		public readonly long Length;

		public EbmlHeader(uint id, long length)
		{
			ID = id;
			Length = length;
		}

		public override string ToString()
		{
			return $"0x{ID:X8}, {Length} bytes";
		}
	}
}