using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Resource
{
    public class ResourceDescriptor
    {
        public string Name { get; }
        public Guid TypeId { get; }
        public Type TemplateType { get; }
        public SortedDictionary<short, object> Data { get; } = new();

        public ResourceDescriptor(string name, Guid typeId, Type templateType)
        {
            Name = name;
            TypeId = typeId;
            TemplateType = templateType;
        }
    }
}