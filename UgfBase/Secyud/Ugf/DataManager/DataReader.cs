﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class DataReader : DefaultArchiveReader
    {
        public DataReader(Stream stream) : base(stream)
        {
        }

        public object ReadDataObject(FieldType fieldType, DataLevel level)
        {
            return fieldType switch
            {
                FieldType.Bool    => ReadBoolean(),
                FieldType.UInt8   => ReadByte(),
                FieldType.UInt16  => ReadUInt16(),
                FieldType.UInt32  => ReadUInt32(),
                FieldType.UInt64  => ReadUInt64(),
                FieldType.Int8    => ReadSByte(),
                FieldType.Int16   => ReadInt16(),
                FieldType.Int32   => ReadInt32(),
                FieldType.Int64   => ReadInt64(),
                FieldType.Single  => ReadSingle(),
                FieldType.Double  => ReadDouble(),
                FieldType.Decimal => ReadDecimal(),
                FieldType.String  => ReadString(),
                FieldType.Guid    => ReadGuid(),
                FieldType.Object  => ReadClassObject(level),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        private object ReadClassObject(DataLevel level)
        {
            if (!ReadBoolean())
                return null;

            Type type = TypeManager.Instance[ReadGuid()];

            object ret = U.Get(type);

            LoadProperties(type, ret, level);

            return ret;
        }

        public void LoadProperties(Type type, object value, DataLevel level)
        {
            TypeDescriptor descriptor = TypeManager.Instance.GetProperty(type);
            SortedDictionary<string, SAttribute> attrs = new();

            PropertyDescriptor current = descriptor.Properties;
            while (current is not null)
            {
                foreach (SAttribute attribute in current.Attributes[(int)level])
                {
                    attrs[attribute.Info.Name] = attribute;
                }

                current = current.BaseProperty;
            }

            int count = ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = ReadString();
                int len = ReadInt32();
                if (attrs.TryGetValue(name, out SAttribute attr))
                {
                    if (attr.ReadOnly)
                    {
                        object field = attr.GetValue(value);

                        if (field is IList list)
                        {
                            LoadList(list, level);
                        }
                        else
                        {
                            LoadProperties(attr.Info.FieldType, field, level);
                        }
                    }
                    else
                    {
                        attr.SetValue(value, ReadDataObject((FieldType)ReadByte(), level));
                    }
                }
                else
                {
                    Reader.BaseStream.Seek(len, SeekOrigin.Current);
                }
            }
        }

        public void LoadList(IList list, DataLevel level)
        {
            if (list.IsFixedSize)
            {
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    list[i] = ReadDataObject((FieldType)ReadByte(), level);
                }
            }
            else
            {
                int count = ReadInt32();
                list.Clear();
                for (int i = 0; i < count; i++)
                {
                    list.Add(ReadDataObject((FieldType)ReadByte(), level));
                }
            }
        }
    }
}