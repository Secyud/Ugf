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
                case FieldType.InValid:
                    throw new UgfException($"invalid field ${value}");
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
            Write(TypeManager.Instance[type].Id);

            SaveProperties(value);
        }

        public void SaveProperties(object value)
        {
            TypeDescriptor descriptor =
                TypeManager.Instance[value.GetType()];
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
                long pRecord = BaseStream.Position;
                object field = attr.GetValue(value);

                if (!attr.ReadOnly ||
                    attr.Type != FieldType.Object)
                {
                    Write((byte)attr.Type);
                    WriteDataObject(field, attr.Type);
                }
                else
                {
                    if (field is IList list)
                    {
                        WriteList(list, attr);
                    }
                    else
                    {
                        SaveProperties(field);
                    }
                }

                int len = (int)(BaseStream.Position - pRecord);

                BaseStream.Seek(-len - 4, SeekOrigin.Current);
                Write(len);
                BaseStream.Seek(len, SeekOrigin.Current);
            }
        }

        private void WriteList(IList list, SAttribute s)
        {
            if (!list.IsFixedSize)
            {
                Write(list.Count);
            }

            for (int i = 0; i < list.Count; i++)
            {
                WriteDataObject(list[i], s.ElementType);
            }
        }

        public void SaveResource(Guid id, ResourceDescriptor resource)
        {
            Write(id);
            Write(resource.Name);
            Write(resource.Data.Length);
            Write(resource.Data);
        }
    }
}