using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class PropertyDescriptor
    {
        public readonly SAttribute[] InitialedProperties;
        public readonly SAttribute[] ArchiveProperties;
        public readonly SAttribute[] IgnoredProperties;

        public PropertyDescriptor(Type type)
        {
            FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);


            List<SAttribute> initialed;
            List<SAttribute> archive;
            List<SAttribute> ignored;

            if (type.BaseType != typeof(object))
            {
                PropertyDescriptor baseDescriptor = U.I.GetProperty(type.BaseType);
                initialed = baseDescriptor.InitialedProperties.ToList();
                archive = baseDescriptor.ArchiveProperties.ToList();
                ignored = baseDescriptor.IgnoredProperties.ToList();
            }
            else
            {
                initialed = new List<SAttribute>();
                archive = new List<SAttribute>();
                ignored = new List<SAttribute>();
            }
        
            foreach (FieldInfo info in infos)
            {
                SAttribute attribute = info.GetCustomAttribute<SAttribute>();

                if (attribute is null) continue;

                attribute.SetPropertyType(info,type);

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
            {
                object value = property.GetValue(o);
                if (property.Info.IsInitOnly)
                {
                    PropertyDescriptor subDescriptor = U.Factory.InitializeManager.GetProperty(value.GetType());
                    writer.SaveProperties(subDescriptor.ArchiveProperties, value);
                }
                else
                    property.Write(property.GetValue(o), writer);
            }
        }

        public void Read(object obj, IArchiveReader reader)
        {
            foreach (SAttribute property in ArchiveProperties)
                if (property.Info.IsInitOnly)
                {
                    object value = property.GetValue(obj);
                    PropertyDescriptor subDescriptor = U.Factory.InitializeManager.GetProperty(value.GetType());
                    reader.LoadProperty(subDescriptor.ArchiveProperties, obj);
                }
                else
                    property.SetValue(obj, property.Read(reader));
        }
    }
}