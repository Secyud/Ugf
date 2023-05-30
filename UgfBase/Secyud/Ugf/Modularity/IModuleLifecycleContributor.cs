#region

using System.Threading.Tasks;

#endregion

namespace Secyud.Ugf.Modularity
{
	public interface IModuleLifecycleContributor
	{
		Task InitializeAsync( IUgfModule module);

		void Initialize(IUgfModule module);

		Task ShutdownAsync( IUgfModule module);

		void Shutdown( IUgfModule module);
	}
}