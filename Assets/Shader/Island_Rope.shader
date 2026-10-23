Shader "Island/Rope" {
	Properties {
		_TopColor ("Top Color", Color) = (0.82,0.7,0.52,1)
		_BottomColor ("Bottom Color", Color) = (0.58,0.46,0.32,1)
		_Gradient ("Gradient Length", Range(0.05, 1)) = 0.6
		_Wrap ("Light Wrap", Range(0, 1)) = 0.3
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			half4 _TopColor;
			half4 _BottomColor;
			float _Gradient;
			float _Wrap;
		CBUFFER_END
		ENDHLSL

		Pass {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
			Blend One Zero, One Zero
			ZWrite On
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

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float gradient : TEXCOORD2; // uv.x runs along the rope's length
				float fogFactor : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				output.positionWS = positionWS;
				output.gradient = input.uv.x;
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				Light mainLight = GetMainLight();
				float3 lightColor = shadow * mainLight.color;

				float3 normalWS = normalize(input.normalWS);
				float3 ambient = SampleSH(normalWS);

				// Wrapped Lambert: light bleeds _Wrap past the terminator.
				float wrapped = saturate((dot(normalWS, mainLight.direction) + _Wrap) / (_Wrap + 1.0));
				float3 lighting = lightColor * wrapped + ambient;

				// Top colour for the first (1 - _Gradient) of the rope, blending to the bottom colour after.
				float t = saturate((input.gradient - (1.0 - _Gradient)) / max(_Gradient, 0.001));
				float3 albedo = lerp(_TopColor.rgb, _BottomColor.rgb, t);

				float3 color = lighting * albedo;
				color = MixFog(color, input.fogFactor);
				return half4(color, 1.0);
			}
			ENDHLSL
		}

		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual
			ColorMask 0
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			float3 _LightDirection;
			float3 _LightPosition;

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
			#if _CASTING_PUNCTUAL_LIGHT_SHADOW
				float3 lightDirectionWS = normalize(_LightPosition - positionWS);
			#else
				float3 lightDirectionWS = _LightDirection;
			#endif
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
				output.positionCS = ApplyShadowClamping(positionCS);
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				return 0;
			}
			ENDHLSL
		}

		Pass {
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }
			ZWrite On
			ZTest LEqual
			ColorMask R
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			struct Attributes {
				float4 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				return output;
			}

			half frag(Varyings input) : SV_Target {
				return input.positionCS.z;
			}
			ENDHLSL
		}

		Pass {
			Name "DepthNormals"
			Tags { "LightMode" = "DepthNormals" }
			ZWrite On
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				return half4(normalize(input.normalWS), 0.0);
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
