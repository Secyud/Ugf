#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
	public class LoadingService : ISingleton
	{
		public float MaxValue { get; set; }

		public float Value { get; set; }
	}
}