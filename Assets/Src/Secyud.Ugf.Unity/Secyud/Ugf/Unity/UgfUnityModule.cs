using Secyud.Ugf.Modularity;
using Secyud.Ugf.Unity.AssetBundles;
using Secyud.Ugf.Unity.Prefabs;

namespace Secyud.Ugf.Unity
{
    
    [DependsOn(
        typeof(UgfCoreModule) 
        )]
    public class UgfUnityModule:UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(new []
            {
                typeof(AssetBundleManager),
                typeof(PrefabManager)
            });
        }
    }
}