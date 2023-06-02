using System;

namespace Secyud.Ugf.Resource
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RAttribute:Attribute
	{
		public int ID { get; }
		public bool NoArchive { get; }
		
		public RAttribute(int id,bool noArchive = false)
		{
			ID = id;
			NoArchive = noArchive;
		}
	}
}