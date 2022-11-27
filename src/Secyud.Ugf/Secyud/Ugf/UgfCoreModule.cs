using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefab;

namespace Secyud.Ugf
{
    public class UgfCoreModule : UgfModule
    {
        public override void PreConfigure(ConfigurationContext context)
        {
            context.Manager.AddTypes(new []
            {
                typeof(PrefabManager),
                typeof(DependencyManager) });
        }
    }
}