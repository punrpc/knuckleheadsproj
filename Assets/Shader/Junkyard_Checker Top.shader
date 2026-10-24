Shader "Junkyard/Checker Top" {
	Properties {
		_CheckerMap ("Checker Texture", 2D) = "white" {}
		_TopColor ("Top Color", Color) = (0.24,0.84,0.55,1)
		_CheckerSize ("Checker Texture Size (m)", Float) = 4
		_SideMap ("Side Texture", 2D) = "white" {}
		_SideColor ("Side Color", Color) = (1,0.55,0.12,1)
		_TopFrom ("Top From (normal up)", Range(0, 1)) = 0.6
		_TopSoft ("Top Edge Softness", Range(0.001, 0.5)) = 0.05
		[Toggle(_EXTRA_LIGHTS)] _ExtraLights ("Lit By Point And Spot Lights", Float) = 0
		[HideInInspector] _StaticAO ("Static AO From Vertex Alpha", Float) = 0
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _CheckerMap_ST;
			float4 _SideMap_ST;
			half4 _TopColor;
			half4 _SideColor;
			float _CheckerSize;
			float _TopFrom;
			float _TopSoft;
			float _StaticAO;
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
			#pragma shader_feature_local _EXTRA_LIGHTS
			#pragma multi_compile_local _ _NO_AO
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_CheckerMap); SAMPLER(sampler_CheckerMap);
			TEXTURE2D(_SideMap);    SAMPLER(sampler_SideMap);

			// Ragdoll.VertexAO globals: capsule and box occluders plus debug/strength controls.
			float4 _AOCapA[32];   // xyz = capsule end A, w = radius
			float4 _AOCapB[32];   // xyz = capsule end B
			float4 _AOBox0[32];   // rows of the world-to-box transform, w = box centre component
			float4 _AOBox1[32];
			float4 _AOBox2[32];
			float _AOCapCount;
			float _AOBoxCount;
			float _AOStrength;
			float _AORange;
			float4 _AOBoundsMin;
			float4 _AOBoundsMax;
			float _AODirect;
			float _AODebug;

			// Global height fog (below _HeightFog.x, fully in at _HeightFog.y; z = distance-fog colour blend).
			float4 _HeightFog;
			half4 _HeightFogColor;

			// Two scripted lamps (train headlights) for _EXTRA_LIGHTS.
			float4 _TrainLampPos[2];   // xyz = position, w = range
			float4 _TrainLampDir[2];   // xyz = direction, w = cos of cone (<= -1: omni)
			float4 _TrainLampColor[2];
			float _TrainLamps;

			float ComputeDynamicAO(float3 positionWS, float3 normalWS) {
				if (any(positionWS < _AOBoundsMin.xyz) || any(_AOBoundsMax.xyz < positionWS))
					return 1.0;

				float3 n = normalize(normalWS);
				int capCount = (int)_AOCapCount;
				int boxCount = (int)_AOBoxCount;
				float visibility = 1.0;

				[loop]
				for (int c = 0; c < capCount; c++) {
					float3 capA = _AOCapA[c].xyz;
					float radius = _AOCapA[c].w;
					float3 axis = _AOCapB[c].xyz - capA;
					float t = saturate(dot(positionWS - capA, axis) / max(dot(axis, axis), 1e-6));
					float3 toAxis = (axis * t + capA) - positionWS;
					float distSq = max(dot(toAxis, toAxis), 1e-6);
					float radiusSq = radius * radius;
					float clampedSq = max(radiusSq, distSq);
					float dist = sqrt(clampedSq);
					float cosTheta = saturate(rsqrt(distSq) * dot(n, toAxis));
					float occlusion = radiusSq * cosTheta / clampedSq;
					float fade = saturate((radius * _AORange - dist) / max(radius * _AORange - radius, 1e-4));
					occlusion = fade * occlusion;
					occlusion = dist < _AORange * radius ? occlusion : 0.0;
					visibility *= 1.0 - occlusion;
				}

				float boxFadeRange = max(_AORange - 1.0, 1e-4);
				[loop]
				for (int b = 0; b < boxCount; b++) {
					float3 rel = positionWS - float3(_AOBox0[b].w, _AOBox1[b].w, _AOBox2[b].w);
					float3 local = float3(dot(_AOBox0[b].xyz, rel), dot(_AOBox1[b].xyz, rel), dot(_AOBox2[b].xyz, rel));
					float3 localNormal = float3(
						dot(_AOBox0[b].xyz, n) / dot(_AOBox0[b].xyz, _AOBox0[b].xyz),
						dot(_AOBox1[b].xyz, n) / dot(_AOBox1[b].xyz, _AOBox1[b].xyz),
						dot(_AOBox2[b].xyz, n) / dot(_AOBox2[b].xyz, _AOBox2[b].xyz));
					localNormal = normalize(localNormal);
					float3 toBox = -local;
					float2 clampedSq = max(dot(toBox, toBox).xx, float2(1e-6, 1.0));
					float dist = sqrt(clampedSq.y);
					float cosTheta = saturate(rsqrt(clampedSq.x) * dot(localNormal, toBox));
					float occlusion = cosTheta / clampedSq.y;
					float fade = saturate((_AORange - dist) / boxFadeRange);
					occlusion = fade * occlusion;
					occlusion = dist < _AORange ? occlusion : 0.0;
					visibility *= 1.0 - occlusion;
				}

				return _AOStrength * (visibility - 1.0) + 1.0;
			}

			struct Attributes {
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv : TEXCOORD0;
				half4 color : COLOR; // a = baked static AO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float fogFactor : TEXCOORD3;
				float ao : TEXCOORD4;
				half3 sh : TEXCOORD5;
			#if defined(_EXTRA_LIGHTS)
				half staticAO : TEXCOORD6;
			#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

			#if defined(_NO_AO)
				float ao = 1.0;
			#else
				float ao = ComputeDynamicAO(positionWS, normalWS);
			#endif
				float staticAO = (_AOStrength * _StaticAO) * (input.color.a - 1.0) + 1.0;

				output.sh = staticAO * SampleSH(normalize(normalWS));
				output.ao = _AODebug > 0.5 ? ao * staticAO : ao;
			#if defined(_EXTRA_LIGHTS)
				output.staticAO = staticAO;
			#endif
				output.positionWS = positionWS;
				output.normalWS = normalWS;
				output.uv = input.uv;
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				if (_AODebug > 0.5)
					return half4(input.ao.xxx, 1.0);

				float3 normalWS = normalize(input.normalWS);

				// World-space planar checker on top, mesh-UV side texture on the walls.
				float2 checkerUV = input.positionWS.xz / max(_CheckerSize, 0.001);
				float3 checker = SAMPLE_TEXTURE2D(_CheckerMap, sampler_CheckerMap, TRANSFORM_TEX(checkerUV, _CheckerMap)).rgb;
				float3 side = SAMPLE_TEXTURE2D(_SideMap, sampler_SideMap, TRANSFORM_TEX(input.uv, _SideMap)).rgb * _SideColor.rgb;
				float top = smoothstep(_TopFrom - _TopSoft, _TopFrom + _TopSoft, normalWS.y);
				float3 albedo = lerp(side, checker * _TopColor.rgb, top);

				float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				Light mainLight = GetMainLight();
				float3 direct = saturate(dot(normalWS, mainLight.direction)) * (shadow * mainLight.color);
				float directAO = _AODirect * (input.ao - 1.0) + 1.0;
				float3 lighting = direct * directAO + input.ao * input.sh;

			#if defined(_EXTRA_LIGHTS)
				float3 extraLight = 0.0;
				if (_TrainLamps > 0.5) {
					for (int l = 0; l < 2; l++) {
						float3 toLamp = _TrainLampPos[l].xyz - input.positionWS;
						float2 distSq = max(dot(toLamp, toLamp).xx, float2(1e-4, 0.5));
						float3 lampDir = toLamp * rsqrt(distSq.x);
						float range = max(1.0 - distSq.x / (_TrainLampPos[l].w * _TrainLampPos[l].w), 0.0);
						float cone = saturate((dot(-lampDir, _TrainLampDir[l].xyz) - _TrainLampDir[l].w) / ((1.0 - _TrainLampDir[l].w) * 0.5));
						cone = cone * cone * (3.0 - 2.0 * cone);
						float spot = _TrainLampDir[l].w <= -1.0 ? 1.0 : cone;
						float attenuation = spot * (range * range) / distSq.y;
						extraLight += attenuation * _TrainLampColor[l].rgb * saturate(dot(normalWS, lampDir));
					}
				}
				#if defined(_ADDITIONAL_LIGHTS)
					uint lightCount = GetAdditionalLightsCount();
					for (uint i = 0u; i < lightCount; i++) {
						Light light = GetAdditionalLight(i, input.positionWS);
						extraLight += (light.distanceAttenuation * light.color) * saturate(dot(normalWS, light.direction));
					}
				#endif
				lighting = (extraLight * input.staticAO) * directAO + lighting;
			#endif

				float3 color = lighting * albedo;
				color = MixFog(color, input.fogFactor);

				// With _HeightFog.z > 0 the height-fog colour blends toward the distance-fog colour far from the camera.
				float distanceFog = length(input.positionWS - _WorldSpaceCameraPos) * unity_FogParams.x;
				distanceFog = (1.0 - exp2(-distanceFog * distanceFog)) * _HeightFog.z;
				float3 heightFogColor = 0.0 < _HeightFog.z ? lerp(_HeightFogColor.rgb, unity_FogColor.rgb, distanceFog) : _HeightFogColor.rgb;

				float heightFog = saturate((_HeightFog.x - input.positionWS.y) / max(_HeightFog.x - _HeightFog.y, 0.01));
				heightFog = heightFog * heightFog * (3.0 - 2.0 * heightFog) * _HeightFogColor.a;
				color = lerp(color, heightFogColor, heightFog);
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
