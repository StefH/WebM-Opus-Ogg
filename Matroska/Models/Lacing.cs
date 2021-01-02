using System;

namespace Matroska.Models
{
    [Flags]
    public enum Lacing : byte
    {
        No = 0x0,
        Xiph = 0x2,
        EBML = 0x6,
        FixedSize = 0x4,

        Any = 0x6
    }
}