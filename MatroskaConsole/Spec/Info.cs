using System;
using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Info : AbstractBase<Info>
    {
        private static readonly IDictionary<ElementDescriptor, Action<Info, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Info, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.TimecodeScale, (_, r) => { _.TimecodeScale = r.ReadUInt(); } },
            { MatroskaSpecification.Duration, (_, r) => { _.Duration = r.ReadFloat(); } },
            { MatroskaSpecification.DateUTC, (_, r) => { _.DateUTC = r.ReadDate(); } },
            { MatroskaSpecification.Title, (_, r) => { _.Title = r.ReadUtf(); } },
            { MatroskaSpecification.MuxingApp, (_, r) => { _.MuxingApp = r.ReadUtf(); } },
            { MatroskaSpecification.WritingApp, (_, r) => { _.WritingApp = r.ReadUtf(); } }
        };

        public ulong TimecodeScale { get; private set; }
        public double Duration { get; private set; }
        public DateTime DateUTC { get; private set; }
        public string Title { get; private set; }
        public string MuxingApp { get; private set; }
        public string WritingApp { get; private set; }

        public static Info Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}