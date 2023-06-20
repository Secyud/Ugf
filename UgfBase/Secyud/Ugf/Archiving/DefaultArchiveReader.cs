using System;
using System.IO;
using System.Threading.Tasks;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Archiving;

public class DefaultArchiveReader : IArchiveReader, IDisposable
{
    private readonly IDependencyProvider _provider;
    private readonly BinaryReader _reader;

    public DefaultArchiveReader(Stream stream, IDependencyProvider provider)
    {
        _provider = provider;
        _reader = new BinaryReader(stream);
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    public bool ReadBoolean()
    {
        return _reader.ReadBoolean();
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public ushort ReadUInt16()
    {
        return _reader.ReadUInt16();
    }

    public uint ReadUInt32()
    {
        return _reader.ReadUInt32();
    }

    public ulong ReadUInt64()
    {
        return _reader.ReadUInt64();
    }

    public sbyte ReadSByte()
    {
        return _reader.ReadSByte();
    }

    public short ReadInt16()
    {
        return _reader.ReadInt16();
    }

    public int ReadInt32()
    {
        return _reader.ReadInt32();
    }

    public long ReadInt64()
    {
        return _reader.ReadInt64();
    }

    public float ReadSingle()
    {
        return _reader.ReadSingle();
    }

    public double ReadDouble()
    {
        return _reader.ReadDouble();
    }

    public decimal ReadDecimal()
    {
        return _reader.ReadDecimal();
    }

    public string ReadString()
    {
        return _reader.ReadString();
    }

    public byte[] ReadBytes(int length)
    {
        return _reader.ReadBytes(length);
    }

    public Guid ReadGuid()
    {
        return new Guid(ReadBytes(16));
    }

    public TObject Read<TObject>() where TObject : class
    {
        Type type = TypeIdMapper.GetType(ReadGuid());
        object obj = _provider.Get(type);
        if (obj is IArchivable archivable)
            archivable.Load(this);
        return obj as TObject;
    }

    public TObject ReadNullable<TObject>() where TObject : class
    {
        return ReadBoolean() ? Read<TObject>() : null;
    }
}