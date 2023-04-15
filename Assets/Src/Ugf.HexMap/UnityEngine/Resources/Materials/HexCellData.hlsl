#include "HexMetrics.hlsl"

TEXTURE2D (_HexCellData);
SAMPLER (sampler_HexCellData);
float4 _HexCellData_TexelSize;

float4 FilterCellData(float4 data, bool editMode)
{
    data.xy = 1;

    return data;
}

float4 GetCellData(float3 uv2, int index, bool editMode)
{
    float2 uv;
    uv.x = (uv2[index] + 0.5) * _HexCellData_TexelSize.x;
    float row = floor(uv.x);
    uv.x -= row;
    uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
    float4 data = SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0);
    data.w *= 255;
    return FilterCellData(data, editMode);
}

float4 GetCellData(float2 cellDataCoordinates, bool editMode)
{
    float2 uv = cellDataCoordinates + 0.5;
    uv.x *= _HexCellData_TexelSize.x;
    uv.y *= _HexCellData_TexelSize.y;
    return FilterCellData(
        SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0), editMode
    );
}

// Cell highlighting data, in hex space.
// x: Highlight center X position.
// y: Highlight center Z position.
// z: Highlight radius, squared with bias. Is negative if there is no highlight.
// w: Hex grid wrap size, to support X wrapping. Is zero if there is no wrapping.
float4 _CellHighlighting;

// Hex grid data derived from world-space XZ position.
struct HexGridData
{
    // Cell center in hex space.
    float2 cellCenter;

    // Approximate cell offset coordinates. Good enough for point-filtered sampling.
    float2 cellOffsetCoordinates;

    // For potential future use. U covers entire cell, V wraps a bit.
    float2 cellUV;

    // Hexagonal distance to cell center, 0 at center, 1 at edges.
    float distanceToCenter;

    // Smoothstep smoothing for cell center distance transitions.
    // Based on screen-space derivatives.
    float distanceSmoothing;

    // Is highlighed if square distance from cell to highlight center is below threshold.
    // Works up to brush size 6.
    bool IsHighlighted()
    {
        float2 cellToHighlight = abs(_CellHighlighting.xy - cellCenter);

        // Adjust for world X wrapping if needed.
        if (cellToHighlight.x > _CellHighlighting.w * 0.5)
        {
            cellToHighlight.x -= _CellHighlighting.w;
        }

        return dot(cellToHighlight, cellToHighlight) < _CellHighlighting.z;
    }

    // Smoothstep from 0 to 1 at cell center distance threshold.
    float Smoothstep01(float threshold)
    {
        return smoothstep(
            threshold - distanceSmoothing,
            threshold + distanceSmoothing,
            distanceToCenter
        );
    }

    // Smoothstep from 1 to 0 at cell center distance threshold.
    float Smoothstep10(float threshold)
    {
        return smoothstep(
            threshold + distanceSmoothing,
            threshold - distanceSmoothing,
            distanceToCenter
        );
    }

    // Smoothstep from 0 to 1 inside cell center distance range.
    float SmoothstepRange(float innerThreshold, float outerThreshold)
    {
        return Smoothstep01(innerThreshold) * Smoothstep10(outerThreshold);
    }
};

#define HEX_ANGLED_EDGE_VECTOR float2(1, sqrt(3))

// Calculate hexagonal center-edge distance for point relative to center in hex space.
// 0 at cell center and 1 at edges.
float HexagonalCenterToEdgeDistance(float2 p)
{
    // Reduce problem to one quadrant.
    p = abs(p);
    // Calculate distance to angled edge.
    float d = dot(p, normalize(HEX_ANGLED_EDGE_VECTOR));
    // Incorporate distance to vertical edge.
    d = max(d, p.x);
    // Double to increase range from center to edge to 0-1.
    return 2 * d;
}

// Calculate hex-based modulo to find position vector.
float2 HexModulo(float2 p)
{
    return p - HEX_ANGLED_EDGE_VECTOR * floor(p / HEX_ANGLED_EDGE_VECTOR);
}

// Get hex grid data analytically derived from world-space XZ position.
HexGridData GetHexGridData(float2 worldPositionXZ)
{
    float2 p = WoldToHexSpace(worldPositionXZ);

    // Vectors from nearest two cell centers to position.
    float2 gridOffset = HEX_ANGLED_EDGE_VECTOR * 0.5;
    float2 a = HexModulo(p) - gridOffset;
    float2 b = HexModulo(p - gridOffset) - gridOffset;
    bool aIsNearest = dot(a, a) < dot(b, b);

    float2 vectorFromCenterToPosition = aIsNearest ? a : b;

    HexGridData d;
    d.cellCenter = p - vectorFromCenterToPosition;
    d.cellOffsetCoordinates.x = d.cellCenter.x - (aIsNearest ? 0.5 : 0.0);
    d.cellOffsetCoordinates.y = d.cellCenter.y / OUTER_TO_INNER;
    d.cellUV = vectorFromCenterToPosition + 0.5;
    d.distanceToCenter = HexagonalCenterToEdgeDistance(vectorFromCenterToPosition);
    d.distanceSmoothing = fwidth(d.distanceToCenter);
    return d;
}
