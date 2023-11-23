using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Ugf;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class TypeManager
    {
        private readonly ConcurrentDictionary<Guid, Type> _typeDict = new();

        /// <summary>
        /// separate guid and property. not every type needs property.
        /// </summary>
        private readonly ConcurrentDictionary<Type, Guid> _idDict = new();

        private readonly ConcurrentDictionary<Type, TypeDescriptor> _propertyDict = new();
        private readonly MD5 _md5 = MD5.Create();
        public static TypeManager Instance { get; } = new();

        /// <summary>
        /// get the property of specific type;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TypeDescriptor GetProperty([NotNull] Type type)
        {
            if (!_propertyDict.TryGetValue(type, out TypeDescriptor property))
            {
                property = new TypeDescriptor(type);
                _propertyDict[type] = property;
            }

            return property;
        }

        /// <summary>
        /// generate object form resource stored in <see cref="_propertyDict"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object ReadObjectFromResource(Type type, string resourceId)
        {
            try
            {
                TypeDescriptor property = GetProperty(type);
                if (property.Resources.TryGetValue(resourceId, out ResourceDescriptor resource))
                {
                    object obj = U.Get(property.Type);

                    WriteObject(obj, resourceId, resource);

                    return obj;
                }
            }
            catch (Exception e)
            {
                U.LogError($"Failed construct from resource: {type}" +
                           $"\r\n\t Resource Id: {resourceId}");
                U.LogError(e);
            }

            return null;
        }

        /// <summary>
        /// write object form resource stored in &lt;see cref="_propertyDict"/&gt;
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public void LoadObjectFromResource(object obj, string resourceId)
        {
            Type type = obj.GetType();
            TypeDescriptor property = GetProperty(type);

            if (property.Resources
                .TryGetValue(resourceId, out ResourceDescriptor resource))
            {
                WriteObject(obj, resourceId, resource);
            }
            else
            {
                U.LogError($"Failed construct from resource: {type}" +
                           $"\r\n\t Resource Id: {resourceId}");
            }
        }
        
        public void LoadResourcesFromStream(Stream stream)
        {
            using DataReader reader = new(stream);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                ResourceDescriptor resource = reader.ReadResource(out Guid id);
                Type type = this[id];

                if (type is not null)
                {
                    TypeDescriptor descriptor = GetProperty(type);
                    descriptor.Resources[resource.Name] = resource;
                }
            }
        }

        private void WriteObject(
            [NotNull] object obj,
            [NotNull] string resourceId,
            [NotNull] ResourceDescriptor resource)
        {
            resource.LoadObject(obj);

            if (obj is IDataResource r &&
                r.ResourceId.IsNullOrEmpty())
            {
                r.ResourceId = resourceId;
            }
        }

        public Type this[Guid id]
        {
            get
            {
                if (!_typeDict.TryGetValue(id, out Type value))
                {
                    U.LogError($"Cannot find type for id: {id}.");
                }

                return value;
            }
            set
            {
                if (value is not null)
                {
                    CheckId(ref id, value);
                    _typeDict[id] = value;
                    _idDict[value] = id;
                }
            }
        }

        public Guid this[Type type]
        {
            get
            {
                if (type is null)
                {
                    return default;
                }

                if (!_idDict.TryGetValue(type, out Guid id))
                {
                    CheckId(ref id, type);
                    this[type] = id;
                }

                return id;
            }
            set
            {
                if (type is not null)
                {
                    CheckId(ref value, type);
                    _typeDict[value] = type;
                    _idDict[type] = value;
                }
            }
        }

        /// <summary>
        /// used in data manager
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Guid TryGetId(Type type)
        {
            _idDict.TryGetValue(type, out var id);

            return id;
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


        private void CheckId(ref Guid id, Type type)
        {
            if (id != default)
            {
                return;
            }

            IDAttribute attr = type.GetCustomAttribute<IDAttribute>();
            if (attr is not null)
            {
                id = attr.Id;
            }

            if (id == default)
            {
                id = new Guid(_md5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName ?? string.Empty)));
            }
        }
    }
}