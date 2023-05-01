#include "HexCellData.hlsl"

void GetVertexCellData_float (
	const float3 Indices,
	out float4 Terrain
) {
	float4 cell0 = GetCellData(Indices, 0);
	float4 cell1 = GetCellData(Indices, 1);
	float4 cell2 = GetCellData(Indices, 2);

	Terrain.x = cell0.w;
	Terrain.y = cell1.w;
	Terrain.z = cell2.w;
	Terrain.w = max(max(cell0.b, cell1.b), cell2.b) * 30.0;
}

float3 GetTerrainUV (
	float3 WorldPosition,
	float4 Terrain,
	int index) {
	return float3(WorldPosition.xz * (2 * TILING_SCALE), Terrain[index]);
}


// Sample appropriate terrain texture and apply cell weights and visibility.
float4 GetTerrainColor (
	UnityTexture2DArray TerrainTextures,
	const float4 Weights0,
	const float4 Weights1,
	const float4 Weights2,
	const float3 Texture0UV,
	const float3 Texture1UV,
	const float3 Texture2UV
) {
	return TerrainTextures.Sample(TerrainTextures.samplerstate, Texture0UV) * Weights0
		+ TerrainTextures.Sample(TerrainTextures.samplerstate, Texture1UV) * Weights1
		+ TerrainTextures.Sample(TerrainTextures.samplerstate, Texture2UV) * Weights2;
}

// Apply an 80% darkening grid outline at hex center distance 0.965-1.
float3 ApplyGrid (const float3 baseColor, HexGridData h) {
	return baseColor * (0.2 + 0.8 * h.SmoothStep10(0.965));
}

// Apply a white outline at hex center distance 0.68-0.8.
float3 ApplyHighlight (float3 baseColor, HexGridData h) {
	return saturate(h.SmoothStepRange(0.68, 0.8) + baseColor.rgb);
}

// Apply a blue color filter based on surface submergence, up to 15 units deep.
float3 ColorizeSubmergence (const float3 baseColor, const float surfaceY, float waterY) {
	const float submergence = waterY - max(surfaceY, 0);
	const float3 colorFilter = float3(0.25, 0.25, 0.75);
	const float filterRange = 1.0 / 15.0;
	return baseColor * lerp(1.0, colorFilter, saturate(submergence * filterRange));
}

void GetFragmentData_float (
	const UnityTexture2DArray DiffuseTexture,
	const UnityTexture2DArray NormalTexture,
	const UnityTexture2DArray GlossTexture,
	const UnityTexture2DArray EmissionTexture,
	const UnityTexture2DArray AmbientOcclusionTexture,
	const UnityTexture2DArray SpecularTexture,
	float3 WorldPosition,
	float4 Terrain,
	const float3 Weights,
	out float3 BaseColor,
	out float3 Normal,
	out float Smoothness,
	out float3 Emission,
	out float AmbientOcclusion,
	out float3 Specular
) {
	const float3 uv0 = GetTerrainUV(WorldPosition, Terrain, 0);
	const float3 uv1 = GetTerrainUV(WorldPosition, Terrain, 1);
	const float3 uv2 = GetTerrainUV(WorldPosition, Terrain, 2);

	float4 c = GetTerrainColor(
		DiffuseTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2);
	BaseColor = ColorizeSubmergence(c.rgb, WorldPosition.y, Terrain.w);
	HexGridData hgd = GetHexGridData(WorldPosition.xz);
	if (hgd.IsHighlighted()) {
		BaseColor = ApplyHighlight(BaseColor, hgd);
	}
	Normal = GetTerrainColor(
		NormalTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2).xyz;
	Smoothness = GetTerrainColor(
		GlossTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2).x;
	Emission = GetTerrainColor(
		EmissionTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2).xyz;
	AmbientOcclusion = GetTerrainColor(
		AmbientOcclusionTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2).x;
	Specular = GetTerrainColor(
		SpecularTexture, Weights[0], Weights[1], Weights[2],
		uv0, uv1, uv2).xyz;
}