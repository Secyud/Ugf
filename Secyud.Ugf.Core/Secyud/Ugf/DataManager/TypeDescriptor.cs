using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DataManager
{
    public class TypeDescriptor
    {
        private PropertyDescriptor _data;
        private ResourcesDictionary _resources;
        public Type Type { get; }
        public PropertyDescriptor Properties => _data ??= new PropertyDescriptor(Type);
        private ResourcesDictionary Resources => _resources ??= new ResourcesDictionary();

        public TypeDescriptor(Type type)
        {
            Type = type;
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(Type);
        }

        /// <summary>
        /// generate object form resource stored in
        /// <see cref="Resources"/>
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object ReadObjectFromResource(int resourceId)
        {
            if (resourceId is 0)
            {
                UgfLogger.LogError($"Please set resource id. For type {Type}");
                return null;
            }

            byte[] data = Resources.Get(resourceId);
            if (data is null)
            {
                LogNotFindIdForType(resourceId);
                return null;
            }
            else
            {
                object obj = Activator.CreateInstance(Type);
                using MemoryStream stream = new(data);
                using BinaryReader reader = new(stream);
                reader.DeserializeResource(obj);
                TrySetResourceId(obj, resourceId);
                return obj;
            }
        }

        /// <summary>
        /// fill the object with the resource stored
        /// in <see cref="Resources"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="resourceId"></param>
        public void LoadObjectFromResource([NotNull] object obj, int resourceId)
        {
            Throw.IfNull(obj);
            byte[] data = Resources.Get(resourceId);
            if (data is null)
            {
                LogNotFindIdForType(resourceId);
            }
            else
            {
                using MemoryStream stream = new(data);
                using BinaryReader reader = new(stream);
                reader.DeserializeResource(obj);
                TrySetResourceId(obj, resourceId);
            }
        }

        public void AddResource(int id,[NotNull] byte[] data)
        {
            Resources.Add(id, data);
        }

        private static void TrySetResourceId(object obj, int resourceId)
        {
            if (obj is IDataResource { ResourceId: 0 } r)
            {
                r.ResourceId = resourceId;
            }
        }

        private void LogNotFindIdForType(int resourceId)
        {
            UgfLogger.LogError(
                $"Data with id {resourceId} not found for type {Type}.");
        }


        private class ResourcesDictionary
        {
            private readonly SortedDictionary<int, byte[]> _innerDictionary = new();

            public byte[] Get(int id)
            {
                return _innerDictionary.GetValueOrDefault(id);
            }

            public void Add(int id, [NotNull] byte[] data)
            {
                Throw.IfNull(data);
                _innerDictionary[id] = data;
            }
        }
    }
}