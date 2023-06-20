#region

using Localization;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.InputManaging;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    public class UgfCoreModule : IUgfModule
    {
        public void ConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddTypes(
                typeof(DefaultLocalizerFactory),
                typeof(InputService),
                typeof(InitializeManager));

            context.AddResource<DefaultResource>();
        }
    }
}