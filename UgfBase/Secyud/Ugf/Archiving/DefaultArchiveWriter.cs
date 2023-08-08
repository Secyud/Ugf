using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.Archiving
{
    public class DefaultArchiveWriter : IArchiveWriter, IDisposable, IAsyncDisposable
    {
        private readonly BinaryWriter _writer;

        public DefaultArchiveWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _writer.DisposeAsync();
        }

        public void Write(bool value)
        {
            _writer.Write(value);
        }

        public void Write(byte value)
        {
            _writer.Write(value);
        }

        public void Write(ushort value)
        {
            _writer.Write(value);
        }

        public void Write(uint value)
        {
            _writer.Write(value);
        }

        public void Write(ulong value)
        {
            _writer.Write(value);
        }

        public void Write(sbyte value)
        {
            _writer.Write(value);
        }

        public void Write(short value)
        {
            _writer.Write(value);
        }

        public void Write(int value)
        {
            _writer.Write(value);
        }

        public void Write(long value)
        {
            _writer.Write(value);
        }

        public void Write(float value)
        {
            _writer.Write(value);
        }

        public void Write(double value)
        {
            _writer.Write(value);
        }

        public void Write(decimal value)
        {
            _writer.Write(value);
        }

        public void Write(string value)
        {
            _writer.Write(value);
        }

        public void Write(byte[] value)
        {
            _writer.Write(value);
        }

        public void Write(Guid id)
        {
            Write(id.ToByteArray());
        }

        public void Write(object value)
        {
            Guid typeId = TypeIdMapper.GetId(value.GetType());

            if (typeId == Guid.Empty)
                throw new UgfInitializationException(
                    $"Type {value.GetType()} doesn't have id!");

            Write(typeId);
            if (value is IArchivable archivable)
                archivable.Save(this);
        }

        public void WriteList<T>(IList<T> value) where T : class
        {
            Write(value.Count);
            foreach (T t in value)
            {
                Write(t);
            }
        }

        public void WriteNullable(object value)
        {
            if (value is null)
                Write(false);
            else
            {
                Write(true);
                Write(value);
            }
        }

        public void Write(object value, FieldType type)
        {
            switch (type)
            {
                case FieldType.Bool:
                    Write((bool)value);
                    break;
                case FieldType.UInt8:
                    Write((byte)value);
                    break;
                case FieldType.UInt16:
                    Write((ushort)value);
                    break;
                case FieldType.UInt32:
                    Write((uint)value);
                    break;
                case FieldType.UInt64:
                    Write((ulong)value);
                    break;
                case FieldType.Int8:
                    Write((sbyte)value);
                    break;
                case FieldType.Int16:
                    Write((short)value);
                    break;
                case FieldType.Int32:
                    Write((int)value);
                    break;
                case FieldType.Int64:
                    Write((long)value);
                    break;
                case FieldType.Single:
                    Write((float)value);
                    break;
                case FieldType.Double:
                    Write((double)value);
                    break;
                case FieldType.Decimal:
                    Write((decimal)value);
                    break;
                case FieldType.String:
                    Write((string)value ?? string.Empty);
                    break;
                case FieldType.Guid:
                    Write((Guid)value);
                    break;
                case FieldType.Object:
                    WriteNullable(value);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void WriteChangeable(object value, FieldType fieldType)
        {
            if (FieldType.Object != fieldType)
                Write(value, fieldType);
            else if (value is null)
                Write(false);
            else
            {
                Write(true);

                Type type = value.GetType();
                Write(TypeIdMapper.GetId(type));

                PropertyDescriptor property = U.Factory.InitializeManager.GetProperty(type);

                SaveProperties(property.ArchiveProperties, value);
                SaveProperties(property.InitialedProperties, value);
                SaveProperties(property.IgnoredProperties, value);
            }
        }

        public void SaveProperties(SAttribute[] attributes, object value)
        {
            Write(attributes.Length);

            foreach (SAttribute attr in attributes)
            {
                Write(attr.ID);
                if (attr.ReadOnly)
                {
                    object obj = attr.GetValue(value);

                    if (obj is IList list)
                    {
                        Write(list.Count);
                        foreach (object o in list)
                            Write(o);
                    }
                    else
                    {
                        PropertyDescriptor property =
                            U.Factory.InitializeManager.GetProperty(obj.GetType());

                        SaveProperties(property.ArchiveProperties, obj);
                    }
                }
                else
                {
                    Write((byte)attr.Type);
                    WriteChangeable(attr.GetValue(value), attr.Type);
                }
            }
        }
    }
}