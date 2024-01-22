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
            DataLoader loader = new(reader);
            loader.LoadProperties(obj);
        }

        public void SaveObject(object obj)
        {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);
            DataSaver saver = new(writer);
            saver.SaveProperties(obj);
            Data = stream.ToArray();
        }
    }
}