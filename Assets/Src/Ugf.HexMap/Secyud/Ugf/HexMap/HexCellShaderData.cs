#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Component that manages cell data used by shaders.
    /// </summary>
    public class HexCellShaderData : MonoBehaviour
    {
        private static readonly int HexCellData = Shader.PropertyToID("_HexCellData");
        private static readonly int HexCellDataTexelSize = Shader.PropertyToID("_HexCellData_TexelSize");

        private Texture2D _cellTexture;

        private Color32[] _cellTextureData;

        public HexGrid Grid { get; set; }

        public bool ImmediateMode { get; set; }

        /// <summary>
        /// Initialize the map data.
        /// </summary>
        /// <param name="x">Map X size.</param>
        /// <param name="z">Map Z size.</param>
        public void Initialize(int x, int z)
        {
            if (_cellTexture)
            {
                _cellTexture.Reinitialize(x, z);
            }
            else
            {
                _cellTexture = new Texture2D(
                    x, z,
                    TextureFormat.RGBA32,
                    false, true)
                {
                    filterMode = FilterMode.Point,
                    wrapModeU = TextureWrapMode.Repeat,
                    wrapModeV = TextureWrapMode.Clamp
                };
                Shader.SetGlobalTexture(HexCellData, _cellTexture);
            }

            Shader.SetGlobalVector(
                HexCellDataTexelSize,
                new Vector4(1f / x, 1f / z, x, z));

            if (_cellTextureData == null || _cellTextureData.Length != x * z)
            {
                _cellTextureData = new Color32[x * z];
            }
            else
            {
                for (int i = 0; i < _cellTextureData.Length; i++)
                {
                    _cellTextureData[i] = new Color32(0, 0, 0, 0);
                }
            }

            enabled = true;
        }

        /// <summary>
        /// Refresh the terrain data of a cell. Supports water surfaces up to 30 units high.
        /// </summary>
        /// <param name="cell">Cell with changed terrain type.</param>
        public void RefreshTerrain(HexCell cell)
        {
            Color32 data = _cellTextureData[cell.Index];
            data.b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
            data.a = (byte)cell.TerrainTypeIndex;
            _cellTextureData[cell.Index] = data;
            enabled = true;
        }

        /// <summary>
        /// Set arbitrary map data of a cell, overriding water data.
        /// </summary>
        /// <param name="cell">Cell to apply data for.</param>
        /// <param name="data">Cell data value, 0-1 inclusive.</param>
        public void SetMapData(HexCell cell, float data)
        {
            _cellTextureData[cell.Index].b =
                data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 255f) : (byte)255);
            enabled = true;
        }

        /// <summary>
        /// Indicate that view elevation data has changed, requiring a visibility reset.
        /// Supports water surfaces up to 30 units high.
        /// </summary>
        /// <param name="cell">Changed cell.</param>
        public void ViewElevationChanged(HexCell cell)
        {
            _cellTextureData[cell.Index].b = cell.IsUnderwater ? (byte)(cell.WaterSurfaceY * (255f / 30f)) : (byte)0;
            enabled = true;
        }

        private void LateUpdate()
        {
            if (_cellTexture)
            {
                _cellTexture.SetPixels32(_cellTextureData);
                _cellTexture.Apply();
            }
        }
    }
}