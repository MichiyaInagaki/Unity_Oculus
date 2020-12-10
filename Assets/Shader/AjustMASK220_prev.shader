Shader "Unlit/AjustMASK220_prev"
{
	Properties
	{
		[Toggle] _AutoTextureRatio("Auto TextureRatio", Float) = 1

		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}

		_BackColor("Back Color", Color) = (0, 0, 0, 0)

		[Header(Mask)]
		[Toggle] _Inverse("Inverse", Float) = 0
		[NoScaleOffset]_MaskTex_upper("MaskTexture upper", 2D) = "white" {}
		[NoScaleOffset]_MaskTex_lower("MaskTexture lower", 2D) = "white" {}
		_Offset_upper("Upper MASK Offset", Range(-1, 1)) = 0
		_Offset_lower("Lower MASK Offset", Range(-1, 1)) = 0
		_ScaleY_upper("Scale Y_upper", Range(0, 2)) = 1
		_ScaleY_lower("Scale Y_lower", Range(0, 2)) = 1
	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _AUTOTEXTURERATIO_ON
			#pragma shader_feature _INVERSE_ON

			#include "UnityCG.cginc"

			//モデルの持っているどの情報を使うか指定：Input
			struct appdata
			{
				float4 vertex : POSITION;	//頂点座標（ : POSITION が頂点座標だよ、という意味（セマンティクス（＝その変数が何に使われるか）と呼ばれる））
				float2 uv : TEXCOORD0;		//UV座標
			};

			//描画のために必要な情報を指定：Output（vertex から fragment へ）※appdataと変数名を合わせる必要はない
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _BackColor;

			sampler2D _MainTex;
			float4 _MainTex_ST;			//TilingとOffset情報
			float4 _MainTex_TexelSize;		//テクスチャサイズ

			sampler2D _MaskTex_upper;
			sampler2D _MaskTex_lower;
			float _Offset_upper;
			float _Offset_lower;
			float _ScaleY_upper;
			float _ScaleY_lower;

			float _Inverse;

			v2f vert(appdata v)
			{
				v2f o;

			#ifdef _AUTOTEXTURERATIO_ON
				float texRatio = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
				v.vertex.x *= texRatio;
			#endif

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//マスクのオフセット分UV座標をずらす
				float2 maskUV_upper = i.uv - float2(0, _Offset_upper);
				float2 maskUV_lower = i.uv - float2(0, _Offset_lower);

				//マスクのスケール調整（スケールはメインテクスチャがmaskUVに対して相対的に小さくなることで実現）
				maskUV_upper.y = (maskUV_upper.y - 0.5) / _ScaleY_upper + 0.5 - (0.5 - _ScaleY_upper / 2.0) / _ScaleY_upper;
				maskUV_lower.y = (maskUV_lower.y - 0.5) / _ScaleY_lower + 0.5 + (0.5 - _ScaleY_lower / 2.0) / _ScaleY_lower / 2.0;

				//マスク部分の決定（上述までで決定した頂点情報（UV情報）に対して，マスクのテクスチャを貼る）
				fixed4 mask_upper = tex2D(_MaskTex_upper, maskUV_upper);
				fixed4 mask_lower = tex2D(_MaskTex_lower, maskUV_lower);

				////upperの上オフセット以上は描画しない
				if (maskUV_upper.y >= 0.99) {
					discard;
				}
				////lowerの下オフセット以下は描画しない
				if (maskUV_lower.y <= 0.01) {
					discard;
				}

				//マスク処理
				float mask_upper_rgb = (mask_upper.r + mask_upper.g + mask_upper.b) / 3.0;
				float mask_lower_rgb = (mask_lower.r + mask_lower.g + mask_lower.b) / 3.0;

				////マスクが白かつ自分の下（上）オフセット以上（以下）は描画しない
				if (mask_upper_rgb == 1.0 && maskUV_upper.y >= 0.0) {
					discard;
				}
				if (mask_lower_rgb == 1.0 && maskUV_lower.y <= 1.0) {
					discard;
				}

				fixed4 col;
				col.a = _BackColor.a;

				return col;
				//return _BackColor;	//黒で塗りつぶし
			}
			ENDCG
		}
	}
}
