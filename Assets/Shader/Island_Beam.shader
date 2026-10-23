Shader "Island/Beam" {
	Properties {
		[HDR] _Color ("Color (alpha = strength)", Color) = (1,0.9,0.75,0.2)
		_Falloff ("Length Falloff", Range(0.5, 6)) = 2
		_Edge ("Edge Softness", Range(0.5, 8)) = 2
	}
	SubShader {
		Tags { "IgnoreProjector"="true" "Queue"="Transparent+10" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
		LOD 0

		Pass {
			Name "Beam"
			Tags { "IgnoreProjector"="true" "LightMode"="UniversalForward" "Queue"="Transparent+10" "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" }
			Blend One One, One One
			BlendOp Add
			ZTest LEqual
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _Color;
				float _Falloff;
				float _Edge;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS   : NORMAL;
				float2 uv         : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS   : TEXCOORD1;
				float  alongBeam  : TEXCOORD2; // uv.y: 0 at the lamp, 1 at the tip
				float  fogFactor  : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionCS = TransformWorldToHClip(positionWS);
				output.positionWS = positionWS;
				output.alongBeam = input.uv.y;
				output.normalWS = TransformObjectToWorldNormal(input.normalOS);
				output.fogFactor = ComputeFogFactor(output.positionCS.z);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 viewDirWS = normalize(_WorldSpaceCameraPos - input.positionWS);
				float3 normalWS = normalize(input.normalWS);

				// Soft edges: fade as the surface turns side-on to the viewer.
				float facing = pow(abs(dot(normalWS, viewDirWS)), _Edge);
				float3 color = facing * (_Color.rgb * _Color.a);

				// Fade along the length of the beam.
				float lengthFade = pow(saturate(1.0 - input.alongBeam), _Falloff);
				color *= lengthFade;

				// Additive: fog fades the beam to black instead of towards the fog colour.
				color = MixFogColor(color, float3(0.0, 0.0, 0.0), input.fogFactor);

				return half4(color, 1.0);
			}
			ENDHLSL
		}
	}
}
