#region

using System;

#endregion

namespace Secyud.Ugf.Localization
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ResourceNameAttribute : Attribute
	{
		public ResourceNameAttribute(Type toResource)
		{
			ToResource = toResource;
		}

		public Type ToResource { get; }
	}
}