#region

using Localization;
using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
	public class UgfCoreModule : IUgfModule, IPostConfigure
	{
		public void PostConfigureGame(ConfigurationContext context)
		{
			Og.Initialize(context.Manager);
		}

		public void ConfigureGame(ConfigurationContext context)
		{
			context.AddResource<DefaultResource>();
		}
	}
}