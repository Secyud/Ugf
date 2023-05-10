#include "HexCellData.hlsl"
#include "Water.hlsl"

void GetFragmentDataEstuary_float (
	UnityTexture2D NoiseTexture,
	const float2 RiverUV,
	float2 ShoreUV,
	float3 WorldPosition,
	const float4 Color,
	out float3 BaseColor,
	out float Alpha
) {
	const float shore = ShoreUV.y;
	const float foam = Foam(shore, WorldPosition.xz, NoiseTexture);
	float waves = Waves(WorldPosition.xz, NoiseTexture);
	waves *= 1 - shore;
	const float shoreWater = max(foam, waves);
	const float river = River(RiverUV, NoiseTexture);

	const float water = lerp(shoreWater, river, ShoreUV.x);
	float4 c = saturate(Color + water);
	BaseColor = c.rgb;
	Alpha = c.a;
}

void GetFragmentDataRoad_float (
	float3 WorldPosition,
	out float2 UV
) {
	UV = WorldPosition.xz * (3 * TILING_SCALE);
}

void GetFragmentDataRiver_float (
	const UnityTexture2D NoiseTexture,
	const float2 RiverUV,
	const float4 Color,
	out float3 BaseColor,
	out float Alpha
) {
	const float river = River(RiverUV, NoiseTexture);
	float4 c = saturate(Color + river);
	BaseColor = c.rgb;
	Alpha = river;
}

void GetFragmentDataWater_float (
	UnityTexture2D NoiseTexture,
	float3 WorldPosition,
	float4 Color,
	out float3 BaseColor,
	out float Alpha
) {
	const float waves = Waves(WorldPosition.xz, NoiseTexture);
	float4 c = saturate(Color + waves);

	BaseColor = c.rgb;
	Alpha = c.a;
}

void GetFragmentDataShore_float (
	UnityTexture2D NoiseTexture,
	float2 ShoreUV,
	float3 WorldPosition,
	float4 Color,
	out float3 BaseColor,
	out float Alpha
) {
	const float shore = ShoreUV.y;
	const float foam = Foam(shore, WorldPosition.xz, NoiseTexture);
	float waves = Waves(WorldPosition.xz, NoiseTexture);
	waves *= 1 - shore;
	float4 c = saturate(Color + max(foam, waves));

	BaseColor = c.rgb;
	Alpha = c.a;
}