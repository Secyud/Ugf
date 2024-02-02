using System;
using System.IO;

namespace Secyud.Ugf.DataManager
{
    public static class TypeManagerExtension
    {
        /// <summary>
        /// return resources, the stream should generated from data manager
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataResource[] ReadResources(this Stream stream)
        {
            using BinaryReader reader = new(stream);
            int count = reader.ReadInt32();
            DataResource[] resources = new DataResource[count];
            for (int i = 0; i < count; i++)
            {
                resources[i].Load(reader);
            }

            return resources;
        }

        public static T ReadObjectFromResource<T>(this TypeManager manager, int resourceId)
            where T : class
        {
            return manager.ReadObjectFromResource(typeof(T), resourceId) as T;
        }

        public static object ReadObjectFromResource(this TypeManager manager, Guid typeId, int resourceId)
        {
            TypeDescriptor property = manager[typeId];
            return property.ReadObjectFromResource(resourceId);
        }

        public static object ReadObjectFromResource(this TypeManager manager, Type type, int resourceId)
        {
            TypeDescriptor property = manager[type];
            return property.ReadObjectFromResource(resourceId);
        }

        public static void LoadObjectFromResource(this TypeManager manager, object obj, int resourceId)
        {
            TypeDescriptor property = manager[obj.GetType()];
            property.LoadObjectFromResource(obj, resourceId);
        }
    }
}