Shader "Player/GhostHand" {
	Properties {
		_BaseMap ("Base Map", 2D) = "white" {}
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_Alpha ("Fade", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half4 _BaseColor;
			half _Alpha;
		CBUFFER_END
		ENDHLSL

		// Depth prepass: writes depth only where the faded hand is at least half visible,
		// so the transparent Ghost pass only shows the front-most surface.
		Pass {
			Name "Depth"
			Tags { "LightMode" = "SRPDefaultUnlit" }
			Blend One Zero, One Zero
			ColorMask 0
			ZWrite On
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			struct Attributes
			{
				float4 positionOS : POSITION;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				half fade : TEXCOORD0;
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
				output.fade = input.color.a * _Alpha;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				clip(input.fade - 0.5);
				return 0;
			}
			ENDHLSL
		}

		Pass {
			Name "Ghost"
			Tags { "LightMode" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				half fade : TEXCOORD2;
				float fogFactor : TEXCOORD3;
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
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.fade = input.color.a * _Alpha;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 normalWS = normalize(input.normalWS);

				// Simple Lambert from the main light (no shadows) plus spherical-harmonic ambient.
				Light mainLight = GetMainLight();
				half3 ambient = SampleSH(normalWS);
				half NdotL = saturate(dot(normalWS, mainLight.direction));
				half3 lighting = mainLight.color * NdotL + ambient;

				half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
				half3 color = MixFog(albedo.rgb * lighting, input.fogFactor);
				return half4(color, albedo.a * input.fade);
			}
			ENDHLSL
		}
	}
}
