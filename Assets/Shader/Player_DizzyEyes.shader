Shader "Player/DizzyEyes" {
	Properties {
		_MainTex ("Eye", 2D) = "white" {}
		[HDR] _Color ("Color", Color) = (2,2,2,1)
		_Eyes ("Eye Centres (left xy, right zw)", Vector) = (0.216,0.514,0.784,0.514)
		_Size ("Size", Range(0.1, 0.9)) = 0.44
		_Spin ("Turns Per Second", Float) = 1.2
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		LOD 0

		Pass {
			Name "Unlit"
			Tags { "LightMode" = "UniversalForward" }
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				half4 _Color;
				float4 _Eyes;
				float _Size;
				float _Spin;
				half _Cutoff;
			CBUFFER_END

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float fogFactor : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				output.uv = input.uv;   // _MainTex_ST is not applied
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Left half of the face UVs uses the left eye centre, right half the right one.
				float2 eyeCentre = (input.uv.x < 0.5) ? _Eyes.xy : _Eyes.zw;
				float2 local = (input.uv - eyeCentre) / _Size;

				// Spin the eye texture around its centre.
				float spinAngle = (_Time.y * _Spin) * 6.2831855;
				float sinSpin, cosSpin;
				sincos(spinAngle, sinSpin, cosSpin);
				float2 rotated = float2(local.x * cosSpin - local.y * sinSpin,
				                        local.y * cosSpin + local.x * sinSpin);

				half4 texel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, rotated + 0.5);

				// Square mask of the rotated quad, then alpha test.
				clip(0.5 - max(abs(rotated.x), abs(rotated.y)));
				clip(texel.a - _Cutoff);

				half3 color = MixFog(texel.rgb * _Color.rgb, input.fogFactor);
				return half4(color, 1.0);
			}
			ENDHLSL
		}
	}
}
