using System.Collections.Generic;
using JetBrains.Annotations;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;

namespace System.IO
{
    public static class UgfBinaryExtension
    {
        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        public static void ReadList<T>(this BinaryReader reader, IList<T> value) where T : class
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                value.Add(reader.ReadObject<T>());
            }
        }

        public static TObject ReadObject<TObject>(this BinaryReader reader)
            where TObject : class
        {
            object obj = TypeManager
                .Instance[reader.ReadGuid()]
                .CreateInstance();
            if (obj is IArchivable archivable)
            {
                archivable.Load(reader);
            }

            return obj as TObject;
        }

        public static TObject ReadNullable<TObject>(this BinaryReader reader)
            where TObject : class
        {
            return reader.ReadBoolean() ? reader.ReadObject<TObject>() : null;
        }

        public static void Write(this BinaryWriter writer, Guid id)
        {
            writer.Write(id.ToByteArray());
        }

        public static void Write(this BinaryWriter writer, [CanBeNull] string value)
        {
            writer.Write(value ?? string.Empty);
        }

        public static void WriteObject(this BinaryWriter writer, object value)
        {
            writer.Write(value.GetType().GUID);
            if (value is IArchivable archivable)
            {
                archivable.Save(writer);
            }
        }

        public static void WriteNullable(this BinaryWriter writer, object value)
        {
            if (value is null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.WriteObject(value);
            }
        }

        public static void WriteListable<T>(this BinaryWriter writer, IList<T> value) where T : class
        {
            writer.Write(value.Count);
            foreach (T t in value)
            {
                writer.WriteObject(t);
            }
        }
    }
}