using System.IO;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class ResourceDescriptor
    {
        public string Name { get; }
        public byte[] Data { get; set; }

        public ResourceDescriptor(string name)
        {
            Name = name;
        }

        public void Save(IArchiveWriter writer)
        {
            byte[] data = Data;
            if (data is not null)
            {
                writer.Write(true);
                writer.Write(data.Length);
                writer.Write(data);
            }
            else
            {
                writer.Write(false);
            }
        }

        public void Load(IArchiveReader reader)
        {
            if (reader.ReadBoolean())
            {
                int len = reader.ReadInt32();
                Data = reader.ReadBytes(len);
            }
            else
            {
                Data = null;
            }
        }

        public void WriteToObject(object obj)
        {
            using MemoryStream stream = new(Data);
            using DataReader reader = new(stream);
            reader.LoadProperties(obj);
        }

        public void ReadFromObject(object obj)
        {
            using MemoryStream stream = new();
            using DataWriter writer = new(stream);
            writer.SaveProperties(obj);
            Data = stream.ToArray();
        }
    }
}