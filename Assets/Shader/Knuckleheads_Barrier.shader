Shader "Knuckleheads/Barrier" {
	Properties {
		_ColorA ("Light Stripe", Color) = (0.188,0.188,0.188,1)
		_ColorB ("Dark Stripe", Color) = (0.157,0.157,0.157,1)
		_Alpha ("Alpha", Range(0, 1)) = 0.8
		_Angle ("Angle", Range(5, 85)) = 30
		_Period ("Stripe Spacing (m)", Float) = 0.5
		_Fill ("Light Share", Range(0, 1)) = 0.58
		_Speed ("Drift (m per s)", Float) = 0.15
		_FrameColor ("Frame", Color) = (0.42,0.42,0.42,1)
		_Frame ("Frame Width (m)", Float) = 0.08
		_FoamDepth ("Touch Foam Width (m)", Float) = 0.18
		_FoamScale ("Foam Lumps per m", Float) = 3
		_FoamSpeed ("Foam Churn", Float) = 0.6
		_FoamReach ("Ripple Reach (m)", Float) = 0.3
		_FoamPulse ("Ripples per s", Float) = 0.45
	}
	SubShader {
		Tags { "IgnoreProjector"="true" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
		LOD 0

		Pass {
			Name "Barrier"
			// No LightMode tag in the shipped pass (renders as SRPDefaultUnlit).
			Tags { "IgnoreProjector"="true" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			BlendOp Add
			ZTest LEqual
			ZWrite Off
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _ColorA;
				half4 _ColorB;
				float _Alpha;
				float _Angle;
				float _Period;
				float _Fill;
				float _Speed;
				half4 _FrameColor;
				float _Frame;
				float _FoamDepth;
				float _FoamScale;
				float _FoamSpeed;
				float _FoamReach;
				float _FoamPulse;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS   : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS   : TEXCOORD1;
				float  fogFactor  : TEXCOORD2;
				float3 positionOS : TEXCOORD3;
				float  eyeDepth   : TEXCOORD4;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				output.positionWS = positionWS;
				output.eyeDepth = -TransformWorldToView(positionWS).z;
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.positionOS = input.positionOS.xyz;
				return output;
			}

			// iq-style hash / 3D value noise.
			float Hash(float3 p)
			{
				p = frac(p * 0.3183099 + 0.1);
				p *= 17.0;
				return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
			}

			float ValueNoise(float3 x)
			{
				float3 i = floor(x);
				float3 f = frac(x);
				f = f * f * (3.0 - 2.0 * f);
				return lerp(lerp(lerp(Hash(i + float3(0, 0, 0)), Hash(i + float3(1, 0, 0)), f.x),
				                 lerp(Hash(i + float3(0, 1, 0)), Hash(i + float3(1, 1, 0)), f.x), f.y),
				            lerp(lerp(Hash(i + float3(0, 0, 1)), Hash(i + float3(1, 0, 1)), f.x),
				                 lerp(Hash(i + float3(0, 1, 1)), Hash(i + float3(1, 1, 1)), f.x), f.y), f.z);
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Churning foam noise (two octaves).
				float churn = _Time.y * _FoamSpeed;
				float3 foamPos = input.positionWS * _FoamScale;
				float noiseA = ValueNoise(foamPos + float3(0.0, churn, churn * 0.6));
				float noiseB = ValueNoise(foamPos * 2.3 - float3(churn * 0.8, 0.0, churn * 0.4));
				float foamNoise = noiseA * 0.65 + noiseB * 0.35;
				float rippleWidthScale = foamNoise + 0.5;
				float frameWidthScale = foamNoise * 1.2 + 0.4;

				// Distance from the surface to whatever opaque geometry is behind it.
				float2 screenUV = GetNormalizedScreenSpaceUV(input.positionCS);
				float sceneEyeDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
				float3 viewDirWS = normalize(input.positionWS - _WorldSpaceCameraPos);
				float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
				float2 depthAndCos = max(float2(sceneEyeDepth - input.eyeDepth, dot(viewDirWS, cameraForward)), float2(0.0, 0.01));
				float contactDist = depthAndCos.x / depthAndCos.y;
				float3 normalWS = normalize(input.normalWS);
				contactDist = max(abs(dot(viewDirWS, normalWS)), 0.25) * contactDist;
				// Re-express in frame units so foam width maps onto the frame width.
				contactDist = contactDist * _Frame / max(_FoamDepth, 0.001);

				// Distance (m) to the panel's border: middle of the three per-axis face distances.
				float4x4 objectToWorld = GetObjectToWorldMatrix();
				float3 objectScale = float3(length(objectToWorld._m00_m10_m20), length(objectToWorld._m01_m11_m21), length(objectToWorld._m02_m12_m22));
				float3 faceDist = (0.5 - abs(input.positionOS)) * objectScale;
				float borderDist = faceDist.x + faceDist.y + faceDist.z
				                 - min(min(faceDist.z, faceDist.y), faceDist.x)
				                 - max(max(faceDist.z, faceDist.y), faceDist.x);
				float dist = min(contactDist, borderDist);
				float aa = max(fwidth(dist), 0.0001);

				// Expanding ripples.
				float pulse = frac(_Time.y * _FoamPulse);
				float rippleWidth = (1.0 - pulse) * (_Frame * 0.4) * rippleWidthScale;
				float ripplePos = pulse * _FoamReach + _Frame * 1.3;
				float ripple = 1.0 - smoothstep(rippleWidth - aa, rippleWidth + aa, abs(dist - ripplePos));

				float3 lumpPos = foamPos * 1.7 + float3(churn * 0.5, -churn, 0.0);
				float lumps = smoothstep(0.42, 0.5, ValueNoise(lumpPos)) * ripple;

				float frameWidth = _Frame * frameWidthScale;
				float frameBand = 1.0 - smoothstep(frameWidth - aa, frameWidth + aa, dist);
				float frameMask = max(lumps, frameBand);

				// Diagonal drifting hazard stripes.
				float angle = _Angle * 0.0174532924;
				float3 stripeDir = float3(sin(-angle), cos(angle), sin(angle));
				float period = max(_Period, 0.01);
				float stripeCoord = dot(input.positionWS, stripeDir) / period - (_Time.y * _Speed) / period;
				float stripeAA = max(fwidth(stripeCoord), 0.0001);
				float phase = frac(stripeCoord);
				float stripeDist = (_Fill >= phase) ? min(phase, _Fill - phase) : -min(phase - _Fill, 1.0 - phase);
				float stripe = saturate(stripeDist / stripeAA + 0.5);

				float3 color = lerp(_ColorB.rgb, _ColorA.rgb, stripe);
				color = lerp(color, _FrameColor.rgb, frameMask);
				color = MixFog(color, input.fogFactor);
				float alpha = lerp(_Alpha, 1.0, frameMask);
				return half4(color, alpha);
			}
			ENDHLSL
		}
	}
}
