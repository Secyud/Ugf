using Secyud.Ugf.Archiving;
using System;
using System.IO;
using UnityEngine;
using ICloneable = System.ICloneable;

namespace Secyud.Ugf.Resource
{
	public abstract class ResourcedBase : IArchivable,ICloneable
	{
		protected ResourceDescriptor Descriptor { get; private set; }

		private ResourceProperty _property;
		protected ResourceProperty PropertyInfos => _property ??=
			ResourceManager.Instance.GetResourceProperty(GetType());

		public virtual void InitSetting(string name = null)
		{
			if (name.IsNullOrEmpty()) name = GetType().Name;
			Descriptor = ResourceManager.Instance.GetResourceDescriptor(name);
			PropertyInfos.Init(this,Descriptor);
			SetDefaultValue();
		}

		public virtual void Save(BinaryWriter writer)
		{
			PropertyInfos.Write(this, writer);
		}

		public virtual void Load(BinaryReader reader)
		{
			PropertyInfos.Read(this, reader);
			SetDefaultValue();
		}

		protected virtual void SetDefaultValue()
		{
		}

		private void CopyTo(ResourcedBase to)
		{
		}

		public object Clone()
		{
			if (Og.TypeManager.ConstructSame(this) is not ResourcedBase obj)
			{
				Debug.LogWarning("resourced base clone failed. Use MemberwiseClone Instead!");
				return MemberwiseClone();
			}
			obj.Descriptor = Descriptor;
			PropertyInfos.CopyTo(this,obj);
			obj.SetDefaultValue();
			return obj;
		}
	}
}