namespace Matroska
{
	/// <summary>
	/// Defines the EBML element types.
	/// </summary>
	public enum EbmlElementType
	{
		/// <summary>
		/// Signed integer. Big-endian, any size from 1 to 8 octets.
		/// </summary>
		SignedInteger,

		/// <summary>
		/// Unsigned integer. Big-endian, any size from 1 to 8 octets
		/// </summary>
		UnsignedInteger,

		/// <summary>
		/// Floating point number. Big-endian, defined for 4 and 8 octets (32, 64 bits).
		/// </summary>
		Float,

		/// <summary>
		/// Printable ASCII (0x20 to 0x7E), zero-padded when needed.
		/// </summary>
		String,

		/// <summary>
		/// Unicode string, zero padded when needed (RFC 2279)
		/// </summary>
		UTF8,

		/// <summary>
		/// Signed 8 octets integer in nanoseconds with 0 indicating the precise beginning of the millennium (at 2001-01-01T00:00:00,000000000 UTC)
		/// </summary>
		Date,

		/// <summary>
		/// Contains other EBML sub-elements of the next lower level
		/// </summary>
		Master,

		/// <summary>
		/// Binary data. Not interpreted by the parser.
		/// </summary>
		Binary,
	}
}