using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DataManager
{
    public class TypeManager
    {
        public static TypeManager Instance { get; } = new();

        private readonly ConcurrentDictionary<Guid, TypeDescriptor> _typeDict = new();
        private readonly ConcurrentDictionary<Type, TypeDescriptor> _genericTypeDict = new();

        private TypeManager()
        {
        }

        public void AddType(Type type)
        {
            if (!_typeDict.ContainsKey(type.GUID))
            {
                _typeDict[type.GUID] = new TypeDescriptor(type);
            }
        }

        public void AddResourcesFromStream(Stream stream)
        {
            using BinaryReader reader = new(stream);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Guid id = reader.ReadGuid();
                TypeDescriptor descriptor = this[id];
                ResourceDescriptor resource = new();
                resource.Load(reader);
                descriptor?.AddResource(resource);
            }
        }

        public TypeDescriptor this[Guid id]
        {
            get
            {
                if (!_typeDict.TryGetValue(id, out TypeDescriptor value))
                {
                    UgfLogger.LogError($"Cannot find type for id: {id}.");
                }

                return value;
            }
        }

        /// <summary>
        /// get or create the property of specific type;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TypeDescriptor this[Type type]
        {
            get
            {
                if (type is null) return null;

                if (type.IsGenericType)
                {
                    if (!_genericTypeDict.TryGetValue(type, out TypeDescriptor descriptor))
                    {
                        descriptor = new TypeDescriptor(type);
                        _genericTypeDict[type] = descriptor;
                    }

                    return descriptor;
                }

                return _typeDict.GetValueOrDefault(type.GUID);
            }
        }

        public bool IsRegistered(Type type)
        {
            return type is not null && _typeDict.ContainsKey(type.GUID);
        }

        public IEnumerable<Type> GetRegisteredType(Type baseType = null)
        {
            IEnumerable<TypeDescriptor> ret = _typeDict.Values;

            if (baseType is not null)
            {
                ret = ret
                    .Where(u =>
                        baseType.IsAssignableFrom(u.Type) &&
                        !u.Type.IsAbstract && u.Type.IsClass);
            }

            return ret.Select(u => u.Type);
        }
    }
}