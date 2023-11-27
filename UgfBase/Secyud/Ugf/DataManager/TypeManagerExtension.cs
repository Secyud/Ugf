using System;
using System.Collections.Generic;
using System.IO;

namespace Secyud.Ugf.DataManager
{
    public static class TypeManagerExtension
    {
        /// <summary>
        /// return list object, the stream should generated from data manager
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<object> ReadResourceObjects(this Stream stream)
        {
            return stream.ReadResourceObjects<object>();
        }

        /// <summary>
        /// return list object, the stream should generated from data manager
        /// </summary>
        /// <param name="stream"></param>
        /// <typeparam name="TObject">limit the type of object</typeparam>
        /// <returns></returns>
        public static List<TObject> ReadResourceObjects<TObject>(this Stream stream)
            where TObject : class
        {
            using DataReader reader = new(stream);
            List<TObject> list = new();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                long position = reader.BaseStream.Position;
                object obj = null;
                int len = 0;
                Guid id = default;
                string resourceId = default;
                try
                {
                    id = reader.ReadGuid();
                    resourceId = reader.ReadString();
                    len = reader.ReadInt32();
                    TypeDescriptor descriptor = U.Tm[id];
                    obj = U.Get(descriptor.Type);
                    reader.LoadProperties(obj);
                }
                catch (Exception e)
                {
                    U.LogError(e);
                    U.LogError($"Failed read resource object: id: {id}\r\nresourceId: {resourceId}");
                    reader.BaseStream.Seek(position + len + 24, SeekOrigin.Begin);
                }

                if (obj is not null)
                {
                    list.Add(obj as TObject);
                }
            }

            return list;
        }

        public static T ReadObjectFromResource<T>(this TypeManager manager, string resourceId)
            where T : class
        {
            return manager.ReadObjectFromResource(typeof(T), resourceId) as T;
        }

        public static object ReadObjectFromResource(this TypeManager manager, Guid typeId, string resourceId)
        {
            TypeDescriptor property = manager[typeId];
            return property.ReadObjectFromResource(resourceId);
        }

        public static object ReadObjectFromResource(this TypeManager manager, Type type, string resourceId)
        {
            TypeDescriptor property = manager[type];
            return property.ReadObjectFromResource(resourceId);
        }

        public static void LoadObjectFromResource(this TypeManager manager, object obj, string resourceId)
        {
            TypeDescriptor property = manager[obj.GetType()];
            property.LoadObjectFromResource(obj, resourceId);
        }
    }
}