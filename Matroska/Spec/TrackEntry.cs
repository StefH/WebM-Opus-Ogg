using System;
using System.Buffers;
using System.Collections.Generic;
using Matroska.Attributes;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class TrackEntry
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackNumber)]
        public ulong TrackNumber { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackUID)]
        public ulong TrackUID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackType)]
        public TrackType TrackType { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Name)]
        public string? Name { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Language)]
        public string? Language { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecID)]
        public string? CodecID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecPrivate)]
        public byte[]? CodecPrivate { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecName)]
        public string? CodecName { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Audio)]
        public Audio? Audio { get; set; }
    }
}