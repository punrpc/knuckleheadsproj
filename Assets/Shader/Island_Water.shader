Shader "Island/Water" {
	Properties {
		_ShallowColor ("Shallow Color", Color) = (0.16,0.76,0.7,1)
		_DeepColor ("Deep Color", Color) = (0.1,0.52,0.6,1)
		_DepthRange ("Depth Range", Float) = 8
		_Clarity ("Clarity", Range(0, 1)) = 0.5
		_ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.35
		_FoamColor ("Foam Color", Color) = (1,1,1,1)
		_FoamDistance ("Foam Distance", Float) = 0.35
		_FoamNoiseScale ("Foam Wobble Scale", Float) = 0.8
		_FoamSpeed ("Foam Speed", Float) = 0.15
		_NoiseMap ("Foam Noise", 2D) = "gray" {}
		_CausticColor ("Caustic Color", Color) = (1,1,1,1)
		_CausticStrength ("Caustic Strength", Range(0, 1)) = 0.45
		_CausticMap ("Caustic Map (two Voronoi layers in R and G)", 2D) = "black" {}
		_CausticScale ("Caustic Scale", Float) = 0.4
		_CausticWidth ("Caustic Width", Range(0.01, 1)) = 0.4
		_CausticChop ("Caustic Chop", Range(0, 1)) = 0.7
		_CausticSpeed ("Caustic Speed", Float) = 0.3
		_SparkleAmount ("Sparkle Amount", Range(0, 1)) = 0.25
		_SparkleStrength ("Sparkle Strength", Range(0, 1)) = 0.6
		_SparkleScale ("Sparkle Scale", Float) = 0.5
		_SparkleSpeed ("Sparkle Speed", Float) = 0.15
	}
	SubShader {
		Tags { "IgnoreProjector" = "True" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }

		Pass {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _ShallowColor;
				half4 _DeepColor;
				half4 _FoamColor;
				half4 _CausticColor;
				float _DepthRange;
				half _Clarity;
				half _ShadowStrength;
				float _FoamDistance;
				float _FoamNoiseScale;
				float _FoamSpeed;
				half _CausticStrength;
				float _CausticScale;
				float _CausticWidth;
				half _CausticChop;
				float _CausticSpeed;
				half _SparkleAmount;
				half _SparkleStrength;
				float _SparkleScale;
				float _SparkleSpeed;
			CBUFFER_END

			TEXTURE2D(_NoiseMap);   SAMPLER(sampler_NoiseMap);
			TEXTURE2D(_CausticMap); SAMPLER(sampler_CausticMap);

			// Integer cell hash (2D); returns the full 32-bit mix.
			uint HashCell(float2 cell) {
				int2 c = int2(cell);
				uint2 h = uint2(c) * uint2(2376512323u, 3625334849u) + uint2(875560960u, 2215641088u);
				uint x = h.y ^ h.x;
				x ^= x >> 15;
				x *= 739982445u;
				x ^= x >> 12;
				x *= 695872825u;
				x ^= x >> 15;
				return x;
			}

			// Second, decorrelated value from the same cell hash.
			uint RehashCell(uint x) {
				x *= 2654435761u;
				return x ^ (x >> 16);
			}

			struct Attributes {
				float4 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float eyeDepth : TEXCOORD1;
				float fogFactor : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				output.positionWS = positionWS;
				output.eyeDepth = -TransformWorldToView(positionWS).z;
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Water thickness behind this pixel, converted to an approximate vertical depth.
				float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
				float sceneEyeDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
				float3 viewDirWS = normalize(input.positionWS - _WorldSpaceCameraPos);
				float3 cameraForward = GetViewForwardDir();
				float2 thickness = max(float2(sceneEyeDepth - input.eyeDepth, dot(viewDirWS, cameraForward)), float2(0.0, 0.01));
				float rayLength = thickness.x / thickness.y;
				float steepness = max(-viewDirWS.y, 0.35);
				float waterDepth = steepness * rayLength;
				float deep = saturate(waterDepth / _DepthRange);

				// Shoreline foam: a hard wobbly edge plus a soft wider band.
				float foamTime = _Time.y * _FoamSpeed;
				float2 foamUV = input.positionWS.xz * _FoamNoiseScale + foamTime * float2(0.3, 0.2);
				float foamNoise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, foamUV).r + 0.5;
				float foamEdge = _FoamDistance * foamNoise;
				float foam = 1.0 - smoothstep(foamEdge - 0.05, foamEdge, waterDepth);
				float2 band = (_FoamDistance * 1.6) * foamNoise * float2(0.4, 0.6);
				float bandT = saturate((waterDepth - band.x) / band.y);
				float foamBand = 1.0 - bandT * bandT * (3.0 - 2.0 * bandT);

				// Caustics: two scrolling Voronoi layers, chopped up by the noise map.
				float caustic;
				if (_CausticStrength > 0.001) {
					float4 causticUV = input.positionWS.xzxz * _CausticScale;
					float causticTime = _Time.y * _CausticSpeed;
					float2 uvA = causticUV.xy * (1.0 / 12.0) + causticTime * float2(0.025, 0.014);
					float cellA = SAMPLE_TEXTURE2D(_CausticMap, sampler_CausticMap, uvA).r * 0.5;
					float2 uvB = causticUV.zw * (2.0 / 15.0) + 0.61 - causticTime * float2(0.016, -0.022);
					float cellB = SAMPLE_TEXTURE2D(_CausticMap, sampler_CausticMap, uvB).g * 0.5;
					float lineA = 1.0 - smoothstep(0.0, _CausticWidth, cellA);
					float lineB = 1.0 - smoothstep(0.0, _CausticWidth, cellB);
					caustic = lineA * 0.75 + lineB * 0.25;

					float2 chopUV = input.positionWS.xz * 0.25 + _Time.y * float2(-0.04, 0.03);
					float chopNoise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, chopUV).r;
					float chopFrom = _CausticChop * 0.7 - 0.15;
					float chop = saturate((chopNoise - chopFrom) * (1.0 / 0.3));
					chop = chop * chop * (3.0 - 2.0 * chop);
					caustic *= chop;
				} else {
					caustic = 0.0;
				}

				// Sparkles: one randomly placed, stretched glint per grid cell.
				float sparkle;
				if (_SparkleStrength > 0.001) {
					float sparkleTime = _Time.y * _SparkleSpeed;
					float2 p = input.positionWS.xz * _SparkleScale + sparkleTime * float2(0.6, 0.25);
					float2 cell = floor(p);
					float2 f = frac(p);

					uint hashA = HashCell(floor(cell + 0.5));
					uint hashB = HashCell(floor(cell + 5.6));
					float2 center = float2(hashA & 65535u, RehashCell(hashA) & 65535u) * (0.8 / 65535.0) + 0.1;
					float spawn = float(hashB & 65535u) * (1.0 / 65535.0);
					float size = float(RehashCell(hashB) & 65535u) * (1.0 / 65535.0) + 0.4;

					float2 d = f - center;
					float on = spawn >= 1.0 - _SparkleAmount ? 1.0 : 0.0;
					float dist = length(d * float2(0.18, 1.0));
					float glintT = saturate((dist - size * 0.018) / (size * 0.012));
					float glint = 1.0 - glintT * glintT * (3.0 - 2.0 * glintT);

					uint hashC = HashCell(floor(cell + 10.2));
					float brightness = float(hashC & 65535u) * (0.5 / 65535.0) + 0.5;
					sparkle = brightness * (on * glint) * _SparkleStrength;
				} else {
					sparkle = 0.0;
				}

				float3 water = lerp(_ShallowColor.rgb, _DeepColor.rgb, deep);
				float3 causticColor = caustic * _CausticColor.rgb * _CausticStrength;
				water += causticColor * (1.0 - deep * 0.7);

				float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				shadow = lerp(1.0, shadow, _ShadowStrength);
				float3 color = water * shadow;
				color = lerp(color, _FoamColor.rgb, foamBand * 0.15);
				color += sparkle;
				color = lerp(color, _FoamColor.rgb, foam);

				float opacity = lerp(1.0 - _Clarity, 1.0, deep);
				float alpha = max(max(foam, sparkle), opacity);

				color = MixFog(color, input.fogFactor);
				return half4(color, alpha);
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
