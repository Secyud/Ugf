using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveReader : IArchiveReader, IDisposable
    {
        private readonly BinaryReader _reader;

        public DefaultArchiveReader(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public decimal ReadDecimal()
        {
            return _reader.ReadDecimal();
        }

        public string ReadString()
        {
            return _reader.ReadString();
        }

        public byte[] ReadBytes(int length)
        {
            return _reader.ReadBytes(length);
        }

        public Guid ReadGuid()
        {
            return new Guid(ReadBytes(16));
        }

        public void ReadList<T>(IList<T> value) where T : class
        {
            int count = ReadInt32();

            for (int i = 0; i < count; i++)
            {
                value.Add(Read<T>());
            }
        }

        public TObject Read<TObject>() where TObject : class
        {
            Type type = TypeIdMapper.GetType(ReadGuid());
            object obj = U.Get(type);
            if (obj is IArchivable archivable)
                archivable.Load(this);
            return obj as TObject;
        }

        public TObject ReadNullable<TObject>() where TObject : class
        {
            return ReadBoolean() ? Read<TObject>() : null;
        }

        public object Read(FieldType type)
        {
            return type switch
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
                FieldType.Object  => ReadNullable<object>(),
                _                 => new NotSupportedException("Type not support!")
            };
        }

        public object ReadChangeable(FieldType fieldType)
        {
            if (FieldType.Object != fieldType)
                return Read(fieldType);

            if (!ReadBoolean())
                return null;

            Guid typeId = ReadGuid();
            Type type = TypeIdMapper.GetType(typeId);
            object ret = U.Get(type);
            PropertyDescriptor property = U.Factory.InitializeManager.GetProperty(type);

            LoadProperties(property.ArchiveProperties, ret);
            LoadProperties(property.InitialedProperties, ret);
            LoadProperties(property.IgnoredProperties, ret);

            return ret;
        }

        public void LoadProperties(SAttribute[] attributes, object value)
        {
            int propertyCount = ReadInt32();
            int aIndex = 0;
            for (int i = 0; i < propertyCount; i++)
            {
                short id = ReadInt16();
                while (attributes[aIndex].ID < id)
                {
                    aIndex++;
                    if (attributes.Length <= aIndex)
                        return;
                }

                SAttribute attr = attributes[aIndex];

                if (attr.ID == id)
                {
                    if (attr.ReadOnly)
                    {
                        object obj = attr.GetValue(value);

                        if (obj is IList list)
                        {
                            list.Clear();
                            int count = ReadInt32();
                            for (int j = 0; j < count; j++)
                                list.Add(Read<object>());
                        }
                        else
                        {
                            PropertyDescriptor property =
                                U.Factory.InitializeManager.GetProperty(obj.GetType());

                            LoadProperties(property.ArchiveProperties, obj);
                        }
                    }
                    else
                    {
                        object obj = ReadChangeable((FieldType)ReadByte());

                        attr.SetValue(value, obj);
                    }
                }

                aIndex++;
            }
        }
    }
}