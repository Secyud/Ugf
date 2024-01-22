using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public interface IHexGridDrawer
    {
        void TriangulateChunk(HexChunk chunk);

        Color32 GetCellShaderData(HexCell cell);

        HexCell CreateCell();
    }
}