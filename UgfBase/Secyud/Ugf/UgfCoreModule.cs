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
        public void PostConfigureGame(ConfigurationContext context)
        {
            Og.Initialize(context.Manager);
        }

        public void ConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(DependencyManager),
                typeof(DefaultStringLocalizerFactory),
                typeof(LoadingService),
                typeof(ArchivingContext)
            );
            context.AddResource<DefaultResource>();
        }
    }
}