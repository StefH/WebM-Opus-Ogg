using System;
using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Spec
{
    public class Tracks : AbstractBase<Tracks>
    {
        private static readonly IDictionary<ElementDescriptor, Action<Tracks, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Tracks, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.TrackEntryDescriptor, (_, r) => { _.TrackEntry = TrackEntry.Read(r); } }
        };

        public TrackEntry TrackEntry { get; private set; }

        public static Tracks Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}
