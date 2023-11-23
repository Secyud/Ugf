#region

using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfCoreModule),
        typeof(UgfUnityModule)
    )]
    public class UgfDotweenExModule : IUgfModule
    {
        public void ConfigureGame(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfDotweenExModule).Assembly);
        }
    }
}