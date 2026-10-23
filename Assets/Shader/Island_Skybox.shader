Shader "Island/Skybox" {
	Properties {
		_ZenithColor ("Zenith Color", Color) = (0.29,0.56,0.86,1)
		_HorizonColor ("Horizon Color", Color) = (0.76,0.89,0.95,1)
		_GroundColor ("Below Horizon Color", Color) = (0.66,0.82,0.89,1)
		_GradientPower ("Gradient Power", Range(0.2, 6)) = 2.2
		_SunColor ("Sun Color", Color) = (1,1,0.96,1)
		_SunSize ("Sun Size", Range(0.999, 1)) = 0.9997
		_SunSoftness ("Sun Edge Softness", Range(0.00001, 0.005)) = 0.0004
		_HaloColor ("Halo Color", Color) = (1,1,0.94,1)
		_HaloPower ("Halo Falloff", Range(1, 64)) = 10
		_HaloStrength ("Halo Strength", Range(0, 2)) = 0.7
		_CloudMap ("Cloud Texture", 2D) = "black" {}
		_CloudLitColor ("Cloud Lit Color", Color) = (1,1,1,1)
		_CloudShadeColor ("Cloud Shade Color", Color) = (0.78,0.87,0.95,1)
		_CloudCoverage ("Coverage", Range(0, 1)) = 0.45
		_CloudSoftness ("Edge Softness", Range(0.01, 0.6)) = 0.28
		_CloudOpacity ("Opacity", Range(0, 1)) = 0.75
		_LayerAScale ("Layer A Scale", Float) = 0.12
		_LayerASpeed ("Layer A Speed", Vector) = (0.004,0.001,0,0)
		_LayerBScale ("Layer B Scale", Float) = 0.32
		_LayerBSpeed ("Layer B Speed", Vector) = (0.011,-0.003,0,0)
		_LayerBWeight ("Layer B Weight", Range(0, 1)) = 0.4
		_CloudHeight ("Cloud Plane Height", Float) = 1
		_DomeCurve ("Cloud Dome (0 flat plane, 1 full dome)", Range(0, 1)) = 0.5
		_CloudFade ("Horizon Fade", Range(0.001, 0.6)) = 0.12
		_CloudTopFade ("Zenith Fade", Range(0, 1)) = 0.25
	}
	SubShader {
		Tags { "PreviewType"="Skybox" "Queue"="Background" "RenderPipeline"="UniversalPipeline" "RenderType"="Background" }
		LOD 0

		Pass {
			Name "Skybox"
			Tags { "PreviewType"="Skybox" "Queue"="Background" "RenderPipeline"="UniversalPipeline" "RenderType"="Background" }
			Blend One Zero, One Zero
			BlendOp Add
			ZTest LEqual
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _CloudMap_ST;
				half4 _ZenithColor;
				half4 _HorizonColor;
				half4 _GroundColor;
				half4 _SunColor;
				half4 _HaloColor;
				half4 _CloudLitColor;
				half4 _CloudShadeColor;
				float4 _LayerASpeed;
				float4 _LayerBSpeed;
				float _GradientPower;
				float _SunSize;
				float _SunSoftness;
				float _HaloPower;
				float _HaloStrength;
				float _CloudCoverage;
				float _CloudSoftness;
				float _CloudOpacity;
				float _LayerAScale;
				float _LayerBScale;
				float _LayerBWeight;
				float _CloudHeight;
				float _DomeCurve;
				float _CloudFade;
				float _CloudTopFade;
			CBUFFER_END

			TEXTURE2D(_CloudMap);
			SAMPLER(sampler_CloudMap);

			struct Attributes
			{
				float4 positionOS : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 viewVectorWS : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float4 positionWS = mul(GetObjectToWorldMatrix(), input.positionOS);
				output.positionCS = mul(GetWorldToHClipMatrix(), positionWS);
				output.viewVectorWS = positionWS.xyz - _WorldSpaceCameraPos;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 dir = normalize(input.viewVectorWS);

				// Cloud layers projected onto a (partly domed) plane overhead.
				float planeDivisor = max(dir.y + _DomeCurve, 0.02);
				float2 planeUV = dir.xz / planeDivisor * _CloudHeight;
				float2 uvA = _LayerASpeed.xy * _Time.y + planeUV * _LayerAScale;
				float2 uvB = _LayerBSpeed.xy * _Time.y + planeUV * _LayerBScale;
				half layerA = SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, uvA).r;
				half layerB = SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, uvB).r;
				float cloud = lerp(layerA, layerA * layerB * 2.0, _LayerBWeight);

				float cloudEdge = cloud - (1.0 - _CloudCoverage);
				float cloudMask = saturate(cloudEdge / _CloudSoftness);
				cloudMask = cloudMask * cloudMask * (3.0 - 2.0 * cloudMask);
				float horizonFade = saturate(dir.y / _CloudFade);
				horizonFade = horizonFade * horizonFade * (3.0 - 2.0 * horizonFade);
				cloudMask *= horizonFade;
				cloudMask *= 1.0 - saturate(dir.y * _CloudTopFade);
				cloudMask *= _CloudOpacity;
				cloudMask *= (dir.y >= 0.001) ? 1.0 : 0.0;

				// Sky gradient.
				float gradient = pow(saturate(dir.y), 1.0 / max(_GradientPower, 0.0001));
				float3 sky = lerp(_HorizonColor.rgb, _ZenithColor.rgb, gradient);
				sky = lerp(sky, _GroundColor.rgb, saturate(dir.y * -8.0));

				// Sun halo.
				float3 sunDir = normalize(_MainLightPosition.xyz);
				float sunDot = dot(dir, sunDir);
				float halo = pow(saturate(sunDot), _HaloPower);
				sky += (halo * _HaloColor.rgb) * _HaloStrength;

				// Clouds.
				float cloudShade = saturate(cloudEdge / max(_CloudSoftness, 0.0001));
				float3 cloudColor = lerp(_CloudShadeColor.rgb, _CloudLitColor.rgb, cloudShade);
				float3 color = lerp(sky, cloudColor, cloudMask);

				// Sun disc.
				float sunEdge0 = _SunSize - _SunSoftness;
				float sunEdge1 = _SunSize + _SunSoftness;
				float sun = saturate((sunDot - sunEdge0) / (sunEdge1 - sunEdge0));
				sun = sun * sun * (3.0 - 2.0 * sun);
				color = lerp(color, _SunColor.rgb, sun);

				return half4(color, 1.0);
			}
			ENDHLSL
		}
	}
}
