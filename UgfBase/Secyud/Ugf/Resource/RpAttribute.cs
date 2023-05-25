using System;

namespace Secyud.Ugf.Resource
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RpAttribute:Attribute
	{
		public short ID { get; }
		
		public RpAttribute(short id)
		{
			ID = id;
		}
	}
}