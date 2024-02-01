using System;
using System.IO;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DataManager
{
    public struct DataResource : IHasId<int>, IArchivable
    {
        public int Id { get; set; }
        public Guid Type { get; set; }
        public byte[] Data { get; set; }


        public void Save(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Type);
            writer.Write(Data.Length);
            writer.Write(Data);
        }

        public void Load(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Type = reader.ReadGuid();
            int length = reader.ReadInt32();
            Data = reader.ReadBytes(length);
        }

        public object GetObject()
        {
            object ret = TypeManager.Instance[Type].CreateInstance();

            FillObject(ret);

            return ret;
        }

        public void FillObject(object obj)
        {
            if (Data is null)
            {
                UgfLogger.LogError($"Data with id {Id} not found for type {Type}.");
                return;
            }

            using MemoryStream stream = new(Data);
            using BinaryReader reader = new(stream);
            reader.DeserializeResource(obj);

            if (obj is IHasResourceId { ResourceId: 0 } r)
            {
                r.ResourceId = Id;
            }
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