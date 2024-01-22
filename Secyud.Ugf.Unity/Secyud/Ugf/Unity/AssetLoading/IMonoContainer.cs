#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.AssetLoading
{
	public interface IMonoContainer<out TComponent> : IObjectAccessor<TComponent>
		where TComponent : Component
	{
		TComponent Create();
		TComponent GetOrCreate();

		void Destroy();
	}
}