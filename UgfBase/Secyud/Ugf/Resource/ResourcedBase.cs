using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
    public abstract class ResourcedBase : ICloneable
    {
        private ClassDescriptor _classDescriptor;
        private ClassDescriptor ClassDescriptor => _classDescriptor ??= Og.ClassManager[this.GetTypeId()];

        public ResourceDescriptor Descriptor { get; private set; }


        public void InitSetting([NotNull] ResourceDescriptor descriptor)
        {
            Descriptor = descriptor;
            ClassDescriptor.Init(this, Descriptor, false);
            SetDefaultValue();
        }

        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(Descriptor.TemplateType.GetId());
            writer.Write(Descriptor.Name);
            ClassDescriptor.Write(this, writer);
        }

        public virtual void Load(BinaryReader reader)
        {
            Type templateType = Og.ClassManager[reader.ReadGuid()].Type;
            string name = reader.ReadString();
            Descriptor = Og.InitializeManager.GetResource(templateType, name);
            ClassDescriptor.Read(this, reader);
            ClassDescriptor.Init(this, Descriptor, true);
        }

        public object Clone()
        {
            if (Og.ClassManager.ConstructSame(this) is not ResourcedBase resourcedBase)
            {
                Debug.LogWarning("resourced base clone failed. Use MemberwiseClone Instead!");
                return MemberwiseClone();
            }

            resourcedBase.Descriptor = Descriptor;
            ClassDescriptor.CopyTo(this, resourcedBase);
            resourcedBase.SetDefaultValue();
            return resourcedBase;
        }

        protected virtual void SetDefaultValue()
        {
        }
    }
}