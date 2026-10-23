Shader "Knuckleheads/ClothDecal" {
	Properties {
		_BaseMap ("Texture", 2D) = "white" {}
		_BaseColor ("Color", Color) = (1,1,1,1)
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		[HideInInspector] _CamClip ("Hide Within (m of the camera)", Float) = 0
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half4 _BaseColor;
			half _Cutoff;
			float _CamClip;
		CBUFFER_END

		TEXTURE2D(_BaseMap);
		SAMPLER(sampler_BaseMap);

		// Cosmetics.cs sets _CamClip (via a MaterialPropertyBlock) to hide the decal
		// when the camera is inside the wearer's head.
		void ClipNearCamera(float3 positionWS)
		{
			float distanceToCamera = length(positionWS - _WorldSpaceCameraPos) - _CamClip;
			clip((_CamClip > 0.0 && distanceToCamera < 0.0) ? -1.0 : 1.0);
		}
		ENDHLSL

		Pass {
			Name "Forward"
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
				float3 positionWS : TEXCOORD1;
				float fogFactor : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				ClipNearCamera(input.positionWS);

				half4 texel = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
				half alpha = texel.a * _BaseColor.a;
				half3 color = texel.rgb * _BaseColor.rgb;
				clip(alpha - _Cutoff);

				color = MixFog(color, input.fogFactor);
				return half4(color, 1.0);
			}
			ENDHLSL
		}

		Pass {
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }
			Blend One Zero, One Zero
			ColorMask R
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
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 positionWS : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				ClipNearCamera(input.positionWS);

				half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
				clip(alpha * _BaseColor.a - _Cutoff);
				return 0;
			}
			ENDHLSL
		}
	}
}
