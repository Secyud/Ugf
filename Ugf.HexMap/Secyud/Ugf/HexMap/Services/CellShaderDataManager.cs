using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public class CellShaderDataManager : IRegistry
    {
        private static readonly int HexCellDataTexelSize = Shader.PropertyToID("_HexCellData_TexelSize");
        private static readonly int HexCellData = Shader.PropertyToID("_HexCellData");

        private readonly Texture2D _cellTexture =
            new(32, 32,
                TextureFormat.RGBA32,
                false, true)
            {
                filterMode = FilterMode.Point,
                wrapModeU = TextureWrapMode.Repeat,
                wrapModeV = TextureWrapMode.Clamp
            };

        public Color32[] CellData { get; private set; } = new Color32[32 * 32];
        
        public void ChangeTextureSize(int x, int z)
        {
            _cellTexture.Reinitialize(x, z);
            if (CellData.Length != x * z)
            {
                CellData = new Color32[x * z];
            }
            Shader.SetGlobalVector(HexCellDataTexelSize, new Vector4(1f / x, 1f / z, x, z));
        }

        public void ApplyTexture()
        {
            _cellTexture.SetPixels32(CellData);
            _cellTexture.Apply();
            Shader.SetGlobalTexture(HexCellData, _cellTexture);
        }
    }
}