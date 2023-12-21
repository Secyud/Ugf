using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveReader :BinaryReader, IArchiveReader, IDisposable
    {
        public DefaultArchiveReader([NotNull] Stream input) : base(input)
        {
        }

        public DefaultArchiveReader([NotNull] Stream input, [NotNull] Encoding encoding) : base(input, encoding)
        {
        }

        public DefaultArchiveReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
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
            TypeDescriptor descriptor = TypeManager.Instance[ReadGuid()];
            object obj = U.Get(descriptor.Type);
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