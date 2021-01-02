using Matroska.Attributes;

namespace Matroska.Spec
{
    public class Tracks
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackEntry, typeof(TrackEntry))]
        public TrackEntry? TrackEntry { get; set; }
    }
}