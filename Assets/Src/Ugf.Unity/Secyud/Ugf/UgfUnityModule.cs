#region

using Secyud.Ugf.Modularity;
using Secyud.Ugf.Unity.AssetBundles;

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
                typeof(AssetBundleManager),
                typeof(SpriteManager)
            );
        }
    }
}