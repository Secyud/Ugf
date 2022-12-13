using Secyud.Ugf;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefabs;

namespace Demo
{
    [DependsOn(
        typeof(DemoDomainModule),
        typeof(UgfCoreModule)
        )]
    public class DemoGameModule:UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            var prefabRegister = context.Manager.GetDependency<IPrefabRegister>();
            
            prefabRegister.RegisterPrefabsInAssembly(typeof(DemoGameModule).Assembly,true);
        }
    }
}