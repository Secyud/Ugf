using System.IO;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public class ResourceDescriptor
    {
        public string Name { get; }
        public byte[][] Data { get; }

        public ResourceDescriptor(string name)
        {
            Name = name;
            Data = new byte[4][];
        }

        public void Save(IArchiveWriter writer)
        {
            for (int i = 0; i < 4; i++)
            {
                byte[] data = Data[i];
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
        }

        public void Load(IArchiveReader reader)
        {
            for (int i = 0; i < 4; i++)
            {
                if (reader.ReadBoolean())
                {
                    int len = reader.ReadInt32();
                    Data[i] = reader.ReadBytes(len);
                }
                else
                {
                    Data[i] = null;
                }
            }
        }

        public void WriteToObject(object obj, DataLevel level)
        {
            using MemoryStream stream = new(Data[(int)level]);
            using DataReader reader = new(stream);
            reader.LoadProperties(obj.GetType(), obj, level);
        }

        public void ReadFromObject(object obj, DataLevel level)
        {
            using MemoryStream stream = new();
            using DataWriter writer = new(stream);
            writer.SaveProperties(obj.GetType(), obj, level);
            Data[(int)level] = stream.ToArray();
        }
    }
}