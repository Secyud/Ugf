using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DataManager
{
    internal class DataLoader 
    {
        private readonly BinaryReader _reader;

        public DataLoader(BinaryReader reader)
        {
            _reader = reader;
        }

        public object ReadDataObject(FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Bool    => _reader.ReadBoolean(),
                FieldType.UInt8   => _reader.ReadByte(),
                FieldType.UInt16  => _reader.ReadUInt16(),
                FieldType.UInt32  => _reader.ReadUInt32(),
                FieldType.UInt64  => _reader.ReadUInt64(),
                FieldType.Int8    => _reader.ReadSByte(),
                FieldType.Int16   => _reader.ReadInt16(),
                FieldType.Int32   => _reader.ReadInt32(),
                FieldType.Int64   => _reader.ReadInt64(),
                FieldType.Single  => _reader.ReadSingle(),
                FieldType.Double  => _reader.ReadDouble(),
                FieldType.Decimal => _reader.ReadDecimal(),
                FieldType.String  => _reader.ReadString(),
                FieldType.Guid    => _reader.ReadGuid(),
                FieldType.Object  => ReadClassObject(),
                FieldType.InValid => new UgfException($"invalid field"),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        private object ReadClassObject()
        {
            if (!_reader.ReadBoolean())
                return null;

            Guid guid = _reader.ReadGuid();

            TypeDescriptor descriptor = TypeManager.Instance[guid];

            object ret = descriptor.CreateInstance();

            LoadProperties(ret);

            return ret;
        }

        public void LoadProperties(object value)
        {
            TypeDescriptor descriptor = TypeManager.Instance[value.GetType()];
            SortedDictionary<string, SAttribute> attrs = new();

            PropertyDescriptor current = descriptor.Properties;
            while (current is not null)
            {
                foreach (SAttribute attribute in current.Attributes)
                {
                    attrs[attribute.Info.Name] = attribute;
                }

                current = current.BaseProperty;
            }

            int count = _reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = _reader.ReadString();
                int len = _reader.ReadInt32();

                long position = _reader.BaseStream.Position;

                try
                {
                    if (attrs.TryGetValue(name, out SAttribute attr))
                    {
                        if (!attr.ReadOnly ||
                            attr.Type != FieldType.Object)
                        {
                            attr.SetValue(value, ReadDataObject((FieldType)_reader.ReadByte()));
                        }
                        else
                        {
                            object field = attr.GetValue(value);

                            if (field is IList list)
                            {
                                LoadList(list, attr);
                            }
                            else
                            {
                                LoadProperties(field);
                            }
                        }
                    }
                    else
                    {
                        _reader.BaseStream.Seek(len, SeekOrigin.Current);
                    }
                }
                catch (Exception e)
                {
                    UgfLogger.LogException(e);
                    _reader.BaseStream.Seek(len + position, SeekOrigin.Begin);
                }
            }
        }

        public void LoadList(IList list, SAttribute s)
        {
            if (list.IsFixedSize)
            {
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    list[i] = ReadDataObject(s.ElementType);
                }
            }
            else
            {
                int count = _reader.ReadInt32();
                list.Clear();
                for (int i = 0; i < count; i++)
                {
                    list.Add(ReadDataObject(s.ElementType));
                }
            }
        }
    }
}