Shader "Island/Ground" {
	Properties {
		_GrassMap ("Grass Texture", 2D) = "white" {}
		_GrassColor ("Grass Color", Color) = (0.24,0.84,0.55,1)
		_DirtMap ("Dirt Texture", 2D) = "white" {}
		_DirtColor ("Dirt Color", Color) = (1,0.55,0.12,1)
		_NoiseMap ("Edge Noise", 2D) = "gray" {}
		_NoiseScale ("Edge Noise Scale", Float) = 0.3
		_GrassDrape ("Grass Drape", Float) = 0.35
		_GrassWobble ("Grass Wobble", Float) = 0.5
		_SkirtBaseY ("Skirt Base Height", Float) = -9999
		_SkirtRise ("Skirt Rise", Float) = 0.25
		_SkirtWobble ("Skirt Wobble", Float) = 0.3
		_SkirtFade ("Skirt Fade (m of ledge)", Float) = 0.25
		_SkirtShare ("Skirt/Drape Max Share of a Step", Float) = 0.4
		_CreaseWidth ("Crease Width at the Foot", Float) = 0.3
		_CreaseRise ("Crease Rise", Float) = 1.5
		_CreaseWobble ("Crease Wobble", Float) = 0.12
		_CreaseAO ("Crease Darkening", Range(0, 1)) = 0.35
		_CreaseAORange ("Crease Darkening Range", Float) = 0.6
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _GrassMap_ST;
			float4 _DirtMap_ST;
			half4 _GrassColor;
			half4 _DirtColor;
			float _NoiseScale;
			float _GrassDrape;
			float _GrassWobble;
			float _SkirtBaseY;
			float _SkirtRise;
			float _SkirtWobble;
			float _SkirtFade;
			float _SkirtShare;
			float _CreaseWidth;
			float _CreaseRise;
			float _CreaseWobble;
			float _CreaseAO;
			float _CreaseAORange;
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

			TEXTURE2D(_GrassMap); SAMPLER(sampler_GrassMap);
			TEXTURE2D(_DirtMap);  SAMPLER(sampler_DirtMap);
			TEXTURE2D(_NoiseMap); SAMPLER(sampler_NoiseMap);

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
				float2 ledge : TEXCOORD1;  // x = distance from the ledge (>= 999: none), y = distance down a step face
				float2 stepInfo : TEXCOORD2;   // x = skirt fade distance, y = step height
				float2 crease0 : TEXCOORD3; // x = crease line coordinate (<= -900: none), y = crease top height
				float2 crease1 : TEXCOORD4; // x = crease foot height, y = coordinate across the crease
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float3 ledgeNoise : TEXCOORD3; // x = ledge.x, yz = edge noise uv
				float3 stepData : TEXCOORD4;   // x = ledge.y, yz = stepInfo
				float fogFactor : TEXCOORD5;
				float4 crease : TEXCOORD6;     // crease0, crease1
				float ao : TEXCOORD7;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input) {
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

				output.ledgeNoise.yz = input.positionOS.xz * _NoiseScale;
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				float ao = ComputeDynamicAO(positionWS, normalWS);

				output.crease = float4(input.crease0, input.crease1);
				output.normalWS = normalWS;
				output.uv = input.uv;
				output.ledgeNoise.x = input.ledge.x;
				output.stepData = float3(input.ledge.y, input.stepInfo);
				output.positionWS = positionWS;
				output.ao = ao;
				return output;
			}

			half4 frag(Varyings input) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				if (_AODebug > 0.5)
					return half4(input.ao.xxx, 1.0);

				float noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, input.ledgeNoise.yz).r - 0.5;
				float drape = noise * _GrassWobble + _GrassDrape;
				float3 grass = SAMPLE_TEXTURE2D(_GrassMap, sampler_GrassMap, TRANSFORM_TEX(input.uv, _GrassMap)).rgb * _GrassColor.rgb;
				float3 dirt = SAMPLE_TEXTURE2D(_DirtMap, sampler_DirtMap, TRANSFORM_TEX(input.uv, _DirtMap)).rgb * _DirtColor.rgb;

				float ledgeDist = input.ledgeNoise.x;
				float stepDist = input.stepData.x;
				float aboveBase = input.positionWS.y - _SkirtBaseY;
				float skirtHeight = noise * _SkirtWobble + _SkirtRise;

				bool onLedge = ledgeDist >= 0.0 && ledgeDist < 999.0;
				bool inBaseSkirt = onLedge && 0.0 < aboveBase && aboveBase < skirtHeight;

				float skirtFade = saturate(input.stepData.y / max(_SkirtFade, 0.001));
				bool hasStep = 0.0 < skirtFade;
				float maxShare = hasStep ? input.stepData.z * _SkirtShare : 1e9;
				drape = min(drape, maxShare);
				skirtHeight = min(skirtHeight, maxShare);
				bool onStep = onLedge && hasStep;
				bool inStepSkirt = onStep && 0.0 < stepDist && stepDist < skirtFade * skirtHeight;

				// Grass crease running up from the foot of a wall.
				float aboveFoot = input.positionWS.y - input.crease.z;
				float creaseT = saturate(1.0 - aboveFoot / max(_CreaseRise, 0.001));
				float creaseWidth = creaseT * creaseT * _CreaseWidth + creaseT * (noise * _CreaseWobble);
				bool inCrease = onLedge && -900.0 < input.crease.x && 0.0 < aboveFoot
					&& input.positionWS.y < input.crease.y + 0.01
					&& abs(input.crease.w - input.crease.x) < creaseWidth;

				bool isGrass = ledgeDist < drape || inBaseSkirt || inStepSkirt || inCrease;
				float3 albedo = isGrass ? grass : dirt;

				// Darken the crease where ground meets the step / skirt base.
				float creaseAORange = max(_CreaseAORange, 1e-4);
				float footShade = onLedge ? saturate(aboveBase / creaseAORange) : 1.0;
				float stepShade = onStep ? saturate(stepDist / creaseAORange) : 1.0;
				float creaseShade = min(stepShade, footShade);
				float creaseFloor = 1.0 - _CreaseAO;
				albedo *= creaseShade * (1.0 - creaseFloor) + creaseFloor;

				float3 normalWS = normalize(input.normalWS);
				float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				Light mainLight = GetMainLight();
				float3 direct = saturate(dot(normalWS, mainLight.direction)) * (shadow * mainLight.color);
				float directAO = _AODirect * (input.ao - 1.0) + 1.0;
				float3 ambient = SampleSH(normalWS) * input.ao;
				float3 lighting = direct * directAO + ambient;

				float3 color = albedo * lighting;
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
