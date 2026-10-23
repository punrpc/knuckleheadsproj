Shader "Island/Poster" {
	Properties {
		_MainTex ("Poster", 2D) = "white" {}
		_Tint ("Tint", Color) = (1,1,1,1)
		_PaperColor ("Torn Paper", Color) = (0.96,0.94,0.87,1)
		_Tear ("Tear Depth", Range(0, 0.3)) = 0.07
		_Ragged ("Raggedness", Range(1, 40)) = 9
		_Fibers ("Fiber Detail", Range(0, 1)) = 0.45
		_Rim ("Paper Edge Width", Range(0, 0.06)) = 0.012
		_Curl ("Edge Shadow", Range(0, 1)) = 0.35
		_Corner ("Torn Corner", Range(0, 0.7)) = 0.25
		_Shreds ("Shreds", Range(0, 1)) = 0.15
		_Seed ("Seed", Float) = 0
		_Wrap ("Soft Light Wrap", Range(0, 1)) = 0.5
		_Ambient ("Ambient Boost", Range(0, 2)) = 1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "RenderPipeline" = "UniversalPipeline" "RenderType" = "TransparentCutout" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			half4 _Tint;
			half4 _PaperColor;
			float _Tear;
			float _Ragged;
			float _Fibers;
			float _Rim;
			float _Curl;
			float _Corner;
			float _Shreds;
			float _Seed;
			float _Wrap;
			float _Ambient;
		CBUFFER_END

		float PosterHash(float2 p)
		{
			p = frac(p * float2(123.34, 456.21));
			p += dot(p, p + 45.32);
			return frac(p.x * p.y);
		}

		float PosterNoise(float x, float seed)
		{
			float cell = floor(x);
			float f = frac(x);
			float u = f * f * (3.0 - 2.0 * f);
			return lerp(PosterHash(float2(cell, seed)), PosterHash(float2(cell + 1.0, seed)), u);
		}

		float PosterNoise2D(float2 p)
		{
			float2 cell = floor(p);
			float2 f = frac(p);
			float2 u = f * f * (3.0 - 2.0 * f);
			float bottom = lerp(PosterHash(cell), PosterHash(cell + float2(1.0, 0.0)), u.x);
			float top = lerp(PosterHash(cell + float2(0.0, 1.0)), PosterHash(cell + float2(1.0, 1.0)), u.x);
			return lerp(bottom, top, u.y);
		}

		// Ragged edge profile: a value-noise octave plus a finer "fiber" octave.
		float TearNoise(float x, float seed)
		{
			float coarse = PosterNoise(x, seed);
			float fine = PosterNoise(x * 3.7, seed + 0.53);
			return lerp(coarse, coarse * 0.7 + fine * 0.3, _Fibers);
		}

		struct PaperCoords
		{
			float2 uvA;      // UV scaled to the poster's aspect ratio (shorter side = 1)
			float offset;    // per-poster random offset
		};

		// Signed distance (in poster units) to the torn paper outline; negative = torn away.
		float TornPaperDistance(float2 uv, float3 normalOS, out PaperCoords coords)
		{
			float4x4 objectToWorld = GetObjectToWorldMatrix();
			float3 scale = float3(length(objectToWorld._m00_m10_m20), length(objectToWorld._m01_m11_m21), length(objectToWorld._m02_m12_m22));
			float3 absNormal = abs(normalOS);
			float2 size;
			if (absNormal.x < absNormal.y && absNormal.z < absNormal.y)
				size = scale.xz;
			else
				size = absNormal.z < absNormal.x ? scale.zy : scale.xy;
			float2 aspect = size / max(min(size.y, size.x), 1e-4);
			aspect = min(max(aspect, 0.05), 20.0);

			float3 objectPos = objectToWorld._m03_m13_m23;
			float seed = frac(_Seed * 0.1713 + PosterHash(objectPos.xz * 0.731 + objectPos.y * 0.377));
			float offset = seed * 61.0;

			float2 uvA = uv * aspect;
			float2 far = aspect - uvA;
			float2 t = (uvA.yx + offset) * _Ragged;
			float left = uvA.x - _Tear * TearNoise(t.x, 1.1);
			float right = far.x - _Tear * TearNoise(t.x, 2.2);
			float bottom = uvA.y - _Tear * TearNoise(t.y, 3.3);
			float top = far.y - _Tear * TearNoise(t.y, 4.4);
			float d = min(min(left, right), min(bottom, top));

			if (_Corner > 0.001)
			{
				float diagonal = (uvA.x + far.y) * 0.70710678 - _Corner;
				float alongCorner = ((uvA.x - far.y) * 0.70710678 + offset) * _Ragged;
				d = min(d, diagonal - _Tear * 1.6 * TearNoise(alongCorner, 5.5));
			}

			if (_Shreds > 0.001)
			{
				float strips = max(floor(aspect.x / 0.14 + 0.5), 3.0);
				float stripWidth = aspect.x / strips;
				float s = uvA.x / stripWidth;
				float strip = floor(s);
				float across = s - strip;
				float maxLength = aspect.y * _Shreds;
				float length0 = maxLength * PosterHash(float2(strip + 0.5, offset));
				float tilt = PosterHash(float2(strip * 3.3 + 0.5, offset + 7.7)) - 0.5;
				float cutDepth = (PosterHash(float2(strip * 1.7 + 0.5, offset + 3.1)) * 0.5 + 0.15) * maxLength + length0 * 0.75;

				float stripBottom = (uvA.y - length0 * 0.75) - tilt * 0.6 * (across - 0.5) * stripWidth;
				stripBottom -= _Tear * TearNoise(t.y, strip + 6.6);
				float stripSide = min(1.0 - across, across) * stripWidth - 0.004;
				stripSide -= TearNoise((uvA.y * 2.0 + offset) * _Ragged, strip + 7.7) * 0.004;
				float shred = min(max(uvA.y - cutDepth, stripSide), stripBottom);
				d = min(shred, d);
			}

			coords.uvA = uvA;
			coords.offset = offset;
			return d;
		}
		ENDHLSL

		Pass {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Cull [_Cull]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fog
			#pragma multi_compile _ _NO_AO
			#pragma multi_compile _ _NO_SHADOWS
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

			// Dynamic vertex AO from capsule and box occluders (set globally by the game).
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
				float ao : TEXCOORD4;
				half3 sh : TEXCOORD5;
				half3 vertexLight : TEXCOORD6;
				float3 normalOS : TEXCOORD7;
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
				output.uv = input.uv;
				output.normalOS = input.normalOS;
				output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

				float3 n = normalize(normalWS);
				#if defined(_NO_AO)
					output.ao = 1.0;
				#else
					output.ao = DynamicAO(positionInputs.positionWS, n);
				#endif
				output.sh = SampleSH(n);
				output.vertexLight = AddAdditionalLights(TrainLampLighting(positionInputs.positionWS, n), positionInputs.positionWS, n);
				return output;
			}

			half4 frag(Varyings input, FRONT_FACE_TYPE isFrontFace : FRONT_FACE_SEMANTIC) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				PaperCoords coords;
				float edge = TornPaperDistance(input.uv, input.normalOS, coords);
				clip(edge);

				half4 poster = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(input.uv, _MainTex));
				float printAlpha = poster.a * _Tint.a;
				float paperShade = PosterNoise2D(coords.uvA * 90.0 + coords.offset) * 0.1 + 0.9;
				float3 paper = paperShade * _PaperColor.rgb;
				float rim = 1.0 - smoothstep(_Rim * 0.6, _Rim, edge);
				float3 albedo = lerp(paper, poster.rgb * _Tint.rgb, printAlpha);
				albedo = lerp(albedo, _PaperColor.rgb * paperShade, rim);
				float curl = (1.0 - smoothstep(0.0, 0.04, edge - _Rim)) * _Curl;
				albedo *= 1.0 - curl * (1.0 - rim);

				float3 n = normalize(input.normalWS) * IS_FRONT_VFACE(isFrontFace, 1.0, -1.0);
				#if defined(_NO_SHADOWS)
					half shadow = 1.0;
				#else
					half shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
				#endif
				Light mainLight = GetMainLight();
				float3 direct = WrapDiffuse(n, mainLight.direction) * (shadow * mainLight.color);

				float aoDirect = _AODirect * (input.ao - 1.0) + 1.0;
				float3 ambient = input.sh * _Ambient * input.ao;
				float3 lighting = direct * aoDirect + ambient;
				lighting = input.vertexLight * aoDirect + lighting;
				float3 color = albedo * lighting;
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
			Cull [_Cull]

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
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalOS : TEXCOORD1;
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
				output.uv = input.uv;
				output.normalOS = input.normalOS;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				PaperCoords coords;
				clip(TornPaperDistance(input.uv, input.normalOS, coords));
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
			Cull [_Cull]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

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
				float2 uv : TEXCOORD0;
				float3 normalOS : TEXCOORD1;
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
				output.uv = input.uv;
				output.normalOS = input.normalOS;
				return output;
			}

			half frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				PaperCoords coords;
				clip(TornPaperDistance(input.uv, input.normalOS, coords));
				return input.positionCS.z;
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
