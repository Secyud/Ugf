#include "../HexCellData.hlsl"

void GetVertexCellData_float(
    float3 WorldPosition,
    bool EditMode,
    out float2 Visibility
)
{
    HexGridData hgd = GetHexGridData(WorldPosition.xz);
    float4 cellData = GetCellData(hgd.cellOffsetCoordinates, EditMode);

    Visibility.x = cellData.x;
    Visibility.x = lerp(0.25, 1, Visibility.x);
    Visibility.y = cellData.y;
}

void GetFragmentData_float(
    UnityTexture2D BaseTexture,
    float2 UV,
    float3 WorldPosition,
    float3 Color,
    float2 Visibility,
    out float3 BaseColor,
    out float Exploration
)
{
    float3 c = BaseTexture.Sample(BaseTexture.samplerstate, UV).rgb * Color;
    BaseColor = c.rgb * Visibility.x;
    Exploration = Visibility.y;
}
