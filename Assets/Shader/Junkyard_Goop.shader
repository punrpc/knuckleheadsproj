Shader "Junkyard/Goop" {
	Properties {
		_DeepColor ("Deep Color", Color) = (0.02,0.22,0.04,1)
		_GoopColor ("Goop Color", Color) = (0.25,0.95,0.15,1)
		[HDR] _GlowColor ("Glow Color", Color) = (0.5,2.4,0.3,1)
		[HDR] _BubbleColor ("Bubble Color", Color) = (1.2,3,0.8,1)
		[HDR] _RimColor ("Rim Glow", Color) = (0.6,2,0.4,1)
		_Scale ("Pattern Scale", Float) = 1.1
		_Flow ("Flow Speed", Float) = 0.25
		_Pulse ("Pulse Speed", Float) = 1.5
		_Bubbles ("Bubble Amount", Range(0, 1)) = 0.35
		_RimPower ("Rim Width", Range(0.5, 8)) = 3
		_Shade ("Sun Shading", Range(0, 1)) = 0.25
		_Wobble ("Surface Wobble (m)", Float) = 0.03
		_WobbleSpeed ("Wobble Speed", Float) = 1.2
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			half4 _DeepColor;
			half4 _GoopColor;
			half4 _GlowColor;
			half4 _BubbleColor;
			half4 _RimColor;
			float _Scale;
			float _Flow;
			float _Pulse;
			float _Bubbles;
			float _RimPower;
			float _Shade;
			float _Wobble;
			float _WobbleSpeed;
		CBUFFER_END

		// Pushes the surface along its normal with a few world-space sine waves.
		// Shared by every pass so shadows and depth match the lit surface.
		float3 GoopWobble(float3 positionOS, float3 normalOS) {
			float2 xz = TransformObjectToWorld(positionOS).xz;
			float diagonal = xz.y + xz.x;
			float2 p = xz * float2(2.3, 1.9);
			float t = _Time.y * _WobbleSpeed;
			float wobble = sin(p.x + t) * cos(p.y + t * 0.8) + sin(diagonal * 3.1 + t * 1.3) * 0.5;
			return positionOS + normalOS * (wobble * _Wobble);
		}
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
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			// xxHash32-style integer hash of a cell, normalised to [0, 1].
			float GoopHash(int2 cell, uint seed) {
				uint h = uint(cell.x) * 374761393u + uint(cell.y) * 668265263u + seed * 2246822519u;
				h = (h ^ (h >> 13)) * 1274126177u;
				h ^= h >> 16;
				return float(h & 16777215u) * (1.0 / 16777215.0);
			}

			float ValueNoise(float2 p) {
				float2 cellF = floor(p);
				float2 f = p - cellF;
				int2 cell = int2(cellF);
				float h00 = GoopHash(cell, 0u);
				float h10 = GoopHash(cell + int2(1, 0), 0u);
				float h01 = GoopHash(cell + int2(0, 1), 0u);
				float h11 = GoopHash(cell + int2(1, 1), 0u);
				float2 u = f * f * (3.0 - 2.0 * f);
				float bottom = lerp(h00, h10, u.x);
				float top = lerp(h01, h11, u.x);
				return lerp(bottom, top, u.y);
			}

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float fogFactor : TEXCOORD2;
				half3 sh : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionOS = GoopWobble(input.positionOS.xyz, input.normalOS);
				float3 positionWS = TransformObjectToWorld(positionOS);
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.normalWS = normalWS;
				output.sh = SampleSH(normalize(normalWS));
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Pattern space: world XZ slanted by height so walls of goop get the pattern too.
				float2 slanted = input.positionWS.y * 0.5 + input.positionWS.xz;
				float flowTime = _Time.y * _Flow;
				float pulseTime = _Time.y * _Pulse;
				float2 p = slanted * _Scale;

				float noiseA = ValueNoise(slanted * _Scale + float2(flowTime, flowTime * 0.6));
				float noiseB = ValueNoise(p * 2.1 - float2(flowTime * 0.7, -flowTime));
				float pattern = noiseB * 0.4 + noiseA * 0.6;

				float glowPulse = sin(pattern * 6.2832 + pulseTime) * 0.35 + 0.65;
				float goop = saturate((pattern - 0.42) * 6.25);
				goop = goop * goop * (3.0 - 2.0 * goop);
				float3 glow = goop * _GlowColor.rgb * glowPulse * 0.35;
				float3 albedo = lerp(_DeepColor.rgb, _GoopColor.rgb, goop);

				float3 normalWS = normalize(input.normalWS);
				Light mainLight = GetMainLight();
				float3 light = saturate(dot(normalWS, mainLight.direction)) * mainLight.color + input.sh;
				float3 shade = _Shade * (light - 1.0) + 1.0;
				float3 color = albedo * shade + glow;

				// Bubbles: one per cell, randomly placed, sized and switched on by _Bubbles.
				float2 bubbleUV = p * 3.0 + float2(0.0, flowTime * 0.5);
				float2 cellF = floor(bubbleUV);
				float2 f = bubbleUV - cellF;
				int2 cell = int2(cellF);
				float2 center = float2(GoopHash(cell, 3u), GoopHash(cell, 4u)) * 0.4 + 0.3;
				float radius = GoopHash(cell, 5u) * 0.14 + 0.1;
				float twinkle = sin(pulseTime * 0.7 + GoopHash(cell, 1u) * 6.2832) * 0.5 + 0.5;
				float bubbleT = saturate((length(f - center) - radius * 0.35) / (radius * 0.65));
				float bubble = 1.0 - bubbleT * bubbleT * (3.0 - 2.0 * bubbleT);
				float spawn = GoopHash(cell, 2u) >= 1.0 - _Bubbles ? 1.0 : 0.0;
				bubble = twinkle * (spawn * bubble);
				color = _BubbleColor.rgb * bubble + color;

				float3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
				float rim = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _RimPower);
				color = _RimColor.rgb * rim + color;

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

				float3 positionWS = TransformObjectToWorld(GoopWobble(input.positionOS.xyz, input.normalOS));
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
				output.positionCS = TransformObjectToHClip(GoopWobble(input.positionOS.xyz, input.normalOS));
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
				output.positionCS = TransformObjectToHClip(GoopWobble(input.positionOS.xyz, input.normalOS));
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
