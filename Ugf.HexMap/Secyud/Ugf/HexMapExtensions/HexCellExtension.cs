using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.HexMapExtensions
{
    public static class HexCellExtension
    {
        public const int Border = 5;
        
        internal static HexGrid CurrentGrid { get; set; }
        
        public static bool IsValid(this HexCell cell)
        {
            if (!cell) return false;
            
            int x = cell.Index % CurrentGrid.CellCountX;
            int z = cell.Index / CurrentGrid.CellCountX;

            return x > Border && x < CurrentGrid.CellCountX - Border &&
                   z > Border && z < CurrentGrid.CellCountZ - Border;
        }
    }
}