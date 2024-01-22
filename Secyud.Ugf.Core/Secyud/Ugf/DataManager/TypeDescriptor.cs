using System;
using System.Collections.Generic;
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
        /// generate object form resource stored in <see cref="Resources"/>
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

            ResourceDescriptor descriptor = Resources.Get(resourceId);
            if (descriptor is null)
            {
                LogNotFindIdForType(resourceId);
                return null;
            }
            else
            {
                object obj = Activator.CreateInstance(Type);
                descriptor.LoadObject(obj);
                TrySetResourceId(obj, resourceId);
                return obj;
            }
        }

        public void LoadObjectFromResource(object obj, int resourceId)
        {
            ResourceDescriptor descriptor = Resources.Get(resourceId);
            if (descriptor is null)
            {
                LogNotFindIdForType(resourceId);
            }
            else
            {
                descriptor.LoadObject(obj);
                TrySetResourceId(obj, resourceId);
            }
        }

        public void AddResource(ResourceDescriptor descriptor)
        {
            Resources.Add(descriptor);
        }

        public void AddResources(IEnumerable<ResourceDescriptor> descriptors)
        {
            foreach (ResourceDescriptor descriptor in descriptors)
            {
                AddResource(descriptor);
            }
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
            private readonly SortedDictionary<int, ResourceDescriptor> _innerDictionary = new();

            public ResourceDescriptor Get(int id)
            {
                return _innerDictionary.GetValueOrDefault(id);
            }

            public void Add(ResourceDescriptor descriptor)
            {
                _innerDictionary[descriptor.Id] = descriptor;
            }
        }
    }
}