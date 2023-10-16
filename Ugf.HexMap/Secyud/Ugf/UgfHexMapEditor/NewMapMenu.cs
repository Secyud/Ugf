#region

using Secyud.Ugf.HexMap;
using Secyud.Ugf.UgfHexMapGenerator;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Secyud.Ugf.UgfHexMapEditor
{
	/// <summary>
	///     Component that applies actions from the new map menu UI to the hex map.
	///     Public methods are hooked up to the in-game UI.
	/// </summary>
	public class NewMapMenu : MonoBehaviour
	{
		[FormerlySerializedAs("hexGrid")] [SerializeField]
		private HexGrid HexGrid;

		private bool _generateMaps = true;

		[FormerlySerializedAs("mapGenerator")] [SerializeField]
		private HexMapGenerator MapGenerator;

		public void ToggleMapGeneration(bool toggle)
		{
			_generateMaps = toggle;
		}


		public void Open()
		{
			gameObject.SetActive(true);
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}

		public void CreateSmallMap()
		{
			CreateMap(20, 15);
		}

		public void CreateMediumMap()
		{
			CreateMap(40, 30);
		}

		public void CreateLargeMap()
		{
			CreateMap(80, 60);
		}

		private void CreateMap(int x, int z)
		{
			if (_generateMaps)
				MapGenerator.GenerateMap(HexGrid, x, z);
			else
				HexGrid.CreateMap(x, z);

			Close();
		}
	}
}