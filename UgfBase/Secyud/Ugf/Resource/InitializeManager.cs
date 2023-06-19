using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Archiving;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
    [Registry]
    public sealed class InitializeManager
    {
        private readonly IDependencyProvider _provider;
        private readonly Dictionary<Type, Dictionary<string, ResourceDescriptor>> _resource = new();
        private readonly Dictionary<Type, PropertyDescriptor> _properties = new();

        public InitializeManager(IDependencyProvider provider)
        {
            _provider = provider;
        }

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

        public ResourceDescriptor GetDescriptor<TTemplate>(string name)
        {
            return GetDescriptor(typeof(TTemplate), name);
        }

        public TTemplate ConstructAndInit<TTemplate>(string name) where TTemplate : class
        {
            ResourceDescriptor resource = GetDescriptor<TTemplate>(name);
            Type type = TypeMap.GetType(resource.TypeId);
            object obj = _provider.Get(type);
            PropertyDescriptor property = GetProperty(type);
            property.Init(obj, resource, false);
            return obj as TTemplate;
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

        public void RegisterFromBinary(string path, Type templateType)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning(
                    $"{nameof(InitializeManager)}_{nameof(RegisterFromBinary)}: file doesn't exist: {path}!"
                );
                return;
            }

            Dictionary<string, ResourceDescriptor> dict = GetOrAddDescriptors(templateType);

            using DefaultArchiveReader reader = new(path, _provider);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Guid id = reader.ReadGuid();
                string name = reader.ReadString();
                int length = reader.ReadInt32();

                if (!dict.TryGetValue(name, out ResourceDescriptor descriptor))
                {
                    descriptor = new ResourceDescriptor(name, id, templateType);
                    dict[name] = descriptor;
                }

                for (int j = 0; j < length; j++)
                {
                    PropertyType type = (PropertyType)reader.ReadByte();
                    short index = reader.ReadInt16();
                    descriptor.Data[index] = type switch
                    {
                        PropertyType.Bool    => reader.ReadBoolean(),
                        PropertyType.UInt8   => reader.ReadByte(),
                        PropertyType.UInt16  => reader.ReadUInt16(),
                        PropertyType.UInt32  => reader.ReadUInt32(),
                        PropertyType.UInt64  => reader.ReadUInt64(),
                        PropertyType.Int8    => reader.ReadSByte(),
                        PropertyType.Int16   => reader.ReadInt16(),
                        PropertyType.Int32   => reader.ReadInt32(),
                        PropertyType.Int64   => reader.ReadInt64(),
                        PropertyType.Single  => reader.ReadSingle(),
                        PropertyType.Double  => reader.ReadDouble(),
                        PropertyType.Decimal => reader.ReadDecimal(),
                        PropertyType.String  => reader.ReadString(),
                        PropertyType.Guid    => reader.ReadGuid(),
                        PropertyType.Object  => reader.ReadNullable<object>(),
                        _                    => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }
    }
}