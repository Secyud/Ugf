using Secyud.Ugf.DataManager;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.VirtualPath;

namespace Secyud.Ugf
{
    public class UgfCoreModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(VirtualPathManager),
                typeof(UgfLocalizerFactory)
                );
            context.Manager.RegisterInstance(TypeManager.Instance);
            context.Manager.RegisterInstance(DefaultLogger.Instance);
        }
    }
}