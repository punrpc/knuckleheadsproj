Shader "Island/Fog Volume" {
	Properties {
		_NearColor ("Near Colour (mist at the open end)", Color) = (0.1,0.11,0.13,1)
		_FarColor ("Far Colour (the back)", Color) = (0.01,0.012,0.015,1)
		_Density ("Density At The Back (per m)", Float) = 1
		_Ground ("Extra Density Near The Floor", Range(0, 4)) = 1.5
		_NoiseMap ("Noise", 2D) = "grey" {}
		_NoiseScale ("Noise Scale (per m)", Float) = 0.35
		_NoiseSpeed ("Drift Speed", Float) = 0.15
		_NoiseAmount ("Noise Amount", Range(0, 1)) = 0.6
		_LampGlow ("Train Lamp Glow", Range(0, 2)) = 0.5
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
		LOD 0

		Pass {
			Name "FogVolume"
			Tags { "LightMode"="UniversalForward" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			BlendOp Add
			ZTest Always
			ZWrite Off
			Cull Front

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _NoiseMap_ST;
				half4 _NearColor;
				half4 _FarColor;
				float _Density;
				float _Ground;
				float _NoiseScale;
				float _NoiseSpeed;
				float _NoiseAmount;
				float _LampGlow;
			CBUFFER_END

			TEXTURE2D(_NoiseMap);
			SAMPLER(sampler_NoiseMap);

			// Train headlamps, set globally from script. xyz = position, w = range.
			float4 _TrainLampPos[2];
			// xyz = forward, w = cos(cone half angle); <= -1 means omni.
			float4 _TrainLampDir[2];
			float4 _TrainLampColor[2];
			float _TrainLamps;

			struct Attributes
			{
				float4 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			float LampAttenuation(float3 positionWS, int i)
			{
				float3 toLamp = _TrainLampPos[i].xyz - positionWS;
				float distSq = dot(toLamp, toLamp);
				float2 distSqClamped = max(distSq.xx, float2(0.0001, 0.5));
				float3 lightDir = toLamp * rsqrt(distSqClamped.x);

				float range = max(1.0 - distSqClamped.x / (_TrainLampPos[i].w * _TrainLampPos[i].w), 0.0);

				float cone = saturate((dot(-lightDir, _TrainLampDir[i].xyz) - _TrainLampDir[i].w) / ((1.0 - _TrainLampDir[i].w) * 0.5));
				cone = cone * cone * (3.0 - 2.0 * cone);
				float spot = (_TrainLampDir[i].w <= -1.0) ? 1.0 : cone;

				return spot * (range * range) / distSqClamped.y;
			}

			float3 TrainLampLight(float3 positionWS)
			{
				float3 light = 0.0;
				if (_TrainLamps > 0.5)
				{
					float atten0 = LampAttenuation(positionWS, 0);
					float atten1 = LampAttenuation(positionWS, 1);
					light = (_TrainLampColor[0].xyz * atten0 + _TrainLampColor[1].xyz * atten1) * _LampGlow;
				}
				return light;
			}

			// One ray-march step through the unit box. Returns the step's opacity; colour in 'stepColor'.
			float FogSample(float t, float stepLength, float3 rayDirWS, float3 rayDirOS, float3 cameraOS, float2 drift, out float3 stepColor)
			{
				float2 positionOS_yz = rayDirOS.yz * t + cameraOS.yz;
				float3 positionWS = rayDirWS * t + _WorldSpaceCameraPos;

				// z runs from the open end (-0.5) to the back (+0.5); y from floor (-0.5) to ceiling (+0.5).
				float depth01 = saturate(positionOS_yz.y + 0.5);
				float floor01 = saturate(0.5 - positionOS_yz.x);

				float2 noiseUV = float2(positionWS.x * 0.5 + positionWS.z, positionWS.y) * _NoiseScale + drift;
				half noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, noiseUV).r;

				float density = (depth01 * depth01) * _Density;
				density *= (floor01 * floor01) * _Ground + 1.0;
				density *= (noise * 2.0 - 1.0) * _NoiseAmount + 1.0;

				stepColor = lerp(_NearColor.rgb, _FarColor.rgb, depth01) + TrainLampLight(positionWS);
				return 1.0 - exp(-(stepLength * density));
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 rayDirWS = normalize(input.positionWS - _WorldSpaceCameraPos);

				// Intersect the view ray with the object's unit cube.
				float3 cameraOS = TransformWorldToObject(_WorldSpaceCameraPos);
				float3 rayDirOS = TransformWorldToObjectDir(rayDirWS, false);
				float3 invDir = 1.0 / rayDirOS;
				float3 tA = (-0.5 - cameraOS) * invDir;
				float3 tB = (0.5 - cameraOS) * invDir;
				float3 tMin = min(tB, tA);
				float3 tMax = max(tB, tA);
				float tEnter = max(max(max(tMin.y, tMin.x), tMin.z), 0.0);
				float tExit = min(min(tMax.y, tMax.x), tMax.z);

				// Stop at opaque geometry.
				float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
				float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
				float cosToForward = max(dot(rayDirWS, cameraForward), 0.0001);
				float sceneDistance = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams) / cosToForward;
				tExit = min(sceneDistance, tExit);

				if (tEnter >= tExit)
					return half4(0.0, 0.0, 0.0, 0.0);

				float span = tExit - tEnter;
				float stepLength = span * 0.25;
				float driftX = _Time.y * _NoiseSpeed;
				float2 drift = float2(driftX, driftX * 0.37);

				// Four samples, composited front to back.
				float3 accumColor = 0.0;
				float transmittance = 1.0;
				float3 stepColor;
				float stepAlpha;

				stepAlpha = FogSample(tEnter + span * 0.125, stepLength, rayDirWS, rayDirOS, cameraOS, drift, stepColor);
				accumColor += (transmittance * stepAlpha) * stepColor;
				transmittance *= 1.0 - stepAlpha;

				stepAlpha = FogSample(tEnter + span * 0.375, stepLength, rayDirWS, rayDirOS, cameraOS, drift, stepColor);
				accumColor += (transmittance * stepAlpha) * stepColor;
				transmittance *= 1.0 - stepAlpha;

				stepAlpha = FogSample(tEnter + span * 0.625, stepLength, rayDirWS, rayDirOS, cameraOS, drift, stepColor);
				accumColor += (transmittance * stepAlpha) * stepColor;
				transmittance *= 1.0 - stepAlpha;

				stepAlpha = FogSample(tEnter + span * 0.875, stepLength, rayDirWS, rayDirOS, cameraOS, drift, stepColor);
				accumColor += (transmittance * stepAlpha) * stepColor;

				float alpha = 1.0 - transmittance * (1.0 - stepAlpha);
				return half4(accumColor / max(alpha, 0.0001), alpha);
			}
			ENDHLSL
		}
	}
}
