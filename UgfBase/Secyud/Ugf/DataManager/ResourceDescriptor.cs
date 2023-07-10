using System;
using System.IO;
using System.Ugf.Collections.Generic;
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
        
        public void ReadArchived(object obj, PropertyDescriptor descriptor)
        {
            LoadDataFromBytes(obj, descriptor.ArchiveProperties,ArchivedData);
        }

        public void ReadInitialed(object obj, PropertyDescriptor descriptor)
        {
            LoadDataFromBytes(obj, descriptor.InitialedProperties,InitialedData);
        }

        public void ReadIgnored(object obj, PropertyDescriptor descriptor)
        {
            LoadDataFromBytes(obj, descriptor.IgnoredProperties,IgnoredData);
        }

        public void WriteArchived(object obj, PropertyDescriptor descriptor)
        {
            ArchivedData =  SaveDataToBytes(obj, descriptor.ArchiveProperties);
        }

        public void WriteInitialed(object obj, PropertyDescriptor descriptor)
        {
            InitialedData = SaveDataToBytes(obj, descriptor.InitialedProperties);
        }

        public void WriteIgnored(object obj,PropertyDescriptor descriptor)
        {
            IgnoredData = SaveDataToBytes(obj, descriptor.IgnoredProperties);
        }
        
        
        public static byte[] SaveDataToBytes(object obj,SAttribute[] attributes)
        {
            using MemoryStream stream = new();
            using DefaultArchiveWriter writer = new(stream);
            writer.SaveProperties(attributes, obj);
            return stream.ToArray();
        }
        public static void LoadDataFromBytes(object obj,SAttribute[] attributes,byte[] data)
        {
            if (data.IsNullOrEmpty())
                return;
            using MemoryStream stream = new(data);
            using DefaultArchiveReader reader = new(stream);
            reader.LoadProperties(attributes, obj);
        }
    }
}