#region

using System.Threading.Tasks;

#endregion

namespace Secyud.Ugf.Modularity
{
	public interface IModuleLifecycleContributor
	{
		Task InitializeAsync(InitializationContext context, IUgfModule module);

		void Initialize(InitializationContext context, IUgfModule module);

		Task ShutdownAsync(ShutdownContext context, IUgfModule module);

		void Shutdown(ShutdownContext context, IUgfModule module);
	}
}