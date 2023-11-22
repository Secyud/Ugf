﻿using System;
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
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class TypeManager
    {
        private readonly ConcurrentDictionary<Guid, Type> _typeDict = new();
        private readonly ConcurrentDictionary<Type, Guid> _idDict = new();
        private readonly ConcurrentDictionary<Type, TypeDescriptor> _propertyDict = new();
        private readonly MD5 _md5 = MD5.Create();
        public static TypeManager Instance { get; } = new();

        public TypeDescriptor GetProperty([NotNull] Type type)
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
            return ConstructFromResource(typeof(T), name) as T;
        }

        public object ConstructFromResource(Guid typeId, string name)
        {
            return ConstructFromResource(this[typeId], name);
        }

        public object ConstructFromResource(Type type, string name)
        {
            TypeDescriptor property = GetProperty(type);
            if (property.Resources.TryGetValue(name, out ResourceDescriptor resource))
            {
                object obj = U.Get(property.Type);
                resource.WriteToObject(obj);

                if (obj is IDataResource r &&
                    r.ResourceId.IsNullOrEmpty())
                {
                    r.ResourceId = name;
                }

                return obj;
            }

            Debug.LogWarning($"Failed construct from resource: {type}" +
                             $"\r\n\t Resource Id: {name}");
            return null;
        }

        public List<TObject> ConstructListFromFile<TObject>(string path)
            where TObject : class
        {
            using FileStream fileStream = File.OpenRead(path);
            using DataReader reader = new(fileStream);

            List<TObject> list = new();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Guid id = reader.ReadGuid();
                string name = reader.ReadString();
                int dataLength = reader.ReadInt32();

                Type type = U.Tm[id];
                object obj = U.Get(type);
                reader.LoadProperties(obj);

                list.Add(obj as TObject);
            }

            return list;
        }

        public bool TryWriteObject(object obj, string resourceId)
        {
            TypeDescriptor property = GetProperty(obj.GetType());
            if (!property.Resources
                    .TryGetValue(resourceId, out ResourceDescriptor resource))
            {
                return false;
            }

            resource.WriteToObject(obj);
            return true;
        }

        public Type this[Guid id]
        {
            get
            {
                if (U.DataManager)
                {
                    _typeDict.TryGetValue(id, out Type value);
                    return value;
                }

                return _typeDict[id];
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

        public void ReadResource(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using DataReader reader = new(stream);

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                Guid id = reader.ReadGuid();
                string name = reader.ReadString();
                int len = reader.ReadInt32();
                byte[] data = reader.ReadBytes(len);

                TypeDescriptor descriptor = GetProperty(this[id]);
                descriptor.Resources[name] = new ResourceDescriptor(name)
                {
                    Data = data
                };
            }
        }

        private void CheckId(ref Guid id, Type type)
        {
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