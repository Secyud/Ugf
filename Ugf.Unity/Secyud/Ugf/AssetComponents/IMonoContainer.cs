#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.AssetComponents
{
	public interface IMonoContainer<out TComponent> : IObjectAccessor<TComponent>
		where TComponent : MonoBehaviour
	{
		TComponent Create();

		void Destroy();
	}
}