Shader "Knuckleheads/UISky" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_ZenithColor ("Zenith Color", Color) = (0.239,0.703,0.878,1)
		_HorizonColor ("Horizon Color", Color) = (0.579,1,0.973,1)
		_GradientPower ("Gradient Power", Range(0.2, 6)) = 2
		_HorizonAt ("Horizon Height", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
		LOD 0

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			// The shipped shader keeps these as plain globals (no UnityPerMaterial block).
			half4 _ZenithColor;
			half4 _HorizonColor;
			float _GradientPower;
			float _HorizonAt;

			struct Attributes
			{
				float4 positionOS : POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.color = input.color;
				output.uv = input.uv;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Vertical gradient from the horizon line (in UV space) up to the zenith.
				float height = saturate((input.uv.y - _HorizonAt) / max(1.0 - _HorizonAt, 0.001));
				height = pow(height, 1.0 / max(_GradientPower, 1e-4));
				half3 sky = lerp(_HorizonColor.rgb, _ZenithColor.rgb, height);

				return half4(sky * input.color.rgb, input.color.a);
			}
			ENDHLSL
		}
	}
}
