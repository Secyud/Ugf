using System;
using System.Collections.Generic;

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
        void WriteObject(object value);
        void WriteListable<T>(IList<T> value)where T : class;
        void WriteNullable(object value);
    }
}