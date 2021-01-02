using System;
using System.Collections.Generic;
using Matroska.Attributes;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Segment
    {
        //private static readonly IDictionary<ElementDescriptor, Action<Segment, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Segment, NEbml.Core.EbmlReader>>
        //{
        //    //{ MatroskaSpecification.InfoDescriptor, (_, r) => { _.Info = Info.ParseProperties(r); } },
        //    { MatroskaSpecification.TracksDescriptor, (_, r) => { _.Tracks = Tracks.Read(r); } },
        //    { MatroskaSpecification.ClusterDescriptor, (_, r) =>
        //        {
        //            if (_.Clusters == null)
        //            {
        //                _.Clusters = new List<Cluster>();
        //            }
        //            _.Clusters.Add(Cluster.Read(r));
        //        }
        //    }
        //};

        [MatroskaElementDescriptor(MatroskaSpecification.Info, typeof(Info))]
        public Info? Info { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Tracks, typeof(Tracks))]
        public Tracks? Tracks { get; set; }

        public List<Cluster>? Clusters { get; set; }
    }
}