using System;
using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Audio : AbstractBase<Audio>
    {
        private static readonly IDictionary<ElementDescriptor, Action<Audio, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Audio, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.SamplingFrequencyDescriptor , (_, r) => { _.SamplingFrequency = r.ReadFloat(); } },
            { MatroskaSpecification.ChannelsDescriptor, (_, r) => { _.Channels = r.ReadUInt(); } },
            { MatroskaSpecification.BitDepthDescriptor, (_, r) => { _.BitDepth = r.ReadUInt(); } }
        };

        public double SamplingFrequency { get; private set; }
        public ulong Channels { get; private set; }
        public ulong BitDepth { get; private set; }

        public static Audio Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}