#region

using System.Threading.Tasks;

#endregion

namespace Secyud.Ugf.Modularity
{
    public abstract class ModuleLifecycleContributorBase : IModuleLifecycleContributor
    {
        public Task InitializeAsync(InitializationContext context, IUgfModule module)
        {
            return Task.CompletedTask;
        }

        public void Initialize(InitializationContext context, IUgfModule module)
        {
        }

        public Task ShutdownAsync(ShutdownContext context, IUgfModule module)
        {
            return Task.CompletedTask;
        }

        public void Shutdown(ShutdownContext context, IUgfModule module)
        {
        }
    }
}