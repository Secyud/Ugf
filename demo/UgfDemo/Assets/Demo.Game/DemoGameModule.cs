using Secyud.Ugf;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Prefab;

namespace Demo
{
    [DependsOn(
        typeof(DemoDomainModule),
        typeof(UgfCoreModule)
        )]
    public class DemoGameModule:UgfModule
    {
        public override void PreConfigure(ConfigurationContext context)
        {
            var prefabRegister = context.Manager.GetDependency<IPrefabRegister>();
            
            prefabRegister.RegisterPrefabsInAssembly(typeof(DemoGameModule).Assembly,true);

        }

        
    }
}