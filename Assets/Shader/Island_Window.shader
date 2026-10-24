Shader "Island/Window" {
	Properties {
		_BaseMap ("Window", 2D) = "white" {}
		_BaseColor ("Tint", Color) = (1,1,1,1)
		_EmissionMap ("Glow Mask", 2D) = "black" {}
		[HDR] _EmissionColor ("Glow", Color) = (2,1.6,0.84,1)
		_GlowThrough ("Glow Through Fog", Range(0, 1)) = 0.2
		_Cutoff ("Cutoff", Range(0, 1)) = 0.5
		_Wrap ("Soft Light Wrap", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "RenderPipeline" = "UniversalPipeline" "RenderType" = "TransparentCutout" }
		LOD 0

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST;
			half4 _BaseColor;
			float4 _EmissionMap_ST;
			half4 _EmissionColor;
			float _GlowThrough;
			float _Cutoff;
			float _Wrap;
		CBUFFER_END

		TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
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

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

			// Height fog (set globally by the game): x = fog top height, y = fully fogged height,
			// z = how much the fog colour blends toward the distance-fog colour far from the camera.
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float3 ApplyHeightFog(float3 color, float3 positionWS)
			{
				// Far from the camera the height fog takes on the distance-fog colour.
				float distanceFog = length(positionWS - _WorldSpaceCameraPos) * unity_FogParams.x;
				distanceFog = (1.0 - exp2(-distanceFog * distanceFog)) * _HeightFog.z;
				float3 fogColor = 0.0 < _HeightFog.z ? lerp(_HeightFogColor.rgb, unity_FogColor.rgb, distanceFog) : _HeightFogColor.rgb;

				float fog = saturate((_HeightFog.x - positionWS.y) / max(_HeightFog.x - _HeightFog.y, 0.01));
				fog = fog * fog * (3.0 - 2.0 * fog) * _HeightFogColor.a;
				return lerp(color, fogColor, fog);
			}

			float3 ApplyFog(float3 color, float fogFactor, float3 positionWS)
			{
				return ApplyHeightFog(MixFog(color, fogFactor), positionWS);
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
				output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
				output.sh = SampleSH(normalize(normalWS));
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				half4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(input.uv, _BaseMap));
				clip(baseSample.a * _BaseColor.a - _Cutoff);
				float3 albedo = baseSample.rgb * _BaseColor.rgb;

				// How much the fog hides what is behind it: fog white and black and compare (green channel).
				float fogTransmit = saturate(ApplyFog(1.0, input.fogFactor, input.positionWS).g - ApplyFog(0.0, input.fogFactor, input.positionWS).g);
				float glowHidden = 1.0 - fogTransmit;

				float3 n = normalize(input.normalWS);
				Light mainLight = GetMainLight();
				float wrapDiffuse = saturate((dot(n, mainLight.direction) + _Wrap) / (_Wrap + 1.0));
				float3 lighting = mainLight.color * wrapDiffuse + input.sh;

				float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, TRANSFORM_TEX(input.uv, _EmissionMap)).rgb * _EmissionColor.rgb;
				float3 color = albedo * lighting + emission;
				color = ApplyFog(color, input.fogFactor, input.positionWS);

				// Part of the glow still shows through the fog.
				color = (emission * _GlowThrough) * glowHidden + color;
				return half4(color, 1.0);
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

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
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
				return output;
			}

			half frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(input.uv, _BaseMap)).a;
				clip(alpha * _BaseColor.a - _Cutoff);
				return input.positionCS.z;
			}
			ENDHLSL
		}
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
