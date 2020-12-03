Shader "Custom/CurvedShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Radius("Radius", Range(1, 20)) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100
            Cull off

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert
            #pragma target 3.0

            sampler2D _MainTex;
            float _Radius;
            struct Input
            {
                float2 uv_MainTex;
            };

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                float pi = 3.1415926;
                float l = v.vertex.z * pi / 2;
                float r = _Radius;
                float th = l / r;
                v.vertex.xyz = float3(v.vertex.x * pi / 2, r * cos(th) - r, r * sin(th));
            }

            void surf(Input IN, inout SurfaceOutput o) {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}