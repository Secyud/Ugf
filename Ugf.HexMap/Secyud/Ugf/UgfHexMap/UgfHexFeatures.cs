#region

using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.UgfHexMap
{
	/// <summary>
	///     Component that manages the map feature visualizations for a hex grid chunk.
	/// </summary>
	public class UgfHexFeatures : MonoBehaviour
	{
		[SerializeField] private HexMesh Walls;
		[SerializeField] private Transform WallTower;
		[SerializeField] private Transform Bridge;

		private Transform _container;

		/// <summary>
		///     Clear all features.
		/// </summary>
		public void Clear()
		{
			if (_container) Destroy(_container.gameObject);

			_container = new GameObject("Features Container").transform;
			_container.SetParent(transform, false);
			Walls.Clear();
		}

		/// <summary>
		///     Apply triangulation.
		/// </summary>
		public void Apply()
		{
			Walls.Apply();
		}

		public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
		{
			roadCenter1 = HexMetrics.Perturb(roadCenter1);
			roadCenter2 = HexMetrics.Perturb(roadCenter2);
			Transform instance = Instantiate(Bridge, _container, false);
			instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
			instance.forward = roadCenter2 - roadCenter1;
			float length = Vector3.Distance(roadCenter1, roadCenter2);
			instance.localScale = new Vector3(
				1f, 1f, length * (1f / HexMetrics.BridgeDesignLength)
			);
		}



	}
}