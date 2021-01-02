using Matroska.Attributes;

namespace Matroska.Models
{
    public class Tracks
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackEntry, typeof(TrackEntry))]
        public TrackEntry? TrackEntry { get; set; }
    }
}