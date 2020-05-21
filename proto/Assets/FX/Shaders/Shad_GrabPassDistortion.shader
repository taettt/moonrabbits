Shader "Custom/Shad_GrabPassDistortion"
{
    Properties
    {
        _MainBump ("Distortion Texture", 2D) = "white" {}
        _DistorPow("Distortion Power", float) = 0.1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
        LOD 200

        GrabPass{}

        CGPROGRAM
        #pragma surface surf Lambert nofog
        #pragma target 3.0

        sampler2D _GrabTexture;
        sampler2D _MainBump;
        float _DistorPow;

        struct Input
        {
            float2 uv_MainBump;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 bump = tex2D(_MainBump, IN.uv_MainBump + _Time.x);
            float2 screenUV = IN.screenPos.rgb / IN.screenPos.a;
            screenUV = float2(screenUV.r, screenUV.g); //노말라이즈, 화면 확대나 축소시 크기가 변하는 걸 막아줌
            o.Emission = tex2D(_GrabTexture, screenUV + bump.r * _DistorPow);
            o.Alpha = bump.a;
        }
        ENDCG
    }
    FallBack "Regacy shaders/Transparent/Diffuse"
}
