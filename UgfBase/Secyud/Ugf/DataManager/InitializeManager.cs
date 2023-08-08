using System;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.DataManager
{
    public sealed class InitializeManager:IRegistry
    {
        private readonly Dictionary<Type, Dictionary<string, ResourceDescriptor>> _resource = new();
        private readonly Dictionary<Type, PropertyDescriptor> _properties = new();

        public Dictionary<string, ResourceDescriptor> GetOrAddDescriptors(Type templateType)
        {
            if (!_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict))
            {
                dict = new Dictionary<string, ResourceDescriptor>();
                _resource[templateType] = dict;
            }

            return dict;
        }

        public ResourceDescriptor GetOrAddDescriptor(Guid typeId, string name, Type templateType)
        {
            Dictionary<string, ResourceDescriptor> dict = GetOrAddDescriptors(templateType);

            if (!dict.TryGetValue(name, out ResourceDescriptor descriptor))
            {
                descriptor = new ResourceDescriptor(name, typeId, templateType);
                dict[name] = descriptor;
            }

            return descriptor;
        }

        public ResourceDescriptor GetDescriptor(Type templateType, string name)
        {
            if (_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict) &&
                dict.TryGetValue(name, out ResourceDescriptor descriptor))
                return descriptor;
            throw new UgfException($"Cannot get resource for template id {templateType} named {name}");
        }

        public ResourceDescriptor TryGetDescriptor(Type templateType, string name)
        {
            if (_resource.TryGetValue(templateType, out Dictionary<string, ResourceDescriptor> dict) &&
             dict.TryGetValue(name, out ResourceDescriptor descriptor))
                return descriptor;
            return null;
        }

        public ResourceDescriptor GetDescriptor<TTemplate>(string name)
        {
            return GetDescriptor(typeof(TTemplate), name);
        }

        public PropertyDescriptor GetProperty(Type type)
        {
            if (!_properties.TryGetValue(type, out PropertyDescriptor descriptor))
            {
                descriptor = new PropertyDescriptor(type);
                _properties[type] = descriptor;
            }

            return descriptor;
        }
        
    }
}