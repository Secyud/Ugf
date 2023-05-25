using Secyud.Ugf.Archiving;
using System;
using System.IO;

namespace Secyud.Ugf.Resource
{
	public abstract class ResourcedBase : IArchivable
	{
		protected ResourceDescriptor Descriptor { get; private set; }

		private ResourceProperty _property;
		protected ResourceProperty Property => _property ??=
			ResourceManager.Instance.GetResourceProperty(GetType());


		
		public virtual void InitSetting(string name = null)
		{
			if (name.IsNullOrEmpty()) name = GetType().Name;
			Descriptor = ResourceManager.Instance.GetResourceDescriptor(name);
			Property.Init(this,Descriptor);
			SetDefaultValue();
		}

		public virtual void Save(BinaryWriter writer)
		{
			Property.Write(this, writer);
		}

		public virtual void Load(BinaryReader reader)
		{
			Property.Read(this, reader);
			SetDefaultValue();
		}

		protected virtual void SetDefaultValue()
		{
		}
	}
}