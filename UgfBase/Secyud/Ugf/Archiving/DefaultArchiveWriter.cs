using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveWriter : BinaryWriter, IArchiveWriter
    {
        public DefaultArchiveWriter()
        {
        }

        public DefaultArchiveWriter(Stream stream)
            : base(stream)
        {
        }


        public DefaultArchiveWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }


        public DefaultArchiveWriter(Stream stream, Encoding encoding, bool leaveOpen)
            : base(stream, encoding, leaveOpen)
        {
        }

        public override void Write([CanBeNull]string value)
        {
            base.Write(value ?? string.Empty);
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