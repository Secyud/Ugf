using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Secyud.Ugf.DataManager
{
    internal class DataSaver 
    {
        private readonly BinaryWriter _writer;

        public DataSaver(BinaryWriter writer)
        {
            _writer = writer;
        }

        public void WriteDataObject(object value, FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Bool:
                    _writer.Write((bool)value);
                    break;
                case FieldType.UInt8:
                    _writer.Write((byte)value);
                    break;
                case FieldType.UInt16:
                    _writer.Write((ushort)value);
                    break;
                case FieldType.UInt32:
                    _writer.Write((uint)value);
                    break;
                case FieldType.UInt64:
                    _writer.Write((ulong)value);
                    break;
                case FieldType.Int8:
                    _writer.Write((sbyte)value);
                    break;
                case FieldType.Int16:
                    _writer.Write((short)value);
                    break;
                case FieldType.Int32:
                    _writer.Write((int)value);
                    break;
                case FieldType.Int64:
                    _writer.Write((long)value);
                    break;
                case FieldType.Single:
                    _writer.Write((float)value);
                    break;
                case FieldType.Double:
                    _writer.Write((double)value);
                    break;
                case FieldType.Decimal:
                    _writer.Write((decimal)value);
                    break;
                case FieldType.String:
                    _writer.Write((string)value ?? string.Empty);
                    break;
                case FieldType.Guid:
                    _writer.Write((Guid)value);
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
                _writer.Write(false);
                return;
            }

            _writer.Write(true);

            _writer.Write(value.GetType().GUID);

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

            _writer.Write(attrs.Count);

            foreach (SAttribute attr in attrs)
            {
                _writer.Write(attr.Info.Name);
                _writer.Write(0);
                long pRecord = _writer.BaseStream.Position;
                object field = attr.GetValue(value);

                if (!attr.ReadOnly ||
                    attr.Type != FieldType.Object)
                {
                    _writer.Write((byte)attr.Type);
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

                int len = (int)(_writer.BaseStream.Position - pRecord);

                _writer.BaseStream.Seek(-len - 4, SeekOrigin.Current);
                _writer.Write(len);
                _writer.BaseStream.Seek(len, SeekOrigin.Current);
            }
        }

        private void WriteList(IList list, SAttribute s)
        {
            if (!list.IsFixedSize)
            {
                _writer.Write(list.Count);
            }

            for (int i = 0; i < list.Count; i++)
            {
                WriteDataObject(list[i], s.ElementType);
            }
        }

        public void SaveResource(Guid id, ResourceDescriptor resource)
        {
            _writer.Write(id);
            _writer.Write(resource.Id);
            _writer.Write(resource.Data.Length);
            _writer.Write(resource.Data);
        }
    }
}