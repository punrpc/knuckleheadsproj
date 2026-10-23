Shader "Island/Vignette" {
	Properties {
		_Color ("Color", Color) = (0,0,0,1)
		_Radius ("Clear radius", Range(0, 1.6)) = 0.75
		_Soft ("Softness", Range(0.01, 1.2)) = 0.45
		_Aspect ("Aspect", Range(0.5, 2)) = 1.15
		_Blur ("Blur", Range(0, 0.1)) = 0
		_Dim ("Edge dim", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "IgnoreProjector"="true" "Queue"="Transparent+400" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
		LOD 0

		Pass {
			Name "Vignette"
			Tags { "IgnoreProjector"="true" "LightMode"="UniversalForward" "Queue"="Transparent+400" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			BlendOp Add
			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _Color;
				float _Radius;
				float _Soft;
				float _Aspect;
				float _Blur;
				float _Dim;
			CBUFFER_END

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
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// Drawn on a unit quad that is stretched straight over the whole view (no matrices).
			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = float4(input.positionOS.xy * 2.0, 1.0, 1.0);
				output.uv = input.uv;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Centre on the eye's optical axis (the projection's off-centre terms).
				float2 center = float2(UNITY_MATRIX_P[0].z, UNITY_MATRIX_P[1].z) * -0.5 + 0.5;
				float2 offset = input.uv - center;
				offset.x *= _Aspect;
				float edge = saturate((length(offset) * 2.0 - _Radius) / _Soft);
				edge = edge * edge * (3.0 - 2.0 * edge);

				if (_Blur <= 0.0001)
					return half4(_Color.rgb, saturate(edge * _Color.a));

				// Blurred, dimmed view of the scene towards the edges: centre tap + two rings of six.
				float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
				float blurRadius = (edge * 0.65 + 0.35) * _Blur;
				float3 sum = SampleSceneColor(screenUV);
				const float angleStep = TWO_PI / 12.0;
				[unroll]
				for (int i = 0; i < 12; i++)
				{
					float angle = i * angleStep;
					float ring = (i & 1) ? 1.0 : 0.5;
					float2 tap = float2(cos(angle) / _Aspect, sin(angle)) * ring;
					sum += SampleSceneColor(tap * blurRadius + screenUV);
				}
				float3 blurred = sum * (1.0 / 13.0);

				float dim = 1.0 - saturate(edge * _Dim);
				return half4(dim * blurred, 1.0);
			}
			ENDHLSL
		}
	}
}
