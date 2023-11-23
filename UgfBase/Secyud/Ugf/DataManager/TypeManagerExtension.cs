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
            using DataReader reader = new(stream);
            List<object> list = new();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                object obj = reader.ReadResourceObject();

                if (obj is not null)
                {
                    list.Add(obj);
                }
            }

            return list;
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
                object obj = reader.ReadResourceObject();

                if (obj is not null)
                {
                    list.Add(obj as TObject);
                }
            }

            return list;
        }

        public static T ReadObjectFromResource<T>(this TypeManager manager, string name)
            where T : class
        {
            return manager.ReadObjectFromResource(typeof(T), name) as T;
        }

        public static object ReadObjectFromResource(this TypeManager manager, Guid typeId, string name)
        {
            return manager.ReadObjectFromResource(manager[typeId], name);
        }
    }
}