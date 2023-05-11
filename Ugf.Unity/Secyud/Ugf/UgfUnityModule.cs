#region

using Secyud.Ugf.AssetLoading;
using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
	[DependsOn(
		typeof(UgfCoreModule)
	)]
	public class UgfUnityModule : IUgfModule
	{
		public void ConfigureGame(ConfigurationContext context)
		{
		}
	}
}