using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using Matroska.Attributes;
using Matroska.Extensions;
using Matroska.Models;
using NEbml.Core;

namespace Matroska
{
    public static class MatroskaSerializer
    {
        private static readonly ObjectCache cache = MemoryCache.Default;

        public static MatroskaDocument Deserialize(Stream stream)
        {
            var reader = new EbmlReader(stream);

            reader.ReadNext();
            var ebml = Deserialize<Ebml>(reader);

            reader.ReadNext();
            var segment = Deserialize<Segment>(reader);

            return new MatroskaDocument
            {
                Ebml = ebml,
                Segment = segment
            };
        }

        public static T Deserialize<T>(EbmlReader reader) where T : class
        {
            return (T)Deserialize(typeof(T), reader);
        }

        public static object Deserialize(Type type, EbmlReader reader)
        {
            reader.EnterContainer();

            var instance = Activator.CreateInstance(type);

            while (reader.ReadNext())
            {
                if (TryGet(type, reader.ElementId.EncodedValue, out var info))
                {
                    SetPropertyValue(instance, info, reader);
                }
            }

            reader.LeaveContainer();

            return instance;
        }

        private static void SetPropertyValue(object instance, MatroskaElementInfo info, EbmlReader reader)
        {
            object? value = null;
            switch (info.ElementDescriptor.Type)
            {
                case ElementType.AsciiString:
                    value = reader.ReadAscii();
                    break;

                case ElementType.Binary:
                    int len = (int)reader.ElementSize;
                    var buffer = ArrayPool<byte>.Shared.Rent(len);
                    reader.ReadBinary(buffer, 0, len);
                    value = buffer.AsSpan().Slice(0, len).ToArray();
                    break;

                case ElementType.Date:
                    value = reader.ReadDate();
                    break;

                case ElementType.Float:
                    value = reader.ReadFloat();
                    break;

                case ElementType.MasterElement:
                    value = Deserialize(info.ElementType, reader);
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
                info.PropertyInfo.SetValue(instance, value);
            }
        }

        private static bool TryGet(Type type, ulong identifier, out MatroskaElementInfo info)
        {
            return GetInfoFromCache(type).TryGetValue(identifier, out info);
        }

        private static Dictionary<ulong, MatroskaElementInfo> GetInfoFromCache(Type type)
        {
            return cache.AddOrGetExisting(type.FullName, () =>
            {
                var dictionary = new Dictionary<ulong, MatroskaElementInfo>();
                foreach (var property in type.GetProperties())
                {
                    var attribute = property.GetCustomAttributes().OfType<MatroskaElementDescriptorAttribute>().FirstOrDefault();
                    if (attribute == null)
                    {
                        continue;
                    }

                    var info = new MatroskaElementInfo
                    {
                        PropertyInfo = property,
                        Identifier = attribute.Identifier,
                        ElementType = attribute.ElementType ?? property.PropertyType,
                        ElementDescriptor = MatroskaSpecification.ElementDescriptorsByIdentifier[attribute.Identifier]
                    };

                    dictionary.Add(attribute.Identifier, info);
                }

                return dictionary;
            });
        }
    }

    struct MatroskaElementInfo
    {
        public PropertyInfo PropertyInfo { get; set; }

        public ulong Identifier { get; set; }

        public Type ElementType { get; set; }

        public ElementDescriptor ElementDescriptor { get; set; }
    }
}
