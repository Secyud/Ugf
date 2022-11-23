using Secyud.Ugf;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefab;

namespace Demo
{
    [DependsOn(
        typeof(UgfCoreModule)
        )]
    public class DemoUiModule:UgfModule
    {
        public override void Configure(ConfigurationContext context)
        {
            var prefabManager = context.Manager.GetDependency<IPrefabManager>();
            
            prefabManager.RegisterPrefabsInFolder("Demo.Ui",true);
        }
    }
}