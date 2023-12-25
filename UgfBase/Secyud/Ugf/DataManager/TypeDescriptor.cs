using System;
using System.Collections.Generic;
using System.Ugf;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class TypeDescriptor
    {
        private PropertyDescriptor _data;
        private ResourcesDictionary _resources;
        public Type Type { get; }
        public PropertyDescriptor Properties => _data ??= new PropertyDescriptor(Type);
        public ResourcesDictionary Resources => _resources ??= new ResourcesDictionary();

        public TypeDescriptor(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// generate object form resource stored in <see cref="Resources"/>
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object ReadObjectFromResource(string resourceId)
        {
            if (resourceId is null)
            {
                U.LogError($"Please set resource id. For type {Type}");
                return null;
            }
            
            if (Resources.TryGetValue(resourceId, out ResourceDescriptor resource))
            {
                object obj = U.Get(Type);
                resource.LoadObject(obj);
                TrySetResourceId(obj, resourceId);
                return obj;
            }

            U.LogError($"Resource not find: {Type}" +
                       $"\r\n\t Resource Id: {resourceId}");

            return null;
        }

        public void LoadObjectFromResource(object obj, string resourceId)
        {
            if (Resources.TryGetValue(resourceId, out ResourceDescriptor resource))
            {
                resource.LoadObject(obj);
                TrySetResourceId(obj, resourceId);
            }
            else
            {
                U.LogError($"Resource not find: {Type}" +
                           $"\r\n\t Resource Id: {resourceId}");
            }
        }


        public void AddResource(ResourceDescriptor descriptor)
        {
            Resources[descriptor.Name] = descriptor;
        }

        public void AddResources(IEnumerable<ResourceDescriptor> descriptors)
        {
            foreach (ResourceDescriptor descriptor in descriptors)
            {
                AddResource(descriptor);
            }
        }

        private static void TrySetResourceId(object obj, string resourceId)
        {
            if (obj is IDataResource r &&
                r.ResourceId.IsNullOrEmpty())
            {
                r.ResourceId = resourceId;
            }
        }
    }
}