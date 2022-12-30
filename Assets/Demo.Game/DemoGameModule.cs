using Secyud.Ugf.Modularity;
using Secyud.Ugf.Unity;
using Secyud.Ugf.Unity.Prefabs;

namespace Demo
{
    [DependsOn(
        typeof(DemoDomainModule),
        typeof(UgfUnityModule)
        )]
    public class DemoGameModule:UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            var prefabRegistrar = context.Manager.Get<IPrefabRegistrar>();
            
            prefabRegistrar.RegisterPrefabsInAssembly(typeof(DemoGameModule).Assembly,true);
        }
    }
}