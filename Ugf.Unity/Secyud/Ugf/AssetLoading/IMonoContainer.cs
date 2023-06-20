#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetLoading
{
	public interface IMonoContainer<out TComponent> : IObjectAccessor<TComponent>
		where TComponent : MonoBehaviour
	{
		TComponent Create();

		void Destroy();
	}
}