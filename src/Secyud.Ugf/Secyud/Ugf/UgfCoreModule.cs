using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefabs;

namespace Secyud.Ugf
{
    public class UgfCoreModule : UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(new []
            {
                typeof(PrefabManager),
                typeof(DependencyManager)
            });
        }
    }
}