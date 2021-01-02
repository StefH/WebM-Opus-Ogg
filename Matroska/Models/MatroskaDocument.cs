namespace Matroska.Models
{
    public class MatroskaDocument
    {
        public Ebml? Ebml { get; set; }

        public Segment? Segment { get; set; }
    }
}