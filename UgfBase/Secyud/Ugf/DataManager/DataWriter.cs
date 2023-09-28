using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class DataWriter : DefaultArchiveWriter
    {
        public DataWriter(Stream stream) : base(stream)
        {
        }

        public void WriteDataObject(object value, FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Bool:
                    Write((bool)value);
                    break;
                case FieldType.UInt8:
                    Write((byte)value);
                    break;
                case FieldType.UInt16:
                    Write((ushort)value);
                    break;
                case FieldType.UInt32:
                    Write((uint)value);
                    break;
                case FieldType.UInt64:
                    Write((ulong)value);
                    break;
                case FieldType.Int8:
                    Write((sbyte)value);
                    break;
                case FieldType.Int16:
                    Write((short)value);
                    break;
                case FieldType.Int32:
                    Write((int)value);
                    break;
                case FieldType.Int64:
                    Write((long)value);
                    break;
                case FieldType.Single:
                    Write((float)value);
                    break;
                case FieldType.Double:
                    Write((double)value);
                    break;
                case FieldType.Decimal:
                    Write((decimal)value);
                    break;
                case FieldType.String:
                    Write((string)value ?? string.Empty);
                    break;
                case FieldType.Guid:
                    Write((Guid)value);
                    break;
                case FieldType.Object:
                    WriteClassObject(value);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteClassObject(object value)
        {
            if (value is null)
            {
                Write(false);
                return;
            }

            Write(true);

            Type type = value.GetType();
            Write(TypeManager.Instance[type]);

            SaveProperties(type, value);
        }

        public void SaveProperties(Type type, object value)
        {
            TypeDescriptor descriptor = TypeManager.Instance.GetProperty(type);
            List<SAttribute> attrs = new();

            
            PropertyDescriptor current = descriptor.Properties;
            while (current is not null)
            {
                foreach (SAttribute attribute in current.Attributes)
                {
                    attrs.AddIfNotContains(attribute);
                }

                current = current.BaseProperty;
            }

            Write(attrs.Count);

            foreach (SAttribute attr in attrs)
            {
                Write(attr.Info.Name);
                Write(0);
                long pRecord = Writer.BaseStream.Position;
                object field = attr.GetValue(value);
                if (attr.ReadOnly)
                {
                    if (field is IList list)
                    {
                        WriteList(list);
                    }
                    else
                    {
                        SaveProperties(attr.Info.FieldType, field);
                    }
                }
                else
                {
                    Write((byte)attr.Type);
                    WriteDataObject(field, attr.Type);
                }

                int len = (int)(Writer.BaseStream.Position - pRecord);

                Writer.BaseStream.Seek(-len - 4, SeekOrigin.Current);
                Write(len);
                Writer.BaseStream.Seek(len, SeekOrigin.Current);
            }
        }

        private void WriteList(IList list)
        {
            Write(list.Count);

            foreach (object obj in list)
            {
                SAttribute.Map.TryGetValue(obj.GetType(), out FieldType fieldType);
                WriteDataObject(obj, fieldType);
            }
        }
    }
}