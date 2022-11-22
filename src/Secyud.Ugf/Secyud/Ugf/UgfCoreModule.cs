using Secyud.Ugf.Modularity;
using Secyud.Ugf.UserInterface;

namespace Secyud.Ugf
{
    public class UgfCoreModule:UgfModule
    {
        public override void PreConfigure(ConfigurationContext context)
        {
            context.Manager.AddType<UiManager>();
            context.Manager.AddType<UiControllerManager>();
        }
    }
}