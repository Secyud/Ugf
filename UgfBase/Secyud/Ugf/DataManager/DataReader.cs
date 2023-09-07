
//#define DATA_MANAGER

using System;
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

        public object ReadDataObject(FieldType fieldType)
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
                FieldType.Object  => ReadClassObject(),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        private object ReadClassObject()
        {
            if (!ReadBoolean())
                return null;

            Type type = TypeManager.Instance[ReadGuid()];

            object ret = U.Get(type);

            LoadProperties(type, ret);

            return ret;
        }

        public void LoadProperties(Type type, object value)
        {
            TypeDescriptor descriptor = TypeManager.Instance.GetProperty(type);
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

            int count = ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = ReadString();
                int len = ReadInt32();
#if DATA_MANAGER
                long position = Reader.BaseStream.Position;
#endif
                if (attrs.TryGetValue(name, out SAttribute attr))
                {
                    if (attr.ReadOnly)
                    {
                        object field = attr.GetValue(value);

                        if (field is IList list)
                        {
                            LoadList(list);
                        }
                        else
                        {
                            LoadProperties(attr.Info.FieldType, field);
                        }
                    }
                    else
                    {
#if DATA_MANAGER
                        try
                        {
                            attr.SetValue(value, ReadDataObject((FieldType)ReadByte()));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Reader.BaseStream.Seek(len + position, SeekOrigin.Begin);
                        }
#else
                        attr.SetValue(value, ReadDataObject((FieldType)ReadByte()));
#endif
                    }
                }
                else
                {
                    Reader.BaseStream.Seek(len, SeekOrigin.Current);
                }
            }
        }

        public void LoadList(IList list)
        {
            if (list.IsFixedSize)
            {
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    list[i] = ReadDataObject((FieldType)ReadByte());
                }
            }
            else
            {
                int count = ReadInt32();
                list.Clear();
                for (int i = 0; i < count; i++)
                {
                    list.Add(ReadDataObject((FieldType)ReadByte()));
                }
            }
        }
    }
}