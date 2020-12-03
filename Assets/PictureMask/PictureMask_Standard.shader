Shader "Gato/PictureMask_standard" {
	Properties {
		[Toggle] _AutoTextureRatio ("Auto TextureRatio", Float) = 1

		[NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}

		_BackColor ("Back Color", Color) = (0, 0, 0, 0)

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		[Header(Mask)] 
		[Toggle] _Inverse ("Inverse", Float) = 0
		[NoScaleOffset]_MaskTex ("MaskTexture", 2D) = "white" {}
		_OffsetX ("Offset X", Float) = 0
		_OffsetY ("Offset Y", Float) = 0
		_Scale ("Scale", Float) = 1
		_ScaleX ("Scale X", Range(0, 1)) = 1
		_ScaleY ("Scale Y", Range(0, 1)) = 1
		//[IntRange]_Rotation ("Ratation", Range(-180, 180)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Cull Back

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma shader_feature _AUTOTEXTURERATIO_ON
		#pragma shader_feature _INVERSE_ON

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;

		sampler2D _MaskTex;
		float _OffsetX;
		float _OffsetY;
		float _Scale;
		float _ScaleX;
		float _ScaleY;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v) {
			#ifdef _AUTOTEXTURERATIO_ON
				float texRatio = _MainTex_TexelSize.y/_MainTex_TexelSize.x;
				v.vertex.x *= texRatio;
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 maskUV = IN.uv_MainTex-float2(_OffsetX, _OffsetY);
			maskUV = (maskUV-0.5)/_Scale/float2(_ScaleX, _ScaleY)+0.5;

			fixed4 mask = tex2D(_MaskTex, maskUV);

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r+mask.g+mask.b)/3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) discard;
			#else
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))-1);
				clip(mask.rgb-0.5);
			#endif

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		Cull Front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma shader_feature _AUTOTEXTURERATIO_ON
		#pragma shader_feature _INVERSE_ON

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		struct Input {
			float2 uv_MainTex;
		};

		float4 _BackColor;

		half _Glossiness;
		half _Metallic;

		sampler2D _MaskTex;
		float _OffsetX;
		float _OffsetY;
		float _Scale;
		float _ScaleX;
		float _ScaleY;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v) {
			#ifdef _AUTOTEXTURERATIO_ON
				float texRatio = _MainTex_TexelSize.y/_MainTex_TexelSize.x;
				v.vertex.x *= texRatio;
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 maskUV = IN.uv_MainTex-float2(_OffsetX, _OffsetY);
			maskUV = (maskUV-0.5)/_Scale/float2(_ScaleX, _ScaleY)+0.5;

			fixed4 mask = tex2D(_MaskTex, maskUV);

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r+mask.g+mask.b)/3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) discard;
			#else
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))-1);
				clip(mask.rgb-0.5);
			#endif

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = _BackColor;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
