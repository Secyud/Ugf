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
            LoadDataFromBytes(obj, DataType.Archived,ArchivedData,descriptor);
        }

        public void ReadInitialed(object obj, PropertyDescriptor descriptor)
        {
            LoadDataFromBytes(obj, DataType.Initialed,InitialedData,descriptor);
        }

        public void ReadIgnored(object obj, PropertyDescriptor descriptor)
        {
            LoadDataFromBytes(obj, DataType.Ignored,IgnoredData,descriptor);
        }

        public void WriteArchived(object obj, PropertyDescriptor descriptor)
        {
            ArchivedData =  SaveDataToBytes(obj, DataType.Archived,descriptor);
        }

        public void WriteInitialed(object obj, PropertyDescriptor descriptor)
        {
            InitialedData = SaveDataToBytes(obj, DataType.Initialed,descriptor);
        }

        public void WriteIgnored(object obj,PropertyDescriptor descriptor)
        {
            IgnoredData = SaveDataToBytes(obj, DataType.Ignored,descriptor);
        }
        
        
        public static byte[] SaveDataToBytes(object obj,DataType dataType,PropertyDescriptor property)
        {
            using MemoryStream stream = new();
            using DefaultArchiveWriter writer = new(stream);

            switch (dataType)
            {
                case DataType.Archived:
                    writer.SaveProperties(property.ArchiveProperties, obj);
                    break;
                case DataType.Initialed:
                    writer.SaveProperties(property.InitialedProperties, obj);
                    break;
                case DataType.Ignored:
                    writer.SaveProperties(property.IgnoredProperties, obj);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }

            return stream.ToArray();
        }
        public static void LoadDataFromBytes(object obj,DataType dataType,byte[] data,PropertyDescriptor property)
        {
            if (data.IsNullOrEmpty())
                return;
            
            using MemoryStream stream = new(data);
            using DefaultArchiveReader reader = new(stream);
            
            switch (dataType)
            {
                case DataType.Archived:
                    reader.LoadProperty(property.ArchiveProperties, obj);
                    break;
                case DataType.Initialed:
                    reader.LoadProperty(property.InitialedProperties, obj);
                    break;
                case DataType.Ignored:
                    reader.LoadProperty(property.IgnoredProperties, obj);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }
        }
    }
}