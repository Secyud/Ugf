using System;
using System.IO;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class ResourceDescriptor : IArchivable
    {
        public string Name { get; }
        public Guid TypeId { get; }
        public Type TemplateType { get; }
        public byte[] ArchivedData { get; private set; }
        public byte[] InitialedData { get; private set; }
        public byte[] IgnoredData { get; private set; }

        public ResourceDescriptor(string name, Guid typeId, Type templateType)
        {
            Name = name;
            TypeId = typeId;
            TemplateType = templateType;
        }

        public void Save(IArchiveWriter writer)
        {
            writer.Write(ArchivedData.Length);
            writer.Write(ArchivedData);
            writer.Write(InitialedData.Length);
            writer.Write(InitialedData);
            writer.Write(IgnoredData.Length);
            writer.Write(IgnoredData);
        }

        public void Load(IArchiveReader reader)
        {
            int len = reader.ReadInt32();
            ArchivedData = reader.ReadBytes(len);
            len = reader.ReadInt32();
            InitialedData = reader.ReadBytes(len);
            len = reader.ReadInt32();
            IgnoredData = reader.ReadBytes(len);
        }

        void SetProperties(object obj, SAttribute[] properties, byte[] data)
        {
            using MemoryStream stream = new(data);
            using DefaultArchiveReader reader = new(stream, U.Factory.Application.DependencyManager);
            foreach (SAttribute attribute in properties)
                attribute.Info.SetValue(obj, attribute.Read(reader));
        }

        byte[] GetProperties(object obj, SAttribute[] properties)
        {
            using MemoryStream stream = new();
            using DefaultArchiveWriter writer = new(stream);
            foreach (SAttribute attribute in properties)
                attribute.Write(attribute.Info.GetValue(obj), writer);

            return stream.ToArray();
        }

        public void ReadArchived(object obj, PropertyDescriptor descriptor)
        {
            SetProperties(obj, descriptor.ArchiveProperties, ArchivedData);
        }

        public void ReadInitialed(object obj, PropertyDescriptor descriptor)
        {
            SetProperties(obj, descriptor.InitialedProperties, InitialedData);
        }

        public void ReadIgnored(object obj, PropertyDescriptor descriptor)
        {
            SetProperties(obj, descriptor.IgnoredProperties, IgnoredData);
        }

        public void WriteArchived(object obj, PropertyDescriptor descriptor)
        {
            ArchivedData = GetProperties(obj, descriptor.ArchiveProperties);
        }

        public void WriteInitialed(object obj, PropertyDescriptor descriptor)
        {
            InitialedData = GetProperties(obj, descriptor.InitialedProperties);
        }

        public void WriteIgnored(object obj, PropertyDescriptor descriptor)
        {
            IgnoredData = GetProperties(obj, descriptor.IgnoredProperties);
        }
    }
}