using System;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager;

public abstract class DataObject : IArchivable,ICloneable
{
    [S(ID = -1)] protected string ObjectName;
    [S(ID = -2)] protected Guid TemplateType;

    public static TTemplate Create<TTemplate>(string name) where TTemplate : DataObject
    {
        InitializeManager manager = U.Factory.InitializeManager;
        ResourceDescriptor resource = manager.GetDescriptor<TTemplate>(name);
        Type type = TypeIdMapper.GetType(resource.TypeId);
        object obj = U.Get(type);
        PropertyDescriptor property = manager.GetProperty(type);
        resource.ReadArchived(obj, property);
        resource.ReadInitialed(obj, property);
        return obj as TTemplate;
    }

    public virtual void Save(IArchiveWriter writer)
    {
        U.AutoSaveObject(this, writer);
    }

    public virtual void Load(IArchiveReader reader)
    {
        U.AutoLoadObject(this, reader);

        InitializeManager manager = U.Factory.InitializeManager;
        ResourceDescriptor resource = manager.TryGetDescriptor(
            TypeIdMapper.GetType(TemplateType), ObjectName);
        if (resource is not null)
        {
            PropertyDescriptor property = manager.GetProperty(GetType());
            resource.ReadInitialed(this, property);
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}