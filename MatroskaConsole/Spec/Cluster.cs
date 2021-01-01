using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Cluster : AbstractBase<Cluster>
    {
        private static readonly IDictionary<ElementDescriptor, Action<Cluster, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Cluster, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.TimecodeDescriptor, (_, r) => { _.Timecode = r.ReadUInt(); } },
            { MatroskaSpecification.SimpleBlockDescriptor , (_, r) =>
                {
                    if (_.SimpleBlocks == null)
                    {
                        _.SimpleBlocks = new List<Block>();
                    }

                    int len = (int)r.ElementSize;

                    var buffer = ArrayPool<byte>.Shared.Rent(len);

                    r.ReadBinary(buffer, 0, len);

                    var ms = new MemoryStream(len);
                    var bytes = buffer.AsSpan().Slice(0, len);
                    ms.Write(bytes);

                    ms.Position = 0;
                    var s = Block.Parse(ms);

                    _.SimpleBlocks.Add(s);

                    if (_.SimpleBlocks.Sum(c => c.Data.Length) >= 4210)
                    {
                        int cccc = 0;
                    }
                    /*
                     * public ulong TrackNumber { get; set; }

        public int NumFrames { get; set; }

        public short TimeCode { get; set; }

        public byte[] Data { get; set; }
                     */



                    //WriteToStream(r, _._simpleBlockMemoryStream);
                }
            }
        };

        //private readonly MemoryStream _simpleBlockMemoryStream = new MemoryStream();

        public ulong Timecode { get; private set; }
        public List<Block> SimpleBlocks { get; private set; }

        public static Cluster Read(NEbml.Core.EbmlReader reader)
        {
            var cluster = Read(reader, Mapping);

            //if (cluster._simpleBlockMemoryStream.Length > 0)
            //{
            //    cluster._simpleBlockMemoryStream.Position = 0;
            //    cluster.SimpleBlock = cluster._simpleBlockMemoryStream.GetBuffer();
            //    cluster._simpleBlockMemoryStream.Dispose();
            //}

            return cluster;
        }
    }
}

/*



 * PrintValue("track number", simple_block.track_number);
    PrintValue("frames", simple_block.num_frames);
    PrintValue("timecode", simple_block.timecode);
    PrintValue("lacing", simple_block.lacing);*/