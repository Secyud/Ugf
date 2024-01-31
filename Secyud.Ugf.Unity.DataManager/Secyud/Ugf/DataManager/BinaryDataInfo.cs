using System;
using System.IO;
using Secyud.Ugf.Abstraction;

namespace Secyud.Ugf.DataManager
{
    public class BinaryDataInfo : IHasName, IHasDescription, IHasId<int>, IArchivable
    {
        public int Id { get; set; }
        public Guid Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Type);
            writer.WriteString(Name);
            writer.WriteString(Description);
            writer.Write(Data.Length);
            writer.Write(Data);
        }

        public void Load(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Type = reader.ReadGuid();
            Name = reader.ReadString();
            Description = reader.ReadString();
            int length = reader.ReadInt32();
            Data = reader.ReadBytes(length);
        }

        public object GetObject()
        {
            object ret = TypeManager.Instance[Type].CreateInstance();

            if (Data is not null)
            {
                using MemoryStream stream = new(Data);
                using BinaryReader reader = new(stream);
                reader.DeserializeResource(ret);
            }

            return ret;
        }

        public void SetObject(object obj)
        {
            using MemoryStream stream = new();
            using BinaryWriter reader = new(stream);
            reader.SerializeResource(obj);
            Data = stream.ToArray();
        }
    }
}