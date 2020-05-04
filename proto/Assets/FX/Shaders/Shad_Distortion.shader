Shader "Custom/Shad_Distortion"
{
    Properties
    {
        _MainCol("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _ScrTex("Scroll Texture", 2D) = "white" {}
        _ScrSpd("Scroll Speed", float) = 1.0
        _ScrPow("Scroll Power", float) = 1.0
    }
    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
        LOD 200
        blend SrcAlpha One
        zwrite off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _ScrTex;
        float4 _MainCol;
        float _ScrSpd;
        float _ScrPow;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ScrTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 Scr = tex2D (_ScrTex, float2(IN.uv_ScrTex.x, IN.uv_ScrTex.y - _Time.y * _ScrSpd));
            float MTex = tex2D(_MainTex, IN.uv_MainTex + Scr * _ScrPow).a;
            o.Emission = MTex * _MainCol.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
