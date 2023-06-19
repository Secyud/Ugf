using System;
using System.IO;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Resource;

namespace Secyud.Ugf.Archiving;

public class DefaultArchiveWriter : IArchiveWriter, IDisposable
{
    private readonly BinaryWriter _writer;
    private readonly FileStream _file;

    public DefaultArchiveWriter(string path)
    {
        _file = File.OpenWrite(path);
        _writer = new BinaryWriter(_file);
    }

    public void Dispose()
    {
        _writer.Dispose();
        _file.Dispose();
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
        Guid typeId = TypeMap.GetId(value.GetType());

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