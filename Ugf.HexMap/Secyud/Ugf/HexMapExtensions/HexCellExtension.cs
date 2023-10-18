using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.HexMapExtensions
{
    public static class HexCellExtension
    {
        internal static HexGrid CurrentGrid { get; set; }
        
        public static bool IsValid(this HexCell cell)
        {
            if (!cell) return false;
            
            const int border = 4;
            
            int x = cell.Index % CurrentGrid.CellCountX;
            int z = cell.Index / CurrentGrid.CellCountX;

            return x > border && x < CurrentGrid.CellCountX - border &&
                   z > border && z < CurrentGrid.CellCountZ - border;
        }
    }
}