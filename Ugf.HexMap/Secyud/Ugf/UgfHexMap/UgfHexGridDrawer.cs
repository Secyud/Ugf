using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfHexGridDrawer : IHexGridDrawer, IRegistry
    {
        public void TriangulateChunk(HexChunk chunk)
        {
            UgfTriangulation triangulation = new(chunk);

            foreach (int index in chunk.Cells)
            {
                triangulation.TriangulateCell(chunk.Grid.GetCell(index) as UgfCell);
            }
        }

        public Color32 GetCellShaderData(HexCell cell)
        {
            return new Color32(1, 1, 1, ((UgfCell)cell).TerrainType);
        }

        public virtual HexCell CreateCell()
        {
            return new UgfCell();
        }
    }
}