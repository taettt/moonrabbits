Shader "Custom/Shad_Distortion"
{
    Properties
    {
        [HDR]_MainCol("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _ScrTex("Scroll Texture", 2D) = "white" {}
        _ScrSpd1("Scroll Speed1", float) = 1.0
        _ScrSpd2("Scroll Speed2", float) = 0.0
        _ScrPow("Scroll Power", float) = 1.0
        
    }
    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
        blend SrcAlpha OneMinusSrcAlpha
        cull off //Two Side

        CGPROGRAM
        #pragma surface surf Lambert nofog noambient alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ScrTex;
        float4 _MainCol;
        float _ScrSpd1;
        float _ScrSpd2;
        float _ScrPow;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ScrTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 Scr = tex2D (_ScrTex, float2(IN.uv_ScrTex.x - _Time.y * _ScrSpd2, IN.uv_ScrTex.y - _Time.y * _ScrSpd1));
            float4 MTex = tex2D(_MainTex, IN.uv_MainTex + Scr * _ScrPow) * _MainCol;
            o.Emission = MTex.rgb;
            o.Alpha = MTex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
