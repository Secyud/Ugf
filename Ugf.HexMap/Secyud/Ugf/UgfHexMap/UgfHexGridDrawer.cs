using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfHexGridDrawer:IHexGridDrawer,IRegistry
    {
        public void TriangulateChunk(HexChunk chunk)
        {
            UgfTriangulation triangulation = new(chunk);

            foreach (HexCell cell in chunk.Cells)
            {
                triangulation.TriangulateCell(cell.Get<UgfCell>());
            }
        }

        public Color32 GetCellShaderData(HexCell cell)
        {
            return new Color32(1,1,1,cell.Get<UgfCell>().TerrainType);
        }
    }
}