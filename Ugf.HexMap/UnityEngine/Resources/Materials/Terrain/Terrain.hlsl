#include "../HexCellData.hlsl"

void GetVertexCellData_float(
    float3 Indices,
    float3 Weights,
    bool EditMode,
    out float4 Terrain,
    out float4 Visibility
)
{
    float4 cell0 = GetCellData(Indices, 0, EditMode);
    float4 cell1 = GetCellData(Indices, 1, EditMode);
    float4 cell2 = GetCellData(Indices, 2, EditMode);

    Terrain.x = cell0.w;
    Terrain.y = cell1.w;
    Terrain.z = cell2.w;
    Terrain.w = max(max(cell0.b, cell1.b), cell2.b) * 30.0;

    Visibility.x = cell0.x;
    Visibility.y = cell1.x;
    Visibility.z = cell2.x;
    Visibility.xyz = lerp(0.25, 1, Visibility.xyz);
    Visibility.w = cell0.y * Weights.x + cell1.y * Weights.y + cell2.y * Weights.z;
}

// Sample appropriate terrain texture and apply cell weights and visibility.
float4 GetTerrainColor(
    UnityTexture2DArray TerrainTextures,
    float3 WorldPosition,
    float4 Terrain,
    float3 Weights,
    float4 Visibility,
    int index
)
{
    float3 uvw = float3(WorldPosition.xz * (2 * TILING_SCALE), Terrain[index]);
    float4 c = TerrainTextures.Sample(TerrainTextures.samplerstate, uvw);
    return c * (Weights[index] * Visibility[index]);
}

// Apply an 80% darkening grid outline at hex center distance 0.965-1.
float3 ApplyGrid(float3 baseColor, HexGridData h)
{
    return baseColor * (0.2 + 0.8 * h.Smoothstep10(0.965));
}

// Apply a white outline at hex center distance 0.68-0.8.
float3 ApplyHighlight(float3 baseColor, HexGridData h)
{
    return saturate(h.SmoothstepRange(0.68, 0.8) + baseColor.rgb);
}

// Apply a blue color filter based on surface submergence, up to 15 units deep.
float3 ColorizeSubmergence(float3 baseColor, float surfaceY, float waterY)
{
    float submergence = waterY - max(surfaceY, 0);
    float3 colorFilter = float3(0.25, 0.25, 0.75);
    float filterRange = 1.0 / 15.0;
    return baseColor * lerp(1.0, colorFilter, saturate(submergence * filterRange));
}

void GetFragmentData_float(
    UnityTexture2DArray TerrainTextures,
    float3 WorldPosition,
    float4 Terrain,
    float4 Visibility,
    float3 Weights,
    bool ShowGrid,
    out float3 BaseColor,
    out float Exploration
)
{
    float4 c =
        GetTerrainColor(TerrainTextures, WorldPosition, Terrain, Weights, Visibility, 0) +
        GetTerrainColor(TerrainTextures, WorldPosition, Terrain, Weights, Visibility, 1) +
        GetTerrainColor(TerrainTextures, WorldPosition, Terrain, Weights, Visibility, 2);

    BaseColor = ColorizeSubmergence(c.rgb, WorldPosition.y, Terrain.w);

    HexGridData hgd = GetHexGridData(WorldPosition.xz);

    if (ShowGrid)
    {
        BaseColor = ApplyGrid(BaseColor, hgd);
    }

    if (hgd.IsHighlighted())
    {
        BaseColor = ApplyHighlight(BaseColor, hgd);
    }

    Exploration = Visibility.w;
}
