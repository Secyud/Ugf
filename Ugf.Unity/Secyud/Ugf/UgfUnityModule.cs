#region

using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
	[DependsOn(
		typeof(UgfCoreModule)
	)]
	public class UgfUnityModule : IUgfModule
	{
		public void Configure(ConfigurationContext context)
		{
			context.Manager.AddAssembly(typeof(UgfUnityModule).Assembly);
		}
	}
}