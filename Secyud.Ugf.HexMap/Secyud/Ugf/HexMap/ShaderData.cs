#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
	/// <summary>
	///     Component that manages cell data used by shaders.
	/// </summary>
	public class ShaderData : MonoBehaviour
	{
		public HexGrid Grid { get; private set; }
		
		public void LateUpdate()
		{
			Grid.ShaderManager.ApplyTexture();
			enabled = false;
		}

		public void Refresh()
		{
			enabled = true;
		}

		public void Initialize(HexGrid grid)
		{
			Grid = grid;
		}
	}
}