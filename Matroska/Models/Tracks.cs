using Matroska.Attributes;

namespace Matroska.Models
{
    public class Tracks
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackEntry)]
        public TrackEntry? TrackEntry { get; set; }
    }
}