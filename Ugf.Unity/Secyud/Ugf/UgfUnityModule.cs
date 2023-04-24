#region

using Secyud.Ugf.AssetBundles;
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
            context.Manager.AddTypes(
                typeof(AssetBundleManager)
            );
        }
    }
}