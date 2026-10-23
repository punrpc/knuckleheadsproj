Shader "Island/Belt" {
	Properties {
		_BaseMap ("Belt Texture", 2D) = "white" {}
		_Tint ("Tint", Color) = (1,1,1,1)
		_Repeat ("Repeats Across Mesh UV", Float) = 4
		[KeywordEnum(Mesh, Loop, Disc)] _Map ("Mapping", Float) = 0
		_Speed ("Speed (m/s)", Float) = 0
		[HideInInspector] _Scroll ("Scroll (m)", Float) = 0
		_Tile ("Tile Length (m)", Float) = 1.4
		_Width ("Loop Width (m)", Float) = 1.38
		_Inner ("Loop Inner Half Size (m)", Float) = 5.08
		_Bend ("Loop Corner Bend Radius (m)", Float) = 3.5
		_Center ("Loop Center", Vector) = (0,0,0,0)
		_RadiusIn ("Disc Inner Radius (m)", Float) = 1
		_RadiusOut ("Disc Outer Radius (m)", Float) = 2.9
		_Wrap ("Soft Light Wrap", Range(0, 1)) = 0.5
		_Ambient ("Ambient Boost", Range(0, 2)) = 1
	}
	SubShader {
		Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half4 _Tint;
			float _Repeat;
			float _Speed;
			float _Scroll;
			float _Tile;
			float _Width;
			float _Inner;
			float _Bend;
			float4 _Center;
			float _RadiusIn;
			float _RadiusOut;
			float _Wrap;
			float _Ambient;
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
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fog
			#pragma shader_feature_local _MAP_MESH _MAP_LOOP _MAP_DISC
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

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

			// Height fog: x = fog top height, y = fully fogged height.
			float4 _HeightFog;
			half4 _HeightFogColor;

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float fogFactor : TEXCOORD3;
				half3 sh : TEXCOORD4;
				float2 positionOSxy : TEXCOORD5;
				float ao : TEXCOORD6;
				half3 vertexLight : TEXCOORD7;
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

			// Rotates a loop-space point into the frame of one of the four belt sides:
			// x = outward from the centre, y = along the side (direction of travel).
			float2 RotateToSide(float2 p, int side)
			{
				if (side == 1)
					return float2(p.y, -p.x);
				if (side == 2)
					return -p;
				return side == 3 ? float2(-p.y, p.x) : p;
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
				output.uv = input.uv;
				output.positionOSxy = input.positionOS.xy;
				output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

				float3 n = normalize(normalWS);
				output.sh = SampleSH(n);
				output.vertexLight = AddAdditionalLights(TrainLampLighting(positionInputs.positionWS, n), positionInputs.positionWS, n);
				output.ao = DynamicAO(positionInputs.positionWS, n);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				if (_AODebug > 0.5)
					return half4(input.ao.xxx, 1.0);

				float2 uv;
				float2 uvDdx;
				float2 uvDdy;
				#if defined(_MAP_LOOP)
					// Rounded-rectangle conveyor loop in world XZ around _Center:
					// u = across the belt, v = distance travelled around the loop in tiles.
					float2 p = input.positionWS.xz - _Center.xz;
					float bend = max(_Width * 0.5, _Bend);
					float halfStraight = _Width * 0.5 + _Inner - bend;
					float straight = halfStraight + halfStraight;
					float sideLength = bend * 1.5707963 + straight;

					int side = p.x >= abs(p.y) ? 0 : (p.y >= abs(p.x) ? 1 : (-p.x >= abs(p.y) ? 2 : 3));
					float2 local = RotateToSide(p, side);
					if (local.y < -halfStraight)
					{
						side = side == 0 ? 3 : side - 1;
						local = RotateToSide(p, side);
					}

					float across = max(abs(p.y), abs(p.x)) - _Inner;
					float cornerAngle = clamp(atan2(local.y - halfStraight, local.x - halfStraight), 0.0, 1.5707963);
					float along = halfStraight >= local.y ? halfStraight + local.y : cornerAngle * bend + straight;

					float perimeter = sideLength * 4.0;
					float tiles = max(round(perimeter / _Tile), 1.0);
					uv.x = saturate(across / _Width);
					float travelled = (float)side * sideLength + along;
					travelled = travelled - _Speed * _Time.y - _Scroll;
					uv.y = tiles * travelled / perimeter;

					uvDdx = ddx(uv);
					uvDdy = ddy(uv);
					uvDdx.y -= tiles * round(uvDdx.y / tiles);
					uvDdy.y -= tiles * round(uvDdy.y / tiles);
				#elif defined(_MAP_DISC)
					// Turntable: u = radial between the inner and outer radius, v = around the disc in tiles.
					float2 p = input.positionOSxy;
					float radius = length(p);
					float tiles = max(round(PI * (_RadiusOut + _RadiusIn) / _Tile), 1.0);
					uv.x = saturate((radius - _RadiusIn) / max(_RadiusOut - _RadiusIn, 0.01));
					uv.y = tiles * atan2(p.y, p.x) * INV_TWO_PI;

					uvDdx = ddx(uv);
					uvDdy = ddy(uv);
					uvDdx.y -= tiles * round(uvDdx.y / tiles);
					uvDdy.y -= tiles * round(uvDdy.y / tiles);
				#else
					uv = input.uv * _Repeat;
					uvDdx = ddx(uv);
					uvDdy = ddy(uv);
				#endif
				float3 albedo = SAMPLE_TEXTURE2D_GRAD(_BaseMap, sampler_BaseMap, uv, uvDdx, uvDdy).rgb * _Tint.rgb;

				float3 n = normalize(input.normalWS);
				half shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				Light mainLight = GetMainLight();
				float3 direct = WrapDiffuse(n, mainLight.direction) * (shadow * mainLight.color);

				float aoDirect = _AODirect * (input.ao - 1.0) + 1.0;
				float3 ambient = input.sh * _Ambient * input.ao;
				float3 lighting = direct * aoDirect + ambient;
				lighting = input.vertexLight * aoDirect + lighting;
				float3 color = lighting * albedo;
				color = MixFog(color, input.fogFactor);
				color = ApplyHeightFog(color, input.positionWS.y);
				return half4(color, 1.0);
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
				return output;
			}

			half frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				return input.positionCS.z;
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
