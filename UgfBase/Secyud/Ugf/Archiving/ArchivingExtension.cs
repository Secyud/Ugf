using System;
using Secyud.Ugf.DataManager;
using UnityEngine;

namespace Secyud.Ugf.Archiving
{
    public static class ArchivingExtension
    {
        public static object ReadField(this IArchiveReader reader, FieldType type)
        {
            return type switch
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
                _                 => new NotSupportedException("Type not support!")
            };
        }
        public static void WriteField(this IArchiveWriter writer,object value, FieldType type)
        {
            switch (type)
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

        
        
        public static void SaveResource(this IDataResource resource, IArchiveWriter writer)
        {
            writer.Write(resource.ResourceId);
        }
        public static void LoadResource(this IDataResource shown, IArchiveReader reader)
        {
            string name = reader.ReadString();
            TypeDescriptor property = U.Tm.GetProperty(shown.GetType());
            if (property.Resources.TryGetValue(name, out ResourceDescriptor resource))
            {
                resource.WriteToObject(shown);
            }
            else
            {
                Debug.LogError($"Cannot get item from resource. Type: {shown.GetType()}, Name: {name}.");
            }
        }
    }
}