#region

using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap.Utilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
	public interface IHexMapManager:IRegistry
	{
		public Transform GetFeature(HexCell cell);

		public Transform GetSpecial(HexCell cell);

		public int GetMoveCost(HexCell from, HexCell to, HexDirection direction);

		public int GetSpeed(HexUnit unit);

		public CellBase InitMessage(int x, int z,HexGrid grid);
	}
}