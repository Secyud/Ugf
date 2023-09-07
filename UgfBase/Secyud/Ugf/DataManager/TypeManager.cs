using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Secyud.Ugf.DataManager
{
    public class TypeManager
    {
        private readonly ConcurrentDictionary<Guid, Type> _typeDict = new();
        private readonly ConcurrentDictionary<string, Guid> _idDict = new();
        private readonly ConcurrentDictionary<Type, TypeDescriptor> _propertyDict = new();
        private readonly MD5 _md5 = MD5.Create();
        public static TypeManager Instance { get; } = new();

        public TypeDescriptor GetProperty(Type type)
        {
            if (!_propertyDict.TryGetValue(type, out TypeDescriptor property))
            {
                property = new TypeDescriptor(type);
                _propertyDict[type] = property;
            }

            return property;
        }
        
        public T ConstructFromResource<T>(string name) 
            where T : class
        {
            TypeDescriptor property= U.Tm.GetProperty(typeof(T));
            T obj = U.Get<T>();
            property.Resources[name].WriteToObject(obj);
            return obj;
        }

        public object ConstructFromResource(Guid typeId, string name)
        {
            return ConstructFromResource(this[typeId],name);
        }
        
        public object ConstructFromResource(Type type, string name)
        {
            TypeDescriptor property = GetProperty(type);
            if (property.Resources.TryGetValue(name, out ResourceDescriptor resource))
            {
                object obj = U.Get(property.Type);
                resource.WriteToObject(obj);
                return obj;
            }

            return null;
        }

        public Type this[Guid id]
        {
            get => _typeDict[id];
            set
            {
                string name = value.FullName;
                CheckId(ref id, name);
                if (name is not null)
                {
                    _typeDict[id] = value;
                    _idDict[name] = id;
                }
            }
        }

        public Guid this[Type type]
        {
            get
            {
                if (type.FullName is null)
                    return default;
                if (!_idDict.TryGetValue(type.FullName, out Guid id))
                {
                    id = new Guid(_md5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName)));
                    this[type] = id;
                }

                return id;
            }
            set
            {
                string name = type.FullName;
                CheckId(ref value, name);
                if (name is not null)
                {
                    _typeDict[value] = type;
                    _idDict[name] = value;
                }
            }
        }

        public List<Tuple<string, Guid>> SubTypes(Type type = null)
        {
            IEnumerable<KeyValuePair<Guid, Type>> types = _typeDict;
            if (type is not null)
            {
                types = types
                    .Where(u =>
                        type.IsAssignableFrom(u.Value) &&
                        !u.Value.IsAbstract && u.Value.IsClass);
            }

            return
                types.Select(u => new Tuple<string, Guid>(u.Value.Name, u.Key))
                    .ToList();
        }

        private void CheckId(ref Guid id, string name)
        {
            if (id == default)
                id = new Guid(_md5.ComputeHash(Encoding.UTF8.GetBytes(name)));
        }
    }
}