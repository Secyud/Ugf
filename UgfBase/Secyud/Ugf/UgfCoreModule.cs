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
                typeof(DefaultLocalizerFactory<string>),
                typeof(InputService));

            context.AddStringResource<DefaultResource>();
        }
    }
}