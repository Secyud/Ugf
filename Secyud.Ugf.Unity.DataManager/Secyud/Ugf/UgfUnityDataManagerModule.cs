using Secyud.Ugf.DataManager;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf
{
    [DependsOn(typeof(UgfUnityModule))]
    public class UgfUnityDataManagerModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddTypes(typeof(UnityDataManagerService));
        }
    }
}