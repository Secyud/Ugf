using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Secyud.Ugf.DependencyInjection;
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

        public bool Dependency { get; }

        public TypeDescriptor(Type type, bool dependency)
        {
            Dependency = dependency;
            Type = type;
        }

        public object CreateInstance()
        {
            return Dependency
                ? DependencyManager.Instance.Get(Type)
                : Activator.CreateInstance(Type, true);
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

            return Resources.Get(resourceId).GetObject();
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
            DataResource data = Resources.Get(resourceId);
            data.FillObject(obj);
        }

        public void AddResource(DataResource data)
        {
            Resources.Add(data);
        }

        private class ResourcesDictionary
        {
            private readonly SortedDictionary<int, DataResource> _innerDictionary = new();

            public DataResource Get(int id)
            {
                return _innerDictionary.GetValueOrDefault(id);
            }

            public void Add(DataResource data)
            {
                _innerDictionary[data.Id] = data;
            }
        }
    }
}