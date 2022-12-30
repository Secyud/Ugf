using Secyud.Ugf;
using Secyud.Ugf.Modularity;

namespace Demo
{
    [DependsOn(
        typeof(UgfCoreModule)
    )]
    public class DemoDomainModule:UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(DemoDomainModule).Assembly);
        }
    }
}