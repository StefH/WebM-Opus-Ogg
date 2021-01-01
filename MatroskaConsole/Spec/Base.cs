using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using NEbml.Core;

namespace Matroska.Spec
{
    public abstract class AbstractBase<T> where T : new()
    {
        public static T Read(NEbml.Core.EbmlReader reader, IDictionary<ElementDescriptor, Action<T, NEbml.Core.EbmlReader>> mapping)
        {
            var instance = new T();

            reader.EnterContainer();

            while (reader.ReadNext())
            {
                mapping.Invoke(instance, reader);
            }

            reader.LeaveContainer();

            return instance;
        }

        protected static void WriteToStream(NEbml.Core.EbmlReader r, Stream stream)
        {
            int len = (int)r.ElementSize;

            var buffer = ArrayPool<byte>.Shared.Rent(len);

            r.ReadBinary(buffer, 0, len);

            stream.Write(buffer, 0, len);
        }
    }

    //public abstract class Base<T> where T : new()
    //{
    //    // protected static IDictionary<ElementDescriptor, Action<T, NEbml.Core.EbmlReader>> Mapping { get; set; }

    //    public static T Parse(NEbml.Core.EbmlReader reader)
    //    {


    //        return instance;
    //    }


    //}
}