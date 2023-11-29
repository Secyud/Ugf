using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.HexMapExtensions
{
    public static class HexCellExtension
    {
        public const int Border = 5;

        internal static HexGrid CurrentGrid { get; set; }

        public static bool IsValid(this HexCell cell)
        {
            if (cell is null) return false;

            return cell.X >= 0 && cell.X < CurrentGrid.CellCountX - 2 * Border &&
                   cell.Z >= 0 && cell.Z < CurrentGrid.CellCountZ - 2 * Border;
        }
    }
}