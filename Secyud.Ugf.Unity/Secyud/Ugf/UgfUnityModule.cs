using Secyud.Ugf.Logging;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Unity.Utilities;

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfCoreModule)
    )]
    public class UgfUnityModule : IUgfModule, IOnPostConfigure, IOnPreConfigure
    {
        public void PreConfigure(ConfigurationContext context)
        {
            UnityLogger logger = new();
            UgfLogger.InnerLogger = logger;
            context.Manager.RegisterInstance<ILogger>(logger);
        }

        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfUnityModule).Assembly);
        }

        public void PostConfigure(ConfigurationContext context)
        {
            U.SetUtility(new DefaultUnityUtility(UgfGameManager.Instance, context.Manager));
        }
    }
}