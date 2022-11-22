using Secyud.Ugf;
using Secyud.Ugf.Modularity;

namespace Demo
{
    [DependsOn(
        typeof(UgfCoreModule)
    )]
    public class DemoDomainModule:UgfModule
    {
        public override void PreConfigure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(DemoDomainModule).Assembly);
        }
    }
}