using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager;

public class PropertyDescriptor
{
    public readonly SAttribute[] InitialedProperties;
    public readonly SAttribute[] ArchiveProperties;
    public readonly SAttribute[] IgnoredProperties;

    public PropertyDescriptor(Type type)
    {
        FieldInfo[] infos = type.GetFields(
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        List<SAttribute> initialed = new();
        List<SAttribute> archive = new();
        List<SAttribute> ignored = new();
        foreach (FieldInfo info in infos)
        {
            SAttribute attribute = info.GetCustomAttribute<SAttribute>();

            if (attribute is null) continue;

            attribute.SetPropertyType(info);

            switch (attribute.DataType)
            {
                case DataType.Archived:
                    archive.InsertBefore(u => u.ID > attribute.ID, attribute);
                    break;
                case DataType.Initialed:
                    initialed.InsertBefore(u => u.ID > attribute.ID, attribute);
                    break;
                case DataType.Ignored:
                    ignored.InsertBefore(u => u.ID > attribute.ID, attribute);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        IgnoredProperties = ignored.ToArray();
        InitialedProperties = initialed.ToArray();
        ArchiveProperties = archive.ToArray();
    }


    public void Write(object o, IArchiveWriter writer)
    {
        foreach (SAttribute property in ArchiveProperties)
            property.Write(property.GetValue(o), writer);
    }

    public void Read(object obj, IArchiveReader reader)
    {
        foreach (SAttribute property in ArchiveProperties)
            property.SetValue(obj, property.Read(reader));
    }
}