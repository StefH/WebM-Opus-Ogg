using System;

namespace Matroska
{
	/// <summary>
	/// Defines the EBML element flags.
	/// </summary>
	[Flags]
	public enum EbmlElementFlags
	{
		/// <summary>
		/// No flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// The element is mandatory.
		/// </summary>
		Mandatory = 1,

		/// <summary>
		/// Multiple elements of this type can occur.
		/// </summary>
		Multiple = 2
	}
}