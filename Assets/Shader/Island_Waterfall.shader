Shader "Island/Waterfall" {
	Properties {
		[HDR] _Bright ("Streak Color", Color) = (0.5,1.6,0.32,1)
		_Deep ("Deep Color", Color) = (0.06,0.38,0.05,1)
		[HDR] _Lip ("Lip Foam", Color) = (0.8,2.2,0.6,1)
		_Speed ("Fall Speed (m/s)", Float) = 4
		_Across ("Streaks Per Metre", Float) = 2.4
		_Along ("Streak Length", Float) = 0.22
		_Alpha ("Opacity", Range(0, 1)) = 0.92
		_Edge ("Side Fade", Range(0.01, 0.5)) = 0.07
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
		LOD 0

		Pass {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _Bright;
				half4 _Deep;
				half4 _Lip;
				float _Speed;
				float _Across;
				float _Along;
				float _Alpha;
				float _Edge;
			CBUFFER_END

			// Height fog (set globally by the game): x = fog top height, y = fully fogged height,
			// z = how much the fog colour blends toward the distance-fog colour far from the camera.
			float4 _HeightFog;
			half4 _HeightFogColor;

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;     // x = across the fall (m), y = down the fall (m)
				float2 uv1 : TEXCOORD1;    // x = 0..1 across (side fade), y = 0..1 down (top fade)
				float2 uv2 : TEXCOORD2;    // x = distance from the lip (m)
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float2 uv1 : TEXCOORD2;
				float fogFactor : TEXCOORD3;
				float lip : TEXCOORD4;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// 24-bit integer hash of a lattice cell (rows wrap every 64 cells), mapped to [0, 1).
			float CellHash(int2 cell, uint seed)
			{
				uint h = (uint)cell.x * 374761393u + (uint)((cell.y & 63) * 668265263) + seed;
				h ^= h >> 13u;
				h *= 1274126177u;
				h ^= h >> 16u;
				return (float)(h & 16777215u) * 5.9604645e-08;
			}

			float ValueNoise(float2 p, uint seed)
			{
				float2 cellF = floor(p);
				int2 cell = (int2)cellF;
				float2 f = p - cellF;
				float2 w = f * f * (3.0 - 2.0 * f);
				float n00 = CellHash(cell, seed);
				float n10 = CellHash(cell + int2(1, 0), seed);
				float n01 = CellHash(cell + int2(0, 1), seed);
				float n11 = CellHash(cell + int2(1, 1), seed);
				float bottom = lerp(n00, n10, w.x);
				float top = lerp(n01, n11, w.x);
				return lerp(bottom, top, w.y);
			}

			float3 ApplyHeightFog(float3 color, float3 positionWS)
			{
				// Far from the camera the height fog takes on the distance-fog colour.
				float distanceFog = length(positionWS - _WorldSpaceCameraPos) * unity_FogParams.x;
				distanceFog = (1.0 - exp2(-distanceFog * distanceFog)) * _HeightFog.z;
				float3 fogColor = 0.0 < _HeightFog.z ? lerp(_HeightFogColor.rgb, unity_FogColor.rgb, distanceFog) : _HeightFogColor.rgb;

				float fog = saturate((_HeightFog.x - positionWS.y) / max(_HeightFog.x - _HeightFog.y, 0.01));
				fog = fog * fog * (3.0 - 2.0 * fog) * _HeightFogColor.a;
				return lerp(color, fogColor, fog);
			}

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
				output.positionCS = positionInputs.positionCS;
				output.positionWS = positionInputs.positionWS;
				output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
				output.uv = input.uv;
				output.uv1 = input.uv1;
				output.lip = input.uv2.x;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Streak lattice scrolls down the fall; the scroll wraps every 64 cells (the hash rows wrap at 64).
				float cellsAlong = _Across * _Along;
				float scroll = frac(cellsAlong * (_Time.y * _Speed) * 0.015625) * 64.0;
				float2 p;
				p.y = input.uv.y * cellsAlong - scroll + 64.0;
				p.x = input.uv.x * _Across;

				float coarse = ValueNoise(p, 0u);
				float fine = ValueNoise(float2(p.x * 2.3 + 17.3, p.y * 2.0), 2246822519u);
				float streak = saturate((fine * 0.4 + coarse * 0.6 - 0.38) * 2.5000002);
				streak = streak * streak * (3.0 - 2.0 * streak);

				float3 color = lerp(_Deep.rgb, _Bright.rgb, streak);
				float streakAlpha = streak * 0.25 + 0.75;

				// Foam right below the lip.
				float lip = min(abs(input.lip) * 2.2222223, 1.0);
				lip = (1.0 - lip * lip * (3.0 - 2.0 * lip)) * 0.7;
				color = lerp(color, _Lip.rgb, lip);

				color = MixFog(color, input.fogFactor);
				color = ApplyHeightFog(color, input.positionWS);

				// Soft sides, soft top, and fade out where the fall drops into the fully fogged layer.
				float invEdge = 1.0 / _Edge;
				float sideA = saturate(invEdge * (1.0 - input.uv1.x));
				float sideB = saturate(invEdge * input.uv1.x);
				sideA = sideA * sideA * (3.0 - 2.0 * sideA);
				sideB = sideB * sideB * (3.0 - 2.0 * sideB);
				float alpha = sideA * sideB * _Alpha;
				float topFade = saturate(input.uv1.y * 25.0);
				topFade = topFade * topFade * (3.0 - 2.0 * topFade);
				alpha = topFade * alpha * streakAlpha;
				float bottomFade = saturate((input.positionWS.y - _HeightFog.y) * 4.0);
				return half4(color, bottomFade * alpha);
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
