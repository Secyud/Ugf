using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.HexMap.Generator
{
	public interface IHexMapGenerator:IRegistry
	{
		void GenerateMap(HexGrid grid, int x, int z);

		HexCell[] TmpCells { get; set; }

		int CellCountX { get; set; }

		int CellCountZ { get; set; }

		int DeltaX { get; set; }

		int DeltaZ { get; set; }
	}
}