namespace Secyud.Ugf.DependencyInjection
{
	public  class DependencyScope : IDependencyScope
	{
		public DependencyScope(IDependencyProviderFactory dependencyProvider)
		{
			DependencyProvider = new DependencyProvider(dependencyProvider);
		}

		public IDependencyProvider DependencyProvider { get; }

		public T Get<T>() where T : class => DependencyProvider.Get<T>();

		public virtual void Dispose()
		{
		}
	}
}