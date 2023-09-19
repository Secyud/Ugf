//#define DATA_MANAGER

#region

using Localization;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.InputManager;
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
                typeof(InputService));

            context.AddResource<DefaultResource>();
        }
    }
}