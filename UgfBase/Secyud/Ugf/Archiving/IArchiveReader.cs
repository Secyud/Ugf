using System;
using System.Collections.Generic;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public interface IArchiveReader
    {
        bool ReadBoolean();
        byte ReadByte();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
        sbyte ReadSByte();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        float ReadSingle();
        double ReadDouble();
        decimal ReadDecimal();
        string ReadString();
        byte[] ReadBytes(int length);
        Guid ReadGuid();
        void ReadList<T>(IList<T> value)where T : class;
        TObject Read<TObject>() where TObject : class;
        TObject ReadNullable<TObject>() where TObject : class;
        public object Read(FieldType type);
        public object ReadChangeable(FieldType fieldType);
        void LoadProperties(SAttribute[] attributes, object value);
    }
}