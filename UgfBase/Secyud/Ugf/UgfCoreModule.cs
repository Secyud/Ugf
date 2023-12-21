#region

using Localization;
using Secyud.Ugf.InputManager;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.VirtualPath;

#endregion

namespace Secyud.Ugf
{
    public class UgfCoreModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(DefaultStringLocalizerFactory),
                typeof(InputService),
                typeof(VirtualPathManager));

            context.AddStringResource<DefaultResource>();
        }
    }
}