using System;
using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Spec
{
    public sealed class Segment : AbstractBase<Segment>
    {
        private static readonly IDictionary<ElementDescriptor, Action<Segment, NEbml.Core.EbmlReader>> Mapping = new Dictionary<ElementDescriptor, Action<Segment, NEbml.Core.EbmlReader>>
        {
            { MatroskaSpecification.InfoDescriptor, (_, r) => { _.Info = Info.Read(r); } },
            { MatroskaSpecification.TracksDescriptor, (_, r) => { _.Tracks = Tracks.Read(r); } },
            { MatroskaSpecification.ClusterDescriptor, (_, r) =>
                {
                    if (_.Clusters == null)
                    {
                        _.Clusters = new List<Cluster>();
                    }
                    _.Clusters.Add(Cluster.Read(r));
                }
            }
        };

        public Info Info { get; private set; }

        public Tracks Tracks { get; private set; }

        public List<Cluster> Clusters { get; private set; }

        public static Segment Read(NEbml.Core.EbmlReader reader)
        {
            return Read(reader, Mapping);
        }
    }
}