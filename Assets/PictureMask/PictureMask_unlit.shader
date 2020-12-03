Shader "Gato/PictureMask_Unlit"
{
	Properties
	{
		[Toggle] _AutoTextureRatio ("Auto TextureRatio", Float) = 1

		[NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}

		_BackColor ("Back Color", Color) = (0, 0, 0, 0)

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
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Cull Back
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _AUTOTEXTURERATIO_ON
			#pragma shader_feature _INVERSE_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			sampler2D _MaskTex;
			float _OffsetX;
			float _OffsetY;
			float _Scale;
			float _ScaleX;
			float _ScaleY;
			//int _Rotation;

			float _Inverse;
			
			v2f vert (appdata v)
			{
				v2f o;

			#ifdef _AUTOTEXTURERATIO_ON
				float texRatio = _MainTex_TexelSize.y/_MainTex_TexelSize.x;
				v.vertex.x *= texRatio;
			#endif

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 maskUV = i.uv-float2(_OffsetX, _OffsetY);
				maskUV = (maskUV-0.5)/_Scale/float2(_ScaleX, _ScaleY)+0.5;

				fixed4 mask = tex2D(_MaskTex, maskUV);

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r+mask.g+mask.b)/3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) discard;
			#else
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))-1);
				clip(mask.rgb-0.5);
			#endif

				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}

		Cull Front
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _AUTOTEXTURERATIO_ON
			#pragma shader_feature _INVERSE_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			float4 _BackColor;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			sampler2D _MaskTex;
			float4 _MaskTex_TexelSize;
			float _OffsetX;
			float _OffsetY;
			float _Scale;
			float _ScaleX;
			float _ScaleY;
			
			v2f vert (appdata v)
			{
				v2f o;

			#ifdef _AUTOTEXTURERATIO_ON
				float texRatio = _MainTex_TexelSize.y/_MainTex_TexelSize.x;
				v.vertex.x *= texRatio;
			#endif

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
				float texRatio = _MainTex_TexelSize.y/_MainTex_TexelSize.x;
				float maskTexRatio = _MaskTex_TexelSize.y/_MaskTex_TexelSize.x;

				float2 maskUV = i.uv-float2(_OffsetX, _OffsetY);
				maskUV = (maskUV-0.5)/_Scale/float2(_ScaleX, _ScaleY)+0.5;

				fixed4 mask = tex2D(_MaskTex, maskUV);

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r+mask.g+mask.b)/3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) discard;
			#else
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))-1);
				clip(mask.rgb-0.5);
			#endif

				return _BackColor;
			}
			ENDCG
		}
	}
}
