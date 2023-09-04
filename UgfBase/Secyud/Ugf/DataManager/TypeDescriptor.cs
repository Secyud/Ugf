using System;

namespace Secyud.Ugf.DataManager
{
    public class TypeDescriptor
    {
        private PropertyDescriptor _data;
        private ResourcesDict _resources;
        public Type Type { get; }
        public PropertyDescriptor Properties => _data??= new PropertyDescriptor(Type);
        public ResourcesDict Resources => _resources??= new ResourcesDict();

        public TypeDescriptor(Type type)
        {
            Type = type;
        }
    }
}