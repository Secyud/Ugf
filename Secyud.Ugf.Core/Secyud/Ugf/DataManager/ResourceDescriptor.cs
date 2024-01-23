using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Secyud.Ugf.Logging;

namespace Secyud.Ugf.DataManager
{
    public class ResourceDescriptor
    {
        public int Id { get; private set; }
        public byte[] Data { get; set; }

        public void Save(BinaryWriter writer)
        {
            if (Data is null)
            {
                UgfLogger.LogError("Data cannot be null for resource saving!");
                return;
            }

            writer.Write(Id);
            byte[] data = Data;
            writer.Write(data.Length);
            writer.Write(data);
        }

        public void Load(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            int len = reader.ReadInt32();
            Data = reader.ReadBytes(len);
        }

        public void LoadObject(object obj)
        {
            using MemoryStream stream = new(Data);
            using BinaryReader reader = new(stream);
            reader.DeserializeResource(obj);
        }

        public void SaveObject(object obj)
        {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);
            writer.SerializeResource(obj);
            Data = stream.ToArray();
        }
    }
}