using System;
using System.Buffers;
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
            { MatroskaSpecification.TrackType, (_, r) => { _.TrackType = r.ReadUInt(); } },
            { MatroskaSpecification.Name, (_, r) => { _.Name = r.ReadUtf(); } },
            { MatroskaSpecification.Language, (_, r) => { _.Language = r.ReadAscii(); } },
            { MatroskaSpecification.CodecID, (_, r) => { _.CodecID = r.ReadAscii(); } },
            { MatroskaSpecification.CodecPrivate, (_, r) =>
                {
                    int len = (int)r.ElementSize;
                    var buffer = ArrayPool<byte>.Shared.Rent(len);

                    r.ReadBinary(buffer, 0, len);
                    _.CodecPrivate = buffer.AsSpan().Slice(0, len).ToArray();

                    int xxxx = 9;
                }
            },
            { MatroskaSpecification.CodecName, (_, r) => { _.CodecName = r.ReadUtf(); } },

            { MatroskaSpecification.Audio, (_, r) => { _.Audio = Audio.Read(r); } }
        };

        public ulong TrackNumber { get; private set; }

        public ulong TrackUID { get; private set; }

        public ulong TrackType { get; private set; }

        public string Name { get; private set; }

        public string Language { get; private set; }

        public string CodecID { get; private set; }

        public byte[] CodecPrivate { get; private set; }

        public string CodecName { get; private set; }

        /*
       

|        |            | Language     |
|        |            |--------------|
|        |            | CodecID      |
|        |            |--------------|
|        |            | CodecPrivate |
|        |            |--------------|
|        |            | CodecName   */


        public Audio Audio { get; private set; }

        

        public static TrackEntry Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}