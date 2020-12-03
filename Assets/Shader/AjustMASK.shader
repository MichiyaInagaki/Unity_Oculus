Shader "Unlit/AjustMASK"
{
	Properties
	{
		[Toggle] _AutoTextureRatio("Auto TextureRatio", Float) = 1

		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}

		_BackColor("Back Color", Color) = (0, 0, 0, 0)

		[Header(Mask)]
		[Toggle] _Inverse("Inverse", Float) = 0
		[NoScaleOffset]_MaskTex("MaskTexture", 2D) = "white" {}
		_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		_Scale("Scale", Float) = 1
		_ScaleX("Scale X", Range(0, 1)) = 1
		_ScaleY_upper("Scale Y_upper", Range(0, 1)) = 1
		_ScaleY_lower("Scale Y_lower", Range(0, 1)) = 1
		//[IntRange]_Rotation ("Ratation", Range(-180, 180)) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Cull Back	//裏側を描画しない＝表面のみ描画
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
				float4 vertex : POSITION;	//頂点座標（ : POSITION が頂点座標だよ、という意味（セマンティクスと呼ばれる））
				float2 uv : TEXCOORD0;		//UV座標
			};
			
			//描画のために必要な情報を指定：Output（vertex から fragment へ）※appdataと変数名を合わせる必要はない
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
			float _ScaleY_upper;
			float _ScaleY_lower;
			//int _Rotation;

			float _Inverse;

			//頂点シェーダー（頂点情報をいじる）：Input→Output
			////比率変えるかどうか
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

			//フラグメントシェーダー（頂点シェーダーで計算された情報をもとに該当ピクセルを塗りつぶす）
			////マスクとインバース処理
			fixed4 frag(v2f i) : SV_Target
			{
				float2 maskUV = i.uv - float2(_OffsetX, _OffsetY);					//マスクのオフセット分UV座標をずらす
				//maskUV = (maskUV - 0.5) / _Scale / float2(_ScaleX, _ScaleY) + 0.5;	//マスクのスケール調整

				//---編集点---
				//マスク上下独立制御
				if (maskUV.y >= 0.5f) {
					maskUV.y = (maskUV.y - 0.5) / _ScaleY_upper + 0.5;
				}
				else {
					maskUV.y = (maskUV.y - 0.5) / _ScaleY_lower + 0.5;
				}
				//-------------


				fixed4 mask = tex2D(_MaskTex, maskUV);								//マスク部分の決定

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r + mask.g + mask.b) / 3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) 
					discard;	//discard：ピクセルを破棄＝描画を行わない
			#else
				//clip関数では、与えられた引数が「0」より小さい場合は描画を行わない
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5)) - 1);		//マスク部分外は描画しない
				clip(mask.rgb - 0.5);	//マスクの黒色（に近いrgb値）の部分は描画しない
			#endif

				fixed4 col = tex2D(_MainTex, i.uv);		//i.uvにマスク処理，つまりそのピクセルを描画するかどうかが格納されている
				return col;		//マスク処理の情報が入った塗りつぶし情報を返す
			}
			ENDCG
		}

		
		Cull Front	//表面を描画しない＝裏側のみ描画（裏側を黒で塗りつぶし）
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
			float _ScaleY_upper;
			float _ScaleY_lower;

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

				float texRatio = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
				float maskTexRatio = _MaskTex_TexelSize.y / _MaskTex_TexelSize.x;

				float2 maskUV = i.uv - float2(_OffsetX, _OffsetY);
				//maskUV = (maskUV - 0.5) / _Scale / float2(_ScaleX, _ScaleY) + 0.5;

				//---編集点---
				if (maskUV.y >= 0.5f) {
					maskUV.y = (maskUV.y - 0.5) / _ScaleY_upper + 0.5;
				}
				else {
					maskUV.y = (maskUV.y - 0.5) / _ScaleY_lower + 0.5;
				}
				//------------

				fixed4 mask = tex2D(_MaskTex, maskUV);

			#ifdef _INVERSE_ON
				mask.rgb = abs(mask.rgb - 1);
				if (((mask.r + mask.g + mask.b) / 3.0 < 0.5) && (abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5))) discard;
			#else
				clip((abs(maskUV.x - 0.5) <= 0.5 && (abs(maskUV.y - 0.5) <= 0.5)) - 1);
				clip(mask.rgb - 0.5);
			#endif

				return _BackColor;	//黒で塗りつぶし
			}
			ENDCG
		}
	}
}
