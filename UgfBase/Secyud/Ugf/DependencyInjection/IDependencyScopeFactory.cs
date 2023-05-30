namespace Secyud.Ugf.DependencyInjection
{
	public interface IDependencyScopeFactory
	{
		TScope CreateScope<TScope>()where TScope:class,IDependencyScope;
		
		void DestroyScope<TScope>()where TScope:class,IDependencyScope;

		TScope GetScope<TScope>()where TScope:class,IDependencyScope;
	}
}