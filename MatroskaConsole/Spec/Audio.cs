using Matroska.Attributes;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Audio : AbstractBase<Audio>
    {
        [ElementDescriptor(MatroskaSpecification.SamplingFrequency)]
        public double SamplingFrequency { get; private set; }

        [ElementDescriptor(MatroskaSpecification.Channels)]
        public ulong Channels { get; private set; }

        [ElementDescriptor(MatroskaSpecification.BitDepth)]
        public ulong BitDepth { get; private set; }
    }
}