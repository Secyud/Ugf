using System;

namespace Secyud.Ugf.DataManager
{
    public class TypeDescriptor
    {
        private PropertyDescriptor _data;
        private ResourcesDictionary _resources;
        public Type Type { get; }
        public PropertyDescriptor Properties => _data ??= new PropertyDescriptor(Type);
        public ResourcesDictionary Resources => _resources ??= new ResourcesDictionary();

        public TypeDescriptor(Type type)
        {
            Type = type;
        }
    }
}