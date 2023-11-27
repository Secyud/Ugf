﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Ugf;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class TypeDescriptor
    {
        private static readonly MD5 MD5 = MD5.Create();
        private PropertyDescriptor _data;
        private ResourcesDictionary _resources;
        public Type Type { get; }
        public Guid Id { get; }
        public PropertyDescriptor Properties => _data ??= new PropertyDescriptor(Type);
        public ResourcesDictionary Resources => _resources ??= new ResourcesDictionary();

        public TypeDescriptor(Type type)
        {
            Type = type;

            IDAttribute attr = type.GetCustomAttribute<IDAttribute>();

            if (attr is not null)
            {
                Id = attr.Id;
            }

            if (Id == default)
            {
                Id = new Guid(MD5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName ?? string.Empty)));
            }
        }

        /// <summary>
        /// generate object form resource stored in <see cref="_propertyDict"/>
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public object ReadObjectFromResource(string resourceId)
        {
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