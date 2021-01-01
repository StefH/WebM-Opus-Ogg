using System;

namespace Matroska.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class ElementDescriptorAttribute : Attribute
    {
        public ulong Identifier { get; }

        public Type ElementType { get; }

        public ElementDescriptorAttribute(ulong identifier, Type type = null)
        {
            Identifier = identifier;
            ElementType = type;
        }
    }
}