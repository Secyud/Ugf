#region

using Localization;
using Secyud.Ugf.InputManager;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    public class UgfCoreModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(DefaultLocalizerFactory),
                typeof(InputService));

            context.AddResource<DefaultResource>();
        }
    }
}