#region

using Localization;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    public class UgfCoreModule : IUgfModule, IPostConfigure
    {
        public void ConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(DependencyManager),
                typeof(DefaultStringLocalizerFactory),
                typeof(ArchivingManager),
                typeof(LoadingService)
            );
            context.AddResource<DefaultResource>();
        }

        public void PostConfigureGame(ConfigurationContext context)
        {
            Og.Initialize(context.Manager);
        }
    }
}