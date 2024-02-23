using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.Logging;

namespace System.Ugf.IO
{
    public static class UgfBinaryExtension
    {
        public static void SaveResource(this BinaryWriter writer, IHasResourceId resourceId)
        {
            writer.Write(resourceId.ResourceId);
        }

        public static void LoadResource(this BinaryReader reader, IHasResourceId shown)
        {
            int resourceId = reader.ReadInt32();
            TypeManager.Instance
                .LoadObjectFromResource(shown, resourceId);
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        public static void ReadList<T>(this BinaryReader reader, IList<T> value) where T : class
        {
            value.Clear();
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                T obj = reader.ReadObject<T>();
                if (obj is null)
                {
                    UgfLogger.LogError($"Loading {typeof(T).Name} failed, Index {i}!");
                }

                value.Add(obj);
            }
        }


        public static void ReadEnsuredList<T>(this BinaryReader reader, IList<T> value)
            where T : IArchivable, new()
        {
            value.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                T t = new();
                t.Load(reader);
                value.Add(t);
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

        public static void WriteString(this BinaryWriter writer, string value)
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

        public static void WriteList<T>(this BinaryWriter writer, IList<T> value) where T : class
        {
            writer.Write(value.Count);
            foreach (T t in value)
            {
                writer.WriteObject(t);
            }
        }

        public static void WriteEnsuredList<T>(this BinaryWriter writer, IList<T> value)
            where T : IArchivable, new()
        {
            writer.Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                value[i].Save(writer);
            }
        }

        public static void SerializeResource(this BinaryWriter writer, object resource)
        {
            Queue<object> resources = new();
            resources.Enqueue(resource);
            SortedDictionary<string, SAttribute> attributes = new();

            while (resources.Count > 0)
            {
                object obj = resources.Dequeue();
                TypeDescriptor descriptor = TypeManager.Instance[obj.GetType()];
                descriptor.Properties.FillAttributes(attributes);
                writer.Write(attributes.Count);
                foreach ((string attributeName, SAttribute attribute) in attributes)
                {
                    writer.Write(attributeName);
                    writer.Write((byte)attribute.Type);
                    WriteValue(attribute.Type, attribute.GetValue(obj));
                }
            }

            return;

            void WriteValue(FieldType fieldType, object fieldValue)
            {
                switch (fieldType)
                {
                    case FieldType.UInt8:
                    {
                        writer.Write((byte)fieldValue);
                    }
                        break;
                    case FieldType.UInt16:
                    {
                        writer.Write((ushort)fieldValue);
                    }
                        break;
                    case FieldType.UInt32:
                    {
                        writer.Write((uint)fieldValue);
                    }
                        break;
                    case FieldType.UInt64:
                    {
                        writer.Write((ulong)fieldValue);
                    }
                        break;
                    case FieldType.Int8:
                    {
                        writer.Write((sbyte)fieldValue);
                    }
                        break;
                    case FieldType.Int16:
                    {
                        writer.Write((short)fieldValue);
                    }
                        break;
                    case FieldType.Int32:
                    {
                        writer.Write((int)fieldValue);
                    }
                        break;
                    case FieldType.Int64:
                    {
                        writer.Write((long)fieldValue);
                    }
                        break;
                    case FieldType.Bool:
                    {
                        writer.Write((bool)fieldValue);
                    }
                        break;
                    case FieldType.Decimal:
                    {
                        writer.Write((decimal)fieldValue);
                    }
                        break;
                    case FieldType.Single:
                    {
                        writer.Write((float)fieldValue);
                    }
                        break;
                    case FieldType.Double:
                    {
                        writer.Write((double)fieldValue);
                    }
                        break;
                    case FieldType.String:
                    {
                        writer.WriteString((string)fieldValue);
                    }
                        break;
                    case FieldType.Guid:
                    {
                        writer.Write((Guid)fieldValue);
                    }
                        break;
                    case FieldType.Object:
                    {
                        if (fieldValue is null)
                        {
                            writer.Write(false);
                        }
                        else
                        {
                            writer.Write(true);
                            writer.Write(fieldValue.GetType().GUID);
                            resources.Enqueue(fieldValue);
                        }
                    }
                        break;
                    case FieldType.List:
                    case FieldType.InValid:
                        throw new NotSupportedException("Field type not support!");
                    default:
                        if (fieldValue is IList list)
                        {
                            FieldType elementType = fieldType & ~FieldType.List;
                            writer.Write(true);
                            writer.Write(list.Count);
                            foreach (object value in list)
                            {
                                WriteValue(elementType, value);
                            }
                        }
                        else
                        {
                            writer.Write(false);
                        }

                        break;
                }
            }
        }

        public static void DeserializeResource(this BinaryReader reader, object resource)
        {
            Queue<object> resources = new();
            resources.Enqueue(resource);
            SortedDictionary<string, SAttribute> attributes = new();

            while (resources.Count > 0)
            {
                object obj = resources.Dequeue();
                if (obj is not null)
                {
                    TypeManager.Instance[obj.GetType()]
                        .Properties.FillAttributes(attributes);
                }
                else
                {
                    attributes.Clear();
                }

                if (reader.BaseStream.Position == reader.BaseStream.Length)
                {
                    break;
                }

                int attributeCount = reader.ReadInt32();

                for (int j = 0; j < attributeCount; j++)
                {
                    string attributeName = reader.ReadString();
                    FieldType attributeType = (FieldType)reader.ReadByte();

                    if (attributes.TryGetValue(attributeName, out SAttribute attribute)
                        && obj is not null)
                    {
                        if (attributeType.HasFlag(FieldType.List))
                        {
                            bool notNull = reader.ReadBoolean();
                            if (!notNull) continue;
                            FieldType elementType = attributeType & ~FieldType.List;
                            int count = reader.ReadInt32();
                            if (attribute.GetValue(obj) is IList list)
                            {
                                if (list.IsFixedSize)
                                {
                                    int k = 0;
                                    for (; k < count; k++)
                                    {
                                        if (k < list.Count)
                                            list[k] = ReadValue(elementType);
                                        else break;
                                    }

                                    if (k < count)
                                    {
                                        ReadValue(elementType, count - k);
                                    }
                                }
                                else
                                {
                                    list.Clear();

                                    for (int k = 0; k < count; k++)
                                    {
                                        list.Add(ReadValue(elementType));
                                    }
                                }
                            }
                            else
                            {
                                ReadValue(attributeType & ~FieldType.List,
                                    count);
                            }
                        }
                        else
                        {
                            attribute.SetValue(obj, ReadValue(attributeType));
                        }
                    }
                    else
                    {
                        if (attributeType.HasFlag(FieldType.List))
                        {
                            bool notNull = reader.ReadBoolean();
                            if (!notNull) continue;
                            ReadValue(
                                attributeType & ~FieldType.List,
                                reader.ReadInt32());
                        }
                        else
                        {
                            ReadValue(attributeType, 1);
                        }
                    }
                }
            }

            return;

            object ReadValue(FieldType fieldType, int seek = 0)
            {
                if (seek > 0)
                {
                    switch (fieldType)
                    {
                        case FieldType.UInt8:
                        {
                            reader.BaseStream.Seek(seek * sizeof(byte), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.UInt16:
                        {
                            reader.BaseStream.Seek(seek * sizeof(ushort), SeekOrigin.Current);

                            break;
                        }
                        case FieldType.UInt32:
                        {
                            reader.BaseStream.Seek(seek * sizeof(uint), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.UInt64:
                        {
                            reader.BaseStream.Seek(seek * sizeof(ulong), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Int8:
                        {
                            reader.BaseStream.Seek(seek * sizeof(sbyte), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Int16:
                        {
                            reader.BaseStream.Seek(seek * sizeof(short), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Int32:
                        {
                            reader.BaseStream.Seek(seek * sizeof(int), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Int64:
                        {
                            reader.BaseStream.Seek(seek * sizeof(long), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Bool:
                        {
                            reader.BaseStream.Seek(seek * sizeof(bool), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Decimal:
                        {
                            reader.BaseStream.Seek(seek * sizeof(decimal), SeekOrigin.Current);
                            break;
                        }
                        case FieldType.Single:
                        {
                            reader.BaseStream.Seek(seek * sizeof(float), SeekOrigin.Current);

                            break;
                        }
                        case FieldType.Double:
                        {
                            reader.BaseStream.Seek(seek * sizeof(double), SeekOrigin.Current);

                            break;
                        }
                        case FieldType.String:
                        {
                            for (int i = 0; i < seek; i++)
                            {
                                int size = reader.Read7BitEncodedInt();
                                reader.BaseStream.Seek(size, SeekOrigin.Current);
                            }

                            break;
                        }
                        case FieldType.Guid:
                        {
                            reader.BaseStream.Seek(seek * 16, SeekOrigin.Current);

                            break;
                        }
                        case FieldType.Object:
                        {
                            bool notNull = reader.ReadBoolean();

                            if (notNull)
                            {
                                reader.BaseStream.Seek(seek * 16, SeekOrigin.Current);
                            }

                            resources.Enqueue(null);
                            break;
                        }
                        case FieldType.List:
                        case FieldType.InValid:
                        default: throw new NotSupportedException();
                    }

                    return null;
                }

                switch (fieldType)
                {
                    case FieldType.UInt8:
                    {
                        return reader.ReadByte();
                    }
                    case FieldType.UInt16:
                    {
                        return reader.ReadInt16();
                    }
                    case FieldType.UInt32:
                    {
                        return reader.ReadUInt32();
                    }
                    case FieldType.UInt64:
                    {
                        return reader.ReadUInt64();
                    }
                    case FieldType.Int8:
                    {
                        return reader.ReadSByte();
                    }
                    case FieldType.Int16:
                    {
                        return reader.ReadInt16();
                    }
                    case FieldType.Int32:
                    {
                        return reader.ReadInt32();
                    }
                    case FieldType.Int64:
                    {
                        return reader.ReadInt64();
                    }
                    case FieldType.Bool:
                    {
                        return reader.ReadBoolean();
                    }
                    case FieldType.Decimal:
                    {
                        return reader.ReadDecimal();
                    }
                    case FieldType.Single:
                    {
                        return reader.ReadSingle();
                    }
                    case FieldType.Double:
                    {
                        return reader.ReadDouble();
                    }
                    case FieldType.String:
                    {
                        return reader.ReadString();
                    }
                    case FieldType.Guid:
                    {
                        return reader.ReadGuid();
                    }
                    case FieldType.Object:
                    {
                        bool notNull = reader.ReadBoolean();
                        object instance = notNull
                            ? TypeManager.Instance
                                .CreateInstance(reader.ReadGuid())
                            : null;
                        resources.Enqueue(instance);
                        return instance;
                    }
                    case FieldType.List:
                    case FieldType.InValid:
                    default: throw new NotSupportedException();
                }
            }
        }


        /// <summary>Reads in a 32-bit integer in compressed format.</summary>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
        /// <exception cref="T:System.FormatException">The stream is corrupted.</exception>
        /// <returns>A 32-bit integer in compressed format.</returns>
        private static int Read7BitEncodedInt(this BinaryReader reader)
        {
            uint num1 = 0;
            for (int index = 0; index < 28; index += 7)
            {
                byte num2 = reader.ReadByte();
                num1 |= (uint)((num2 & sbyte.MaxValue) << index);
                if (num2 <= 127)
                    return (int)num1;
            }

            byte num3 = reader.ReadByte();
            if (num3 > 15)
                throw new FormatException("Format_Bad7BitInt");
            return (int)(num1 | (uint)num3 << 28);
        }
    }
}