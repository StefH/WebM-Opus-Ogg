using System;
using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class TrackEntry : AbstractBase<TrackEntry>
    {
        private static readonly IDictionary<ElementDescriptor, Action<TrackEntry, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<TrackEntry, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.TrackNumber, (_, r) => { _.TrackNumber = r.ReadUInt(); } },
            { MatroskaSpecification.TrackUID, (_, r) => { _.TrackUID = r.ReadUInt(); } },

            { MatroskaSpecification.Audio, (_, r) => { _.Audio = Audio.Read(r); } }
        };

        public ulong TrackNumber { get; private set; }

        public ulong TrackUID { get; private set; }

        public Audio Audio { get; private set; }

        public static TrackEntry Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}