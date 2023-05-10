#region

using Secyud.Ugf.HexMap.Utilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
	public interface IHexMapManager
	{
		public Transform GetFeature(HexCell cell);

		public Transform GetSpecialFeature(HexCell cell);

		public int GetMoveCost(HexUnit unit, HexCell from, HexCell to, HexDirection direction);

		public int GetSpeed(HexUnit unit);
	}
}