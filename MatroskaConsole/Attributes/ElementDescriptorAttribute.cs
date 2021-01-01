using System;

namespace Matroska.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class ElementDescriptorAttribute : Attribute
    {
        public ulong Identifier { get; }

        public ElementDescriptorAttribute(ulong identifier)
        {
            Identifier = identifier;
        }
    }
}