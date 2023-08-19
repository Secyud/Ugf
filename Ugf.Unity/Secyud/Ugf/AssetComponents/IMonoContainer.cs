#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetComponents
{
	public interface IMonoContainer<out TComponent> : IObjectAccessor<TComponent>
		where TComponent : Component
	{
		TComponent Create();

		void Destroy();
	}
}