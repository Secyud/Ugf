using System;
using System.Collections;
using System.Collections.Generic;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public interface IArchiveWriter
    {
        void Write(bool value);
        void Write(byte value);
        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);
        void Write(sbyte value);
        void Write(short value);
        void Write(int value);
        void Write(long value);
        void Write(float value);
        void Write(double value);
        void Write(decimal value);
        void Write(string value);
        void Write(byte[] value);
        void Write(Guid value);
        void Write(object value);
        void WriteList<T>(IList<T> value)where T : class;
        void WriteNullable(object value);
        void Write(object value, FieldType type);
        void WriteChangeable(object value,FieldType fieldType);
        void SaveProperties(SAttribute[] attributes, object value);
    }
}