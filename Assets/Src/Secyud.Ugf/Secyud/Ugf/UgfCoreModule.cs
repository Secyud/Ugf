using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Modularity;

namespace Secyud.Ugf
{
    public class UgfCoreModule : UgfModule
    {
        protected override void PreConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(new []
            {
                typeof(DependencyManager)
            });
        }
    }
}