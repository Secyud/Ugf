using System;
using System.Collections.Generic;
using System.Reflection;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public enum EditStyle : byte
    {
        Default,
        FlagOrMemo
    }

    public enum DataType : byte
    {
        Archived,
        Initialed,
        Ignored
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SAttribute : Attribute
    {
        private static readonly Dictionary<Type, FieldType> Map = new()
        {
            [typeof(bool)] = FieldType.Bool,
            [typeof(byte)] = FieldType.UInt8,
            [typeof(ushort)] = FieldType.UInt16,
            [typeof(uint)] = FieldType.UInt32,
            [typeof(ulong)] = FieldType.UInt64,
            [typeof(sbyte)] = FieldType.Int8,
            [typeof(short)] = FieldType.Int16,
            [typeof(int)] = FieldType.Int32,
            [typeof(long)] = FieldType.Int64,
            [typeof(float)] = FieldType.Single,
            [typeof(double)] = FieldType.Double,
            [typeof(decimal)] = FieldType.Decimal,
            [typeof(string)] = FieldType.String,
            [typeof(Guid)] = FieldType.Guid,
        };

        public short ID { get; set; }
        public DataType DataType { get; set; }
        public EditStyle Style { get; set; }
        public FieldInfo Info { get; private set; }
        public FieldType Type { get; private set; }

        public void SetPropertyType(FieldInfo info)
        {
            if (Info is not null) return;
            Info = info;
            Map.TryGetValue(info.FieldType, out FieldType type);
            Type = type;
        }

        public object GetValue(object obj)
        {
            return Info.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            Info.SetValue(obj, value);
        }

        private object GetDefault()
        {
            return Info.FieldType.IsValueType ? Activator.CreateInstance(Info.FieldType) : null;
        }

        public void Write(object value, IArchiveWriter writer)
        {
            value ??= GetDefault();

            switch (Type)
            {
                case FieldType.Bool:
                    writer.Write((bool)value);
                    break;
                case FieldType.UInt8:
                    writer.Write((byte)value);
                    break;
                case FieldType.UInt16:
                    writer.Write((ushort)value);
                    break;
                case FieldType.UInt32:
                    writer.Write((uint)value);
                    break;
                case FieldType.UInt64:
                    writer.Write((ulong)value);
                    break;
                case FieldType.Int8:
                    writer.Write((sbyte)value);
                    break;
                case FieldType.Int16:
                    writer.Write((short)value);
                    break;
                case FieldType.Int32:
                    writer.Write((int)value);
                    break;
                case FieldType.Int64:
                    writer.Write((long)value);
                    break;
                case FieldType.Single:
                    writer.Write((float)value);
                    break;
                case FieldType.Double:
                    writer.Write((double)value);
                    break;
                case FieldType.Decimal:
                    writer.Write((decimal)value);
                    break;
                case FieldType.String:
                    writer.Write((string)value ?? string.Empty);
                    break;
                case FieldType.Guid:
                    writer.Write((Guid)value);
                    break;
                case FieldType.Object:
                    writer.WriteNullable(value);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public object Read(IArchiveReader reader)
        {
            object value = Type switch
            {
                FieldType.Bool    => reader.ReadBoolean(),
                FieldType.UInt8   => reader.ReadByte(),
                FieldType.UInt16  => reader.ReadUInt16(),
                FieldType.UInt32  => reader.ReadUInt32(),
                FieldType.UInt64  => reader.ReadUInt64(),
                FieldType.Int8    => reader.ReadSByte(),
                FieldType.Int16   => reader.ReadInt16(),
                FieldType.Int32   => reader.ReadInt32(),
                FieldType.Int64   => reader.ReadInt64(),
                FieldType.Single  => reader.ReadSingle(),
                FieldType.Double  => reader.ReadDouble(),
                FieldType.Decimal => reader.ReadDecimal(),
                FieldType.String  => reader.ReadString(),
                FieldType.Guid    => reader.ReadGuid(),
                FieldType.Object  => reader.ReadNullable<object>(),
                _                 => new NotImplementedException("Type not support!")
            };

            value ??= GetDefault();

            return value;
        }
    }
}