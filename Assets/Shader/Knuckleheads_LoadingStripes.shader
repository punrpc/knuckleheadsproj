Shader "Knuckleheads/LoadingStripes" {
	Properties {
		_ColorA ("Light Stripe", Color) = (0.188,0.188,0.188,1)
		_ColorB ("Dark Stripe", Color) = (0.157,0.157,0.157,1)
		_Angle ("Angle", Range(5, 85)) = 30
		_Count ("Stripes Around", Float) = 36
		_Fill ("Light Share", Range(0, 1)) = 0.58
		_Speed ("Drift (deg per s)", Float) = 3
		_Alpha ("Alpha", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Overlay+2" "IgnoreProjector" = "True" }
		LOD 0

		Pass {
			Name "Stripes"
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
				half4 _ColorA;
				half4 _ColorB;
				float _Angle;
				float _Count;
				float _Fill;
				float _Speed;
				float _Clock;   // not a material property: drift clock, set from script
				float _Alpha;
			CBUFFER_END

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

				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
				output.positionWS = positionWS;
				output.positionCS = TransformWorldToHClip(positionWS);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Helical stripes wrapped around the viewer, parameterised by view direction.
				float3 viewDir = normalize(input.positionWS - _WorldSpaceCameraPos);

				// Two azimuths whose atan2 seams sit on opposite sides; the anti-aliasing width
				// takes the smaller derivative so neither seam produces a fwidth spike.
				float azimuth        = atan2(viewDir.x, viewDir.z);
				float azimuthFlipped = atan2(-viewDir.x, -viewDir.z);
				float drift = fmod(_Speed * _Clock, 360.0);
				float elevation = asin(clamp(viewDir.y, -1.0, 1.0));

				float sinAngle, cosAngle;
				sincos(radians(_Angle), sinAngle, cosAngle);
				float elevationTerm = degrees(elevation * cosAngle);
				float stripeScale = _Count / (sinAngle * 360.0);

				float bandsFlipped = (elevationTerm - (degrees(azimuthFlipped) + drift + 180.0) * sinAngle) * stripeScale;
				float bands        = (elevationTerm - (degrees(azimuth) + drift) * sinAngle) * stripeScale;
				float aaWidth = max(min(fwidth(bandsFlipped), fwidth(bands)), 1e-4);

				// Signed distance to the nearest light/dark stripe edge (positive inside the light share).
				float phase = frac(bands);
				float edgeDistance = (_Fill >= phase)
					? min(phase, _Fill - phase)
					: -min(phase - _Fill, 1.0 - phase);
				float lightMask = saturate(edgeDistance / aaWidth + 0.5);
				half3 color = lerp(_ColorB.rgb, _ColorA.rgb, lightMask);

				// Fade to the average colour near the poles where the stripes converge.
				float poleFade = smoothstep(0.82, 0.97, abs(viewDir.y));
				color = lerp(color, (_ColorA.rgb + _ColorB.rgb) * 0.5, poleFade);

				return half4(color, _Alpha);
			}
			ENDHLSL
		}
	}
}
