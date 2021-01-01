using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using Matroska.Attributes;
using Matroska.Extensions;
using NEbml.Core;

namespace Matroska.Spec
{
    public abstract class AbstractBase<T> where T : new()
    {
        private static readonly ObjectCache cache = MemoryCache.Default;

        public static T Read(NEbml.Core.EbmlReader reader, IDictionary<ElementDescriptor, Action<T, NEbml.Core.EbmlReader>> mapping)
        {
            var instance = new T();

            reader.EnterContainer();

            while (reader.ReadNext())
            {
                var el = MatroskaSpecification.ElementDescriptors.GetValueOrDefault(reader.ElementId);
                if (el != default && el.Type != ElementType.Binary)
                {
                    Console.WriteLine(reader.GetName());
                }

                mapping.Invoke(instance, reader);
            }

            reader.LeaveContainer();

            return instance;
        }

        public static T ParseProperties(NEbml.Core.EbmlReader reader)
        {
            var pa = cache.AddOrGetExisting(typeof(T).FullName, () =>
            {
                return typeof(T).GetProperties().Select(property => new
                {
                    Property = property,
                    property.GetCustomAttributes(false).OfType<ElementDescriptorAttribute>().First().Identifier
                }).ToDictionary(key => key.Identifier, value => value.Property);
            });

            var instance = new T();

            reader.EnterContainer();

            while (reader.ReadNext())
            {
                var el = MatroskaSpecification.ElementDescriptors.GetValueOrDefault(reader.ElementId);
                if (el != default && el.Type != ElementType.Binary)
                {
                    Console.WriteLine(reader.GetName());
                }

                PropertyInfo pi;
                if (pa.TryGetValue(el.Identifier.EncodedValue, out pi))
                {
                    object value = null;
                    switch (MatroskaSpecification.ElementIdentifiers[el.Identifier.EncodedValue].Type)
                    {
                        case ElementType.AsciiString:
                            value = reader.ReadAscii();
                            break;
                        //case ElementType.Binary:
                        //    dump = "binary data";
                        //    break;
                        case ElementType.Date:
                            value = reader.ReadDate();
                            break;
                        case ElementType.Float:
                            value = reader.ReadFloat();
                            break;
                        case ElementType.SignedInteger:
                            value = reader.ReadInt();
                            break;
                        case ElementType.UnsignedInteger:
                            value = reader.ReadUInt();
                            break;
                        case ElementType.Utf8String:
                            value = reader.ReadUtf();
                            break;
                    }

                    if (value != null)
                    {
                        pi.SetValue(instance, value);
                    }
                }
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
}