using Secyud.Ugf.Archiving;
using System.IO;

namespace Secyud.Ugf.ResourceDomain
{
	public abstract class ResourcedBase:IArchivable
	{
		private ResourceDescriptor _descriptor;
		protected ResourceDescriptor Descriptor => _descriptor ??= 
			ResourceManager.Instance.GetResourceDescriptor(GetType().Name);
		private ResourceProperty _property;
		protected ResourceProperty Property => _property ??= 
			ResourceManager.Instance.GetResourceProperty(GetType());
		
		public virtual void InitSetting()
		{
			Property.Init(this,Descriptor);
			SetDefaultValue();
		}

		public virtual void Save(BinaryWriter writer)
		{
			Property.Write(this,writer);
		}

		public virtual void Load(BinaryReader reader)
		{
			Property.Read(this,reader);
			SetDefaultValue();
		}

		protected virtual void SetDefaultValue()
		{
			
		}
	}
}