Shader "Custom/shader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("노말맵", 2D) = "Bump" {}
        _Powerful ("노말맵강조", float) = 0.5
        _Smoothness ("스무스니스", float) = 0.5
        _Speed ("스피드", float) = 0.5


    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard 

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MainTex2;
        sampler2D _BumpMap;
        float _Powerful;
        float _Smoothness;
        float _Speed;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
            float2 uv_BumpMap;
        };

        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            float3 n = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap - _Time.x * _Speed));
            

            

            o.Normal = float3(n.r * _Powerful, n.g * _Powerful, n.b);








            fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2 - _Time.x * _Speed);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + d.r);
            



            



            o.Smoothness = _Smoothness;



            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
