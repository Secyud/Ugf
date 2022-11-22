using Secyud.Ugf;
using Secyud.Ugf.Modularity;

namespace Demo
{
    [DependsOn(
        typeof(DemoUiModule),
        typeof(DemoDomainModule),
        typeof(UgfCoreModule)
        )]
    public class DemoGameModule:UgfModule
    {
        public override void PreConfigure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(DemoGameModule).Assembly);
        }

        
    }
}