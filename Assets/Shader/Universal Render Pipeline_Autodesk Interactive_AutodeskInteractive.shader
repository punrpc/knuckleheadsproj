Shader "Universal Render Pipeline/Autodesk Interactive/AutodeskInteractive" {
	Properties {
		[ToggleUI] _UseColorMap ("UseColorMap", Float) = 0
		_Color ("BaseColor", Vector) = (0,0,0,0)
		[NoScaleOffset] _MainTex ("ColorMap", 2D) = "white" {}
		[ToggleUI] _UseNormalMap ("UseNormalMap", Float) = 0
		[NoScaleOffset] _BumpMap ("NormalMap", 2D) = "white" {}
		[ToggleUI] _UseMetallicMap ("UseMetallicMap", Float) = 0
		_Metallic ("Metallic", Float) = 0
		[NoScaleOffset] _MetallicGlossMap ("MetallicMap", 2D) = "white" {}
		[ToggleUI] _UseRoughnessMap ("UseRoughnessMap", Float) = 0
		_Glossiness ("Roughness", Float) = 0
		[NoScaleOffset] _SpecGlossMap ("RoughnessMap", 2D) = "white" {}
		[ToggleUI] _UseEmissiveMap ("UseEmissiveMap", Float) = 0
		[HDR] _EmissionColor ("Emissive", Vector) = (0,0,0,0)
		[NoScaleOffset] _EmissionMap ("EmissiveMap", 2D) = "white" {}
		[ToggleUI] _UseAoMap ("UseAoMap", Float) = 0
		[NoScaleOffset] _OcclusionMap ("AoMap", 2D) = "white" {}
		_UvOffset ("UVOffset", Vector) = (0,0,0,0)
		_UvTiling ("UVScale", Vector) = (1,1,0,0)
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] _XRMotionVectorsPass ("_XRMotionVectorsPass", Float) = 1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}