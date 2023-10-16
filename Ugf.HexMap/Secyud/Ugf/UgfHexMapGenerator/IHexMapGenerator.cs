using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.UgfHexMapGenerator
{
	public interface IHexMapGenerator:IRegistry
	{
		void GenerateMap(HexGrid grid, int x, int z);

		int CellCountX { get; set; }

		int CellCountZ { get; set; }

		int DeltaX { get; set; }

		int DeltaZ { get; set; }
	}
}