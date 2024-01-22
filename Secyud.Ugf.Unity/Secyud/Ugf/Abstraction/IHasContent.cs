#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.Abstraction
{
	/// <summary>
	/// Set changeable sub content of transform.
	/// </summary>
	public interface IHasContent
	{
		void SetContent(Transform transform);
	}
}