namespace Secyud.Ugf.DependencyInjection
{
	public interface IDependencyScopeFactory
	{
		TScope CreateScope<TScope>()where TScope:DependencyScope;
		
		void DestroyScope<TScope>()where TScope:DependencyScope;

		TScope GetScope<TScope>()where TScope:DependencyScope;
	}
}