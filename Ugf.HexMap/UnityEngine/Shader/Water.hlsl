// #include <UnityShaderVariables.cginc>
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

float Foam (float shore, float2 worldXZ, UnityTexture2D noiseTex) {
	shore = sqrt(shore) * 0.9;

	const float2 noiseUV = worldXZ + _Time.y * 0.25;
	float4 noise = noiseTex.Sample(noiseTex.samplerstate, noiseUV * (2 * TILING_SCALE));

	const float distortion1 = noise.x * (1 - shore);
	float foam1 = sin((shore + distortion1) * 10 - _Time.y);
	foam1 *= foam1;

	const float distortion2 = noise.y * (1 - shore);
	float foam2 = sin((shore + distortion2) * 10 + _Time.y + 2);
	foam2 *= foam2 * 0.7;

	return max(foam1, foam2) * shore;
}

float River (const float2 riverUV, UnityTexture2D noiseTex) {
	float2 uv = riverUV;
	uv.y = uv.y * 0.0625 + _Time.y * 0.005;
	uv.x -= _Time.y * 0.25;
	float4 noise = noiseTex.Sample(noiseTex.samplerstate, uv);

	float2 uv2 = riverUV;
	uv2.y = uv2.y * 0.0625 - _Time.y * 0.0052;
	uv2.x -= _Time.y * 0.23;
	float4 noise2 = noiseTex.Sample(noiseTex.samplerstate, uv2);

	return noise.r * noise2.w;
}

float Waves (float2 worldXZ, UnityTexture2D noiseTex) {
	float2 uv1 = worldXZ;
	uv1.y += _Time.y;
	float4 noise1 = noiseTex.Sample(noiseTex.samplerstate, uv1 * (3 * TILING_SCALE));

	float2 uv2 = worldXZ;
	uv2.x += _Time.y;
	float4 noise2 = noiseTex.Sample(noiseTex.samplerstate, uv2 * (3 * TILING_SCALE));

	float blendWave = sin(
		(worldXZ.x + worldXZ.y) * 0.1 +
		(noise1.y + noise2.z) + _Time.y
	);
	blendWave *= blendWave;

	const float waves =
		lerp(noise1.z, noise1.w, blendWave) +
		lerp(noise2.x, noise2.y, blendWave);
	return smoothstep(0.75, 2, waves);
}