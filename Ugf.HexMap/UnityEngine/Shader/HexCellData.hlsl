#include "HexMetrics.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_HexCellData);
SAMPLER(sampler_HexCellData);
float4 _HexCellData_TexelSize;


float4 GetCellData (const float3 uv2, int index) {
	float2 uv;
	uv.x = (uv2[index] + 0.5) * _HexCellData_TexelSize.x;
	const float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
	float4 data = SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0);
	data.w *= 255;
	return data;
}

float4 GetCellData (const float2 cellDataCoordinates) {
	float2 uv = cellDataCoordinates + 0.5;
	uv.x *= _HexCellData_TexelSize.x;
	uv.y *= _HexCellData_TexelSize.y;
	return SAMPLE_TEXTURE2D_LOD(_HexCellData, sampler_HexCellData, uv, 0);
}

// Cell highlighting data, in hex space.
// x: Highlight center X position.
// y: Highlight center Z position.
// z: Highlight radius, squared with bias. Is negative if there is no highlight.
// w: Hex grid wrap size, to support X wrapping. Is zero if there is no wrapping.
float4 _CellHighlighting;

// Hex grid data derived from world-space XZ position.
struct HexGridData {
	// Cell center in hex space.
	float2 cellCenter;

	// Approximate cell offset coordinates. Good enough for point-filtered sampling.
	float2 cellOffsetCoordinates;

	// For potential future use. U covers entire cell, V wraps a bit.
	float2 cellUV;

	// Hexagonal distance to cell center, 0 at center, 1 at edges.
	float distanceToCenter;

	// Smooth-step smoothing for cell center distance transitions.
	// Based on screen-space derivatives.
	float distanceSmoothing;

	// Is highlighted if square distance from cell to highlight center is below threshold.
	// Works up to brush size 6.
	bool IsHighlighted () {
		float2 cellToHighlight = abs(_CellHighlighting.xy - cellCenter);

		// Adjust for world X wrapping if needed.
		if (cellToHighlight.x > _CellHighlighting.w * 0.5) {
			cellToHighlight.x -= _CellHighlighting.w;
		}

		return dot(cellToHighlight, cellToHighlight) < _CellHighlighting.z;
	}

	// Smooth-step from 0 to 1 at cell center distance threshold.
	float SmoothStep01 (float threshold) {
		return smoothstep(
			threshold - distanceSmoothing,
			threshold + distanceSmoothing,
			distanceToCenter
		);
	}

	// Smooth-step from 1 to 0 at cell center distance threshold.
	float SmoothStep10 (float threshold) {
		return smoothstep(
			threshold + distanceSmoothing,
			threshold - distanceSmoothing,
			distanceToCenter
		);
	}

	// Smooth-step from 0 to 1 inside cell center distance range.
	float SmoothStepRange (const float innerThreshold, const float outerThreshold) {
		return SmoothStep01(innerThreshold) * SmoothStep10(outerThreshold);
	}
};

#define HEX_ANGLED_EDGE_VECTOR float2(1, sqrt(3))

// Calculate hexagonal center-edge distance for point relative to center in hex space.
// 0 at cell center and 1 at edges.
float HexagonalCenterToEdgeDistance (float2 p) {
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
float2 HexModulo (float2 p) {
	return p - HEX_ANGLED_EDGE_VECTOR * floor(p / HEX_ANGLED_EDGE_VECTOR);
}

// Get hex grid data analytically derived from world-space XZ position.
HexGridData GetHexGridData (float2 worldPositionXZ) {
	const float2 p = WoldToHexSpace(worldPositionXZ);

	// Vectors from nearest two cell centers to position.
	const float2 gridOffset = HEX_ANGLED_EDGE_VECTOR * 0.5;
	const float2 a = HexModulo(p) - gridOffset;
	const float2 b = HexModulo(p - gridOffset) - gridOffset;
	const bool aIsNearest = dot(a, a) < dot(b, b);

	const float2 vectorFromCenterToPosition = aIsNearest ? a : b;

	HexGridData d;
	d.cellCenter = p - vectorFromCenterToPosition;
	d.cellOffsetCoordinates.x = d.cellCenter.x - (aIsNearest ? 0.5 : 0.0);
	d.cellOffsetCoordinates.y = d.cellCenter.y / OUTER_TO_INNER;
	d.cellUV = vectorFromCenterToPosition + 0.5;
	d.distanceToCenter = HexagonalCenterToEdgeDistance(vectorFromCenterToPosition);
	d.distanceSmoothing = fwidth(d.distanceToCenter);
	return d;
}