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

        public void AddType(Type type, bool dependency)
        {
            if (type.GUID == default)
                return;
            if (!_typeDict.ContainsKey(type.GUID))
            {
                _typeDict[type.GUID] = new TypeDescriptor(type, dependency);
            }
        }

        public void AddResources(DataResource[] resources)
        {
            foreach (DataResource resource in resources)
            {
                TypeDescriptor descriptor = this[resource.Type];
                descriptor?.AddResource(resource);
            }
        }

        public object CreateInstance(Guid id)
        {
            return this[id]?.CreateInstance();
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
                        descriptor = new TypeDescriptor(type, false);
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

        public IEnumerable<TypeDescriptor> GetRegisteredType(Type baseType = null, bool dependency = true)
        {
            IEnumerable<TypeDescriptor> ret = _typeDict.Values;

            if (!dependency)
            {
                ret = ret.Where(u => !u.Dependency);
            }

            if (baseType is not null)
            {
                ret = ret
                    .Where(u =>
                        baseType.IsAssignableFrom(u.Type) &&
                        !u.Type.IsAbstract && u.Type.IsClass);
            }

            return ret;
        }
    }
}