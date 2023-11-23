using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveWriter : IArchiveWriter, IDisposable, IAsyncDisposable
    {
        protected readonly BinaryWriter Writer;

        public DefaultArchiveWriter(Stream stream)
        {
            Writer = new BinaryWriter(stream);
        }

        public void Dispose()
        {
            Writer.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return Writer.DisposeAsync();
        }

        public void Write(bool value)
        {
            Writer.Write(value);
        }

        public void Write(byte value)
        {
            Writer.Write(value);
        }

        public void Write(ushort value)
        {
            Writer.Write(value);
        }

        public void Write(uint value)
        {
            Writer.Write(value);
        }

        public void Write(ulong value)
        {
            Writer.Write(value);
        }

        public void Write(sbyte value)
        {
            Writer.Write(value);
        }

        public void Write(short value)
        {
            Writer.Write(value);
        }

        public void Write(int value)
        {
            Writer.Write(value);
        }

        public void Write(long value)
        {
            Writer.Write(value);
        }

        public void Write(float value)
        {
            Writer.Write(value);
        }

        public void Write(double value)
        {
            Writer.Write(value);
        }

        public void Write(decimal value)
        {
            Writer.Write(value);
        }

        public void Write(string value)
        {
            Writer.Write(value);
        }

        public void Write(byte[] value)
        {
            Writer.Write(value);
        }

        public void Write(Guid id)
        {
            Write(id.ToByteArray());
        }

        public void WriteObject(object value)
        {
            Write(value.GetType().GUID);
            if (value is IArchivable archivable)
                archivable.Save(this);
        }
        
        public void WriteNullable(object value)
        {
            if (value is null)
                Write(false);
            else
            {
                Write(true);
                WriteObject(value);
            }
        }

        public void WriteListable<T>(IList<T> value) where T : class
        {
            Write(value.Count);
            foreach (T t in value)
            {
                WriteObject(t);
            }
        }

    }
}