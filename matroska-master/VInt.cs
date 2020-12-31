using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Matroska
{
	/// <summary>
	/// Represents a Matroska variable-length integer.
	/// </summary>
	public readonly struct VInt
	{
		public readonly int Length;
		public readonly ulong Raw;
		public readonly ulong Value;

		public VInt(int length, ulong raw, ulong value)
		{
			Length = length;
			Raw = raw;
			Value = value;
		}

		/// <summary>
		/// Returns the length of the VInt encoding for the specified value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int GetSize(ulong value)
		{
			int octets = 1;
			while ((value + 1) >> octets * 7 != 0)
				++octets;
			return octets;
		}
	}
}
