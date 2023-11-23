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
                FieldType.InValid => new UgfException($"invalid field"),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        private object ReadClassObject()
        {
            if (!ReadBoolean())
                return null;

            Guid guid = ReadGuid();

            Type type = TypeManager.Instance[guid];

            object ret = U.Get(type);

            LoadProperties(ret);

            return ret;
        }

        public void LoadProperties(object value)
        {
            TypeDescriptor descriptor = TypeManager.Instance.GetProperty(
                value.GetType());
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

                long position = Reader.BaseStream.Position;

                if (attrs.TryGetValue(name, out SAttribute attr))
                {
                    if (!attr.ReadOnly ||
                        attr.Type != FieldType.Object)
                    {
                        if (U.DataManager)
                        {
                            try
                            {
                                attr.SetValue(value, ReadDataObject((FieldType)ReadByte()));
                            }
                            catch (Exception e)
                            {
                                U.LogError(e);
                                Reader.BaseStream.Seek(len + position, SeekOrigin.Begin);
                            }
                        }
                        else
                        {
                            attr.SetValue(value, ReadDataObject((FieldType)ReadByte()));
                        }
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
                    Reader.BaseStream.Seek(len, SeekOrigin.Current);
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
                int count = ReadInt32();
                list.Clear();
                for (int i = 0; i < count; i++)
                {
                    list.Add(ReadDataObject(s.ElementType));
                }
            }
        }


        public ResourceDescriptor ReadResource(out Guid id)
        {
            id = ReadGuid();
            string name = ReadString();
            int len = ReadInt32();
            byte[] data = ReadBytes(len);
            
            return new ResourceDescriptor(name)
            {
                Data = data
            };
        }

        public object ReadResourceObject()
        {
            long position = Reader.BaseStream.Position;
            int len = 0;
            Guid id = default;
            string resourceId = default;
            try
            {
                id = ReadGuid();
                resourceId = ReadString();
                len = ReadInt32();
                Type type = U.Tm[id];
                object obj = U.Get(type);
                LoadProperties(obj);
                return obj;
            }
            catch (Exception e)
            {
                U.LogError($"Failed read resource object: id: {id}\r\nresourceId: {resourceId}");
                U.LogError(e);
                Reader.BaseStream.Seek(position + len + 24, SeekOrigin.Begin);
            }

            return null;
        }
    }
}