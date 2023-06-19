using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.Resource;

public class PropertyDescriptor
{
    public readonly SAttribute[] TotalProperties;
    public readonly SAttribute[] ArchiveProperties;

    public PropertyDescriptor(Type type)
    {
        PropertyInfo[] infos = type.GetProperties();
        List<SAttribute> attributes = new();
        foreach (PropertyInfo info in infos)
        {
            SAttribute attribute = info.GetCustomAttribute<SAttribute>();

            if (attribute is null) continue;

            attribute.SetPropertyType(info);
            attributes.Add(attribute);
        }

        TotalProperties = attributes.OrderBy(u => u.ID).ToArray();
        ArchiveProperties = TotalProperties.Where(u => !u.AlwaysSet).OrderBy(u => u.ID).ToArray();
    }

    private static void Write(object o, SAttribute property, IArchiveWriter writer)
    {
        object v = property.Info.GetValue(o);
        switch (property.Type)
        {
            case PropertyType.Bool:
                writer.Write((bool)v);
                break;
            case PropertyType.UInt8:
                writer.Write((byte)v);
                break;
            case PropertyType.UInt16:
                writer.Write((ushort)v);
                break;
            case PropertyType.UInt32:
                writer.Write((uint)v);
                break;
            case PropertyType.UInt64:
                writer.Write((ulong)v);
                break;
            case PropertyType.Int8:
                writer.Write((sbyte)v);
                break;
            case PropertyType.Int16:
                writer.Write((short)v);
                break;
            case PropertyType.Int32:
                writer.Write((int)v);
                break;
            case PropertyType.Int64:
                writer.Write((long)v);
                break;
            case PropertyType.Single:
                writer.Write((float)v);
                break;
            case PropertyType.Double:
                writer.Write((double)v);
                break;
            case PropertyType.Decimal:
                writer.Write((decimal)v);
                break;
            case PropertyType.String:
                writer.Write((string)v);
                break;
            case PropertyType.Guid:
                writer.Write((Guid)v);
                break;
            case PropertyType.Object:
                writer.WriteNullable(v);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void Read(object obj, SAttribute property, IArchiveReader reader)
    {
        property.Info.SetValue(obj, property switch
        {
            { Type: PropertyType.Bool }    => reader.ReadBoolean(),
            { Type: PropertyType.UInt8 }   => reader.ReadByte(),
            { Type: PropertyType.UInt16 }  => reader.ReadUInt16(),
            { Type: PropertyType.UInt32 }  => reader.ReadUInt32(),
            { Type: PropertyType.UInt64 }  => reader.ReadUInt64(),
            { Type: PropertyType.Int8 }    => reader.ReadSByte(),
            { Type: PropertyType.Int16 }   => reader.ReadInt16(),
            { Type: PropertyType.Int32 }   => reader.ReadInt32(),
            { Type: PropertyType.Int64 }   => reader.ReadInt64(),
            { Type: PropertyType.Single }  => reader.ReadSingle(),
            { Type: PropertyType.Double }  => reader.ReadDouble(),
            { Type: PropertyType.Decimal } => reader.ReadDecimal(),
            { Type: PropertyType.String }  => reader.ReadString(),
            { Type: PropertyType.Guid }    => reader.ReadGuid(),
            { Type: PropertyType.Object }  => reader.ReadNullable<object>(),
            _                              => default
        });
    }

    public void Init(object obj, ResourceDescriptor descriptor, bool archiving)
    {
        SAttribute[] properties = archiving ? ArchiveProperties : TotalProperties;
        short i = 0;
        foreach ((short id, object data) in descriptor.Data)
        {
            while (properties[i].ID < id)
            {
                if (++i >= properties.Length)
                    return;
            }

            SAttribute property = properties[i];
            if (property.ID == id)
                property.Info.SetValue(obj, data);
        }
    }

    public void Write(object o, IArchiveWriter writer)
    {
        foreach (SAttribute property in ArchiveProperties)
            Write(o, property, writer);
    }

    public void Read(object obj, IArchiveReader reader)
    {
        foreach (SAttribute property in ArchiveProperties)
            Read(obj, property, reader);
    }
}