using System;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveReader : IArchiveReader, IDisposable
    {
        protected readonly BinaryReader Reader;

        public DefaultArchiveReader(Stream stream)
        {
            Reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            Reader.Dispose();
        }

        public bool ReadBoolean()
        {
            return Reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return Reader.ReadByte();
        }

        public ushort ReadUInt16()
        {
            return Reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return Reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return Reader.ReadUInt64();
        }

        public sbyte ReadSByte()
        {
            return Reader.ReadSByte();
        }

        public short ReadInt16()
        {
            return Reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return Reader.ReadInt64();
        }

        public float ReadSingle()
        {
            return Reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return Reader.ReadDouble();
        }

        public decimal ReadDecimal()
        {
            return Reader.ReadDecimal();
        }

        public string ReadString()
        {
            return Reader.ReadString();
        }

        public byte[] ReadBytes(int length)
        {
            return Reader.ReadBytes(length);
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
                value.Add(ReadObject<T>());
            }
        }

        public TObject ReadObject<TObject>() where TObject : class
        {
            Type type = TypeManager.Instance[ReadGuid()];
            object obj = U.Get(type);
            if (obj is IArchivable archivable)
                archivable.Load(this);
            return obj as TObject;
        }

        public TObject ReadNullable<TObject>() where TObject : class
        {
            return ReadBoolean() ? ReadObject<TObject>() : null;
        }
    }
}