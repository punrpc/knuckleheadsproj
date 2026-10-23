Shader "Island/TVScreen" {
	Properties {
		_MainTex ("Screen", 2D) = "black" {}
		_OffTop ("Off Top", Color) = (0.2,0.25,0.3,1)
		_OffBottom ("Off Bottom", Color) = (0.1,0.12,0.15,1)
		_Power ("Power", Range(0, 1)) = 0
		_Cells ("Pixel Cells", Vector) = (320,296,0,0)
		_Gap ("Pixel Gap", Range(0, 1)) = 0.14
		_Glow ("Glow", Range(0.5, 2)) = 1.1
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
		LOD 0

		Pass {
			Name "Unlit"
			Tags { "LightMode"="UniversalForward" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
			Blend One Zero, One Zero
			BlendOp Add
			ZTest LEqual
			ZWrite On
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				half4 _OffTop;
				half4 _OffBottom;
				float _Power;
				float4 _Cells;
				float _Gap;
				float _Glow;
			CBUFFER_END

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv         : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv         : TEXCOORD0;
				float  fogFactor  : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				output.uv = input.uv;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.uv;

				// Pixel grid: darken the gaps between cells.
				float2 cell = uv * _Cells.xy;
				float2 cellLocal = frac(cell) - 0.5;
				float cellEdge = smoothstep(0.5, 0.38, max(abs(cellLocal.y), abs(cellLocal.x)));
				float pixelMask = lerp(1.0 - _Gap, 1.0, cellEdge);

				// CRT power-on: a horizontal line that opens up vertically.
				float openT = saturate(_Power * 2.0 - 1.0);
				float open = openT * openT * (3.0 - 2.0 * openT);
				float halfHeight = open * 0.488 + 0.012;
				float2 centered = uv - 0.5;
				float2 screenUV = float2(uv.x, centered.y * (0.5 / halfHeight) + 0.5);
				float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, screenUV).rgb * _Glow;
				color = pixelMask * color;

				// Scanline flicker.
				float flicker = sin(cell.y * 3.14159 + _Time.y * 4.0) * 0.04 + 0.96;
				float lineT = saturate(_Power + _Power);
				float flash = lineT * (1.0 - openT);
				color = lerp(color * flicker, float3(1.0, 1.0, 0.95), flash);

				// Corner vignette.
				float radius = min(length(abs(centered)) * 1.4, 1.0);
				float vignette = 1.0 - radius * radius * radius * 0.12;

				// Off-screen gradient outside the lit area.
				float3 offColor = lerp(_OffBottom.rgb, _OffTop.rgb, saturate(uv.y));
				float halfWidth = lineT * lineT * (3.0 - 2.0 * lineT) * 0.5;
				float inside = (halfWidth >= abs(centered.x) ? 1.0 : 0.0) * (halfHeight >= abs(centered.y) ? 1.0 : 0.0);
				color = lerp(offColor, color * vignette, inside);

				color = MixFog(color, input.fogFactor);
				return half4(color, 1.0);
			}
			ENDHLSL
		}
	}
}
