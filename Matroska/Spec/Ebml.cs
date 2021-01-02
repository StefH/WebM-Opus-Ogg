using Matroska.Attributes;

namespace Matroska.Spec
{
    public class Ebml
    {
		[MatroskaElementDescriptor(MatroskaSpecification.EBMLVersion)]
		public ulong EBMLVersion { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.EBMLReadVersion)]
		public ulong EBMLReadVersion { get; set; }

	}
}

/*
 * EBMLVersion = Uint(0x4286, nameof(EBMLVersion)),
				EBMLReadVersion = Uint(0x42f7, nameof(EBMLReadVersion)),
				EBMLMaxIDLength = Uint(0x42f2, nameof(EBMLMaxIDLength)),
				EBMLMaxSizeLength = Uint(0x42f3, nameof(EBMLMaxSizeLength)),
				DocType = Ascii(0x4282, nameof(DocType)),
				DocTypeVersion = Uint(0x4287, nameof(DocTypeVersion)),
				DocTypeReadVersion = Uint(0x4285, nameof(DocTypeReadVersion));*/
