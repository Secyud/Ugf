using System;
using Secyud.Ugf.Archiving;

namespace Secyud.Ugf.DataManager
{
    public abstract class DataObject : IArchivable,ICloneable
    {
        protected string ObjectName;
        protected Guid TemplateType;

        public static TTemplate Create<TTemplate>(string name) where TTemplate : DataObject
        {
            InitializeManager manager = U.Factory.InitializeManager;
            ResourceDescriptor resource = manager.GetDescriptor<TTemplate>(name);
            Type type = TypeIdMapper.GetType(resource.TypeId);
            object obj = U.Get(type);
            PropertyDescriptor property = manager.GetProperty(type);
            resource.ReadArchived(obj, property);
            resource.ReadInitialed(obj, property);
            if (obj is not TTemplate ret) return null;
            ret.ObjectName = name;
            ret.TemplateType = resource.TypeId;
            return ret;
        }

        public virtual void Save(IArchiveWriter writer)
        {
            writer.Write(ObjectName);
            writer.Write(TemplateType);
        
            U.AutoSaveObject(this, writer);
        }

        public virtual void Load(IArchiveReader reader)
        {
            ObjectName=reader.ReadString();
            TemplateType=reader.ReadGuid();
        
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
}