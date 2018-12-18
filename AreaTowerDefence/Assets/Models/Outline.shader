Shader "Custom/Outline" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 0)
		_OutlineWidth("Outline Width", float) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
		LOD 200

			Cull Front

			CGPROGRAM

#pragma surface surf Lambert vertex:vert

		float4 _OutlineColor;
		float _OutlineWidth;

		struct Input {
			float4 vertexColor : COLOR;
		};

		void vert(inout appdata_full v, out Input o) {
			float distance = -UnityObjectToViewPos(v.vertex).z;
			v.vertex.xyz += v.normal * distance * _OutlineWidth;
			o.vertexColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _OutlineColor.rgb;
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG

			Cull Back
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
