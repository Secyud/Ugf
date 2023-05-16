#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
	public abstract class HexMapRootBase : MonoBehaviour
	{
		public HexGrid Grid;
		public Camera Camera;
		public HexMapCamera MapCamera;

		public HexCell GetCellUnderCursor()
		{
			return Grid.GetCell(Camera.ScreenPointToRay(Input.mousePosition));
		}

		public void Hide()
		{
			transform.position = new Vector3(65535, 65535, 65535);
			Grid.enabled = false;
			MapCamera.enabled = false;
			Camera.enabled = false;
		}

		public void Show()
		{
			transform.position = new Vector3(0, 0, 0);
			Grid.enabled = true;
			MapCamera.enabled = true;
			Camera.enabled = true;
		}
	}
}