using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace Secyud.Ugf.Resource
{
    public abstract class ResourcedBase:ICloneable
    {
        private ClassDescriptor _classDescriptor;
        private ClassDescriptor ClassDescriptor => _classDescriptor ??= Og.ClassManager[this.GetTypeId()];
        [R(-1)] public string ResourceName { get; set; }
        public void InitSetting([NotNull] string name)
        {
            ResourceDescriptor resourceDescriptor =
                Og.InitializeManager.GetOrAddResource(name);
            ClassDescriptor.Init(this, resourceDescriptor, false);
            SetDefaultValue(resourceDescriptor);
        }

        public virtual void Save(BinaryWriter writer)
        {
            ClassDescriptor.Write(this, writer);
        }

        public virtual void Load(BinaryReader reader)
        {
            ClassDescriptor.Read(this, reader);
            ResourceDescriptor resourceDescriptor =
                Og.InitializeManager.GetOrAddResource(ResourceName);
            ClassDescriptor.Init(this, resourceDescriptor, true);
            SetDefaultValue(resourceDescriptor);
        }

        public object Clone()
        {
            if (Og.ClassManager.ConstructSame(this) is not ResourcedBase obj)
            {
                Debug.LogWarning("resourced base clone failed. Use MemberwiseClone Instead!");
                return MemberwiseClone();
            }

            ClassDescriptor.CopyTo(this, obj);
            ResourceDescriptor resourceDescriptor =
                Og.InitializeManager.GetOrAddResource(ResourceName);
            obj.SetDefaultValue(resourceDescriptor);
            return obj;
        }

        protected virtual void SetDefaultValue(ResourceDescriptor descriptor)
        {
        }
    }
}