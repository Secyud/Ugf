#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.UgfHexMapEditor
{
	/// <summary>
	///     Component that represents a single save or load menu item.
	/// </summary>
	public class SaveLoadItem : MonoBehaviour
	{
		private string _mapName;

		/// <summary>
		///     Parent save-load menu.
		/// </summary>
		public SaveLoadMenu Menu { get; set; }

		/// <summary>
		///     Map name of the item.
		/// </summary>
		public string MapName
		{
			get => _mapName;
			set
			{
				_mapName = value;
				transform.GetChild(0).GetComponent<Text>().text = value;
			}
		}

		/// <summary>
		///     Selection method, hooked up to the in-game UI.
		/// </summary>
		public void Select()
		{
			Menu.SelectItem(_mapName);
		}
	}
}