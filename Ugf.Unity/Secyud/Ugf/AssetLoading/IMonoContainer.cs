#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.Container
{
	public interface IMonoContainer<out TComponent> : IObjectAccessor<TComponent>
		where TComponent : MonoBehaviour
	{

		TComponent Create();

		void Destroy();
	}
}