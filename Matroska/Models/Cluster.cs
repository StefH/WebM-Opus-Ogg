using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Matroska.Attributes;
using NEbml.Core;

namespace Matroska.Models
{
    public sealed class Cluster
    {
        private static readonly IDictionary<ElementDescriptor, Action<Cluster, EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Cluster, EbmlReader>>
        {
            { MatroskaSpecification.SimpleBlockDescriptor , (_, r) =>
                {
                    if (_.SimpleBlocks == null)
                    {
                        _.SimpleBlocks = new List<SimpleBlock>();
                    }

                    int len = (int)r.ElementSize;

                    var buffer = ArrayPool<byte>.Shared.Rent(len);

                    r.ReadBinary(buffer, 0, len);

                    var ms = new MemoryStream(len);
                    var bytes = buffer.AsSpan().Slice(0, len).ToArray();
                    ms.Write(bytes, 0, bytes.Length);

                    ms.Position = 0;
                   // var s = Block.Parse(bytes);

                //    _.Blocks.Add(s);

                    //if (_.SimpleBlocks.Sum(c => c.Data.Length) >= 4210)
                    //{
                    //    int cccc = 0;
                    //}
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

        [MatroskaElementDescriptor(MatroskaSpecification.Timecode)]
        public ulong Timecode { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SimpleBlock)]
        public List<SimpleBlock>? SimpleBlocks { get; set; }

        //public static Cluster Read(NEbml.Core.EbmlReader reader)
        //{
        //    var cluster = Read(reader, Mapping);

        //    //if (cluster._simpleBlockMemoryStream.Length > 0)
        //    //{
        //    //    cluster._simpleBlockMemoryStream.Position = 0;
        //    //    cluster.SimpleBlock = cluster._simpleBlockMemoryStream.GetBuffer();
        //    //    cluster._simpleBlockMemoryStream.Dispose();
        //    //}

        //    return cluster;
        //}
    }
}