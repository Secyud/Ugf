namespace Secyud.Ugf.Resource
{
	public static class ResourceExtension
	{
		public static TResourcedBase Init<TResourcedBase>(
			this TResourcedBase resourcedBase,string name)
		where TResourcedBase:ResourcedBase
		{
			resourcedBase.InitSetting(name);
			return resourcedBase;
		}
	}
}