Shader "Island/Gradient" {
	Properties {
		_TopColor ("Top Color", Color) = (0.95,0.85,0.55,1)
		_BottomColor ("Bottom Color", Color) = (0.75,0.45,0.2,1)
		_Direction ("Gradient Direction", Vector) = (0,1,0,0)
		[Toggle] _WorldSpace ("World Space", Float) = 0
		_Bottom ("Bottom (along direction)", Float) = -0.5
		_Top ("Top (along direction)", Float) = 0.5
		_Wrap ("Soft Light Wrap", Range(0, 1)) = 0.5
		_Ambient ("Ambient Boost", Range(0, 2)) = 1
		[Toggle(_FADE_TOP)] _FadeTop ("Fade Out At Top", Float) = 0
		_FadeStart ("Fade Start (world height)", Float) = 12
		_FadeEnd ("Fade End (world height)", Float) = 20
		[HideInInspector] _MeshGradient ("Gradient From Mesh UV3", Float) = 0
		[HideInInspector] _CamClip ("Hide Within (m of the camera)", Float) = 0
		[HideInInspector] _StaticAO ("Static AO From Vertex Alpha", Float) = 0
		[Toggle] _KeepColor ("Keep Color On Clothes", Float) = 0
		[Toggle(_PIXEL_LIGHTS)] _PixelLights ("Point And Spot Lights Per Pixel", Float) = 0
		[HideInInspector] _SrcBlend ("Src Blend", Float) = 1
		[HideInInspector] _DstBlend ("Dst Blend", Float) = 0
		[HideInInspector] _ZWrite ("Z Write", Float) = 1
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			half4 _TopColor;
			half4 _BottomColor;
			float4 _Direction;
			float _WorldSpace;
			float _Bottom;
			float _Top;
			float _Wrap;
			float _Ambient;
			float _FadeStart;
			float _FadeEnd;
			float _MeshGradient;
			float _CamClip;
			float _StaticAO;
			float _KeepColor;
		CBUFFER_END

		// Cosmetics.NearClip: hide the part of a worn item that is within _CamClip metres of the camera.
		void CameraClip(float3 positionWS)
		{
			#if defined(_CAM_CLIP)
				if (_CamClip > 0.0)
					clip(distance(positionWS, _WorldSpaceCameraPos) - _CamClip);
			#endif
		}
		ENDHLSL

		Pass {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
			Blend [_SrcBlend] [_DstBlend], [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fog
			#pragma multi_compile _ _NO_AO
			#pragma multi_compile _ _NO_SHADOWS
			#pragma shader_feature_local _FADE_TOP
			#pragma shader_feature_local _PIXEL_LIGHTS
			#pragma shader_feature_local _CAM_CLIP
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			// Dynamic vertex AO from capsule and box occluders (set globally by the game).
			float4 _AOCapA[32];   // xyz = capsule end A, w = radius
			float4 _AOCapB[32];   // xyz = capsule end B
			float4 _AOBox0[32];   // rows of the world-to-box transform, w = box centre component
			float4 _AOBox1[32];
			float4 _AOBox2[32];
			float _AOCapCount;
			float _AOBoxCount;
			float _AOStrength;
			float _AODebug;
			float _AORange;
			float4 _AOBoundsMin;
			float4 _AOBoundsMax;
			float _AODirect;

			// Two train headlamps (point or spot), set globally by the game.
			float4 _TrainLampPos[2];   // xyz = position, w = range
			float4 _TrainLampDir[2];   // xyz = spot direction, w = cos(outer angle), <= -1 for a point light
			float4 _TrainLampColor[2];
			float _TrainLamps;

			// Height fog below a water/cloud plane: x = fog top height, y = fully fogged height.
			float4 _HeightFog;
			half4 _HeightFogColor;

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv3 : TEXCOORD3;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float gradient : TEXCOORD2;
				float fogFactor : TEXCOORD3;
				float ao : TEXCOORD4;
				half3 sh : TEXCOORD5;
				half3 vertexLight : TEXCOORD6;   // static AO only, with _PIXEL_LIGHTS
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float WrapDiffuse(float3 normalWS, float3 lightDirWS)
			{
				return saturate((dot(normalWS, lightDirWS) + _Wrap) / (_Wrap + 1.0));
			}

			float DynamicAO(float3 positionWS, float3 normalWS)
			{
				if (any(positionWS < _AOBoundsMin.xyz) || any(_AOBoundsMax.xyz < positionWS))
					return 1.0;

				int capCount = (int)_AOCapCount;
				int boxCount = (int)_AOBoxCount;
				float visibility = 1.0;
				for (int c = 0; c < capCount; c++)
				{
					float3 a = _AOCapA[c].xyz;
					float3 ab = _AOCapB[c].xyz - a;
					float t = saturate(dot(positionWS - a, ab) / max(dot(ab, ab), 1e-6));
					float3 toAxis = (ab * t + a) - positionWS;
					float dist2 = max(dot(toAxis, toAxis), 1e-6);
					float radius = _AOCapA[c].w;
					float radius2 = radius * radius;
					float clampedDist2 = max(dist2, radius2);
					float dist = sqrt(clampedDist2);
					float occlusion = radius2 * saturate(dot(normalWS, toAxis) * rsqrt(dist2)) / clampedDist2;
					occlusion *= saturate((radius * _AORange - dist) / max(radius * _AORange - radius, 1e-4));
					occlusion = dist < _AORange * radius ? occlusion : 0.0;
					visibility *= 1.0 - occlusion;
				}

				float boxFade = max(_AORange - 1.0, 1e-4);
				for (int b = 0; b < boxCount; b++)
				{
					float3 rel = positionWS - float3(_AOBox0[b].w, _AOBox1[b].w, _AOBox2[b].w);
					float3 local = float3(dot(_AOBox0[b].xyz, rel), dot(_AOBox1[b].xyz, rel), dot(_AOBox2[b].xyz, rel));
					float3 localNormal = float3(
						dot(_AOBox0[b].xyz, normalWS) / dot(_AOBox0[b].xyz, _AOBox0[b].xyz),
						dot(_AOBox1[b].xyz, normalWS) / dot(_AOBox1[b].xyz, _AOBox1[b].xyz),
						dot(_AOBox2[b].xyz, normalWS) / dot(_AOBox2[b].xyz, _AOBox2[b].xyz));
					localNormal = normalize(localNormal);
					float2 dist2 = max(dot(-local, -local).xx, float2(1e-6, 1.0));
					float dist = sqrt(dist2.y);
					float occlusion = saturate(dot(localNormal, -local) * rsqrt(dist2.x)) / dist2.y;
					occlusion = saturate((_AORange - dist) / boxFade) * occlusion;
					occlusion = dist < _AORange ? occlusion : 0.0;
					visibility *= 1.0 - occlusion;
				}
				return _AOStrength * (visibility - 1.0) + 1.0;
			}

			float3 TrainLampLighting(float3 positionWS, float3 normalWS)
			{
				float3 light = 0.0;
				if (_TrainLamps > 0.5)
				{
					for (int i = 0; i < 2; i++)
					{
						float3 toLamp = _TrainLampPos[i].xyz - positionWS;
						float2 dist2 = max(dot(toLamp, toLamp).xx, float2(1e-4, 0.5));
						float3 dir = toLamp * rsqrt(dist2.x);
						float range = max(1.0 - dist2.x / (_TrainLampPos[i].w * _TrainLampPos[i].w), 0.0);
						float cone = saturate((dot(-dir, _TrainLampDir[i].xyz) - _TrainLampDir[i].w) / ((1.0 - _TrainLampDir[i].w) * 0.5));
						cone = cone * cone * (3.0 - 2.0 * cone);
						float spot = -1.0 >= _TrainLampDir[i].w ? 1.0 : cone;
						float atten = spot * (range * range) / dist2.y;
						light += (atten * _TrainLampColor[i].xyz) * WrapDiffuse(normalWS, dir);
					}
				}
				return light;
			}

			float3 AddAdditionalLights(float3 light, float3 positionWS, float3 normalWS)
			{
				#if defined(_ADDITIONAL_LIGHTS)
					uint count = GetAdditionalLightsCount();
					for (uint i = 0u; i < count; ++i)
					{
						Light l = GetAdditionalLight(i, positionWS);
						light += (l.distanceAttenuation * l.color) * WrapDiffuse(normalWS, l.direction);
					}
				#endif
				return light;
			}

			float3 ApplyHeightFog(float3 color, float heightWS)
			{
				float fog = saturate((_HeightFog.x - heightWS) / max(_HeightFog.x - _HeightFog.y, 0.01));
				fog = fog * fog * (3.0 - 2.0 * fog) * _HeightFogColor.a;
				return lerp(color, _HeightFogColor.rgb, fog);
			}

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.positionCS = positionInputs.positionCS;
				output.positionWS = positionInputs.positionWS;
				output.normalWS = normalWS;

				float3 direction = normalize(_Direction.xyz + float3(0.0, 1e-6, 0.0));
				float along = _WorldSpace > 0.5 ? dot(positionInputs.positionWS, direction) : dot(input.positionOS.xyz, direction);
				output.gradient = _MeshGradient > 0.5 ? input.uv3.x : along;
				output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

				float3 n = normalize(normalWS);
				#if defined(_NO_AO)
					float ao = 1.0;
				#else
					float ao = DynamicAO(positionInputs.positionWS, n);
				#endif
				float staticAO = (_AOStrength * _StaticAO) * (input.color.a - 1.0) + 1.0;

				output.sh = staticAO * SampleSH(n);
				#if defined(_PIXEL_LIGHTS)
					output.vertexLight = staticAO.xxx;
				#else
					output.vertexLight = staticAO * AddAdditionalLights(TrainLampLighting(positionInputs.positionWS, n), positionInputs.positionWS, n);
				#endif
				output.ao = _AODebug > 0.5 ? ao * staticAO : ao;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				CameraClip(input.positionWS);
				if (_AODebug > 0.5)
					return half4(input.ao.xxx, 1.0);

				float3 n = normalize(input.normalWS);
				float3 albedo = lerp(_BottomColor.rgb, _TopColor.rgb, smoothstep(_Bottom, _Top, input.gradient));

				#if defined(_NO_SHADOWS)
					half shadow = 1.0;
				#else
					half shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				#endif
				Light mainLight = GetMainLight();
				float3 direct = WrapDiffuse(n, mainLight.direction) * (shadow * mainLight.color);

				float aoDirect = _AODirect * (input.ao - 1.0) + 1.0;
				float3 ambient = input.sh * _Ambient * input.ao;
				#if defined(_PIXEL_LIGHTS)
					float3 pointLights = AddAdditionalLights(TrainLampLighting(input.positionWS, n), input.positionWS, n) * input.vertexLight.x;
				#else
					float3 pointLights = input.vertexLight;
				#endif

				float3 lighting = direct * aoDirect + ambient;
				lighting = pointLights * aoDirect + lighting;
				float3 color = lighting * albedo;
				color = MixFog(color, input.fogFactor);
				color = ApplyHeightFog(color, input.positionWS.y);

				half alpha = 1.0;
				#if defined(_FADE_TOP)
					alpha = 1.0 - smoothstep(_FadeStart, _FadeEnd, input.positionWS.y);
				#endif
				return half4(color, alpha);
			}
			ENDHLSL
		}

		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Blend One Zero, One Zero
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

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif
				output.positionCS = ApplyShadowClamping(TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS)));
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				return 0;
			}
			ENDHLSL
		}

		Pass {
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			ColorMask R
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local _CAM_CLIP
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(output.positionWS);
				return output;
			}

			half frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				CameraClip(input.positionWS);
				return input.positionCS.z;
			}
			ENDHLSL
		}

		Pass {
			Name "DepthNormals"
			Tags { "LightMode" = "DepthNormals" }
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local _CAM_CLIP
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
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

				output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(output.positionWS);
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				CameraClip(input.positionWS);
				return half4(normalize(input.normalWS), 0.0);
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
	// Shipped with CustomEditor "Ragdoll.GradientGUI"; that editor class is not in this project.
	//CustomEditor "Ragdoll.GradientGUI"
}
