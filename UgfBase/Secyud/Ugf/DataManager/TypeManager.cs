using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Secyud.Ugf.DataManager
{
    public class TypeManager
    {
        private readonly ConcurrentDictionary<Guid, TypeDescriptor> _typeDict = new();

        /// <summary>
        /// store property.
        /// </summary>
        private readonly ConcurrentDictionary<Type, TypeDescriptor> _propertyDict = new();

        public static TypeManager Instance { get; } = new();


        public void AddResourcesFromStream(Stream stream)
        {
            using DataReader reader = new(stream);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                ResourceDescriptor resource = reader.ReadResource(out Guid id);
                TypeDescriptor descriptor = this[id];
                if (descriptor is not null)
                {
                    descriptor.Resources[resource.Name] = resource;
                }
            }
        }

        public TypeDescriptor this[Guid id]
        {
            get
            {
                if (!_typeDict.TryGetValue(id, out TypeDescriptor value))
                {
                    U.LogError($"Cannot find type for id: {id}.");
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
                if (type is null)
                {
                    return null;
                }

                if (!_propertyDict.TryGetValue(type, out TypeDescriptor property))
                {
                    property = new TypeDescriptor(type);
                    _propertyDict[type] = property;
                    _typeDict[property.Id] = property;
                }

                return property;
            }
        }

        public TypeDescriptor TryGet(Type type)
        {
            if (type is null)
            {
                return null;
            }

            _propertyDict.TryGetValue(type, out TypeDescriptor property);
            
            return property;
        }

        public IEnumerable<TypeDescriptor> GetChildrenOfBaseType(Type baseType = null)
        {
            IEnumerable<TypeDescriptor> ret = _typeDict.Values;

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