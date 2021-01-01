using System;
using Matroska.Attributes;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Info : AbstractBase<Info>
    {
        [ElementDescriptor(MatroskaSpecification.TimestampScale)]
        public ulong TimestampScale { get; private set; }

        [ElementDescriptor(MatroskaSpecification.Duration)]
        public double Duration { get; private set; }

        [ElementDescriptor(MatroskaSpecification.DateUTC)]
        public DateTime DateUTC { get; private set; }

        [ElementDescriptor(MatroskaSpecification.Title)]
        public string Title { get; private set; }

        [ElementDescriptor(MatroskaSpecification.MuxingApp)]
        public string MuxingApp { get; private set; }

        [ElementDescriptor(MatroskaSpecification.WritingApp)]
        public string WritingApp { get; private set; }
    }
}