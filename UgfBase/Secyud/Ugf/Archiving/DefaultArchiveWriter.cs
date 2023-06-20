using System;
using System.IO;
using System.Threading.Tasks;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Archiving;

public class DefaultArchiveWriter : IArchiveWriter, IDisposable,IAsyncDisposable
{
    private readonly BinaryWriter _writer;

    public DefaultArchiveWriter(Stream stream)
    {
        _writer = new BinaryWriter(stream);
    }

    public void Dispose()
    {
        _writer.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _writer.DisposeAsync();
    }

    public void Write(bool value)
    {
        _writer.Write(value);
    }

    public void Write(byte value)
    {
        _writer.Write(value);
    }

    public void Write(ushort value)
    {
        _writer.Write(value);
    }

    public void Write(uint value)
    {
        _writer.Write(value);
    }

    public void Write(ulong value)
    {
        _writer.Write(value);
    }

    public void Write(sbyte value)
    {
        _writer.Write(value);
    }

    public void Write(short value)
    {
        _writer.Write(value);
    }

    public void Write(int value)
    {
        _writer.Write(value);
    }

    public void Write(long value)
    {
        _writer.Write(value);
    }

    public void Write(float value)
    {
        _writer.Write(value);
    }

    public void Write(double value)
    {
        _writer.Write(value);
    }

    public void Write(decimal value)
    {
        _writer.Write(value);
    }

    public void Write(string value)
    {
        _writer.Write(value);
    }

    public void Write(byte[] value)
    {
        _writer.Write(value);
    }

    public void Write(Guid id)
    {
        Write(id.ToByteArray());
    }

    public void Write(object value)
    {
        Guid typeId = TypeIdMapper.GetId(value.GetType());

        if (typeId == Guid.Empty)
            throw new UgfInitializationException(
                $"Type {value.GetType()} doesn't have id!");

        Write(typeId);
        if (value is IArchivable archivable)
            archivable.Save(this);
    }

    public void WriteNullable(object value)
    {
        if (value is null)
            Write(false);
        else
        {
            Write(true);
            Write(value);
        }
    }
}