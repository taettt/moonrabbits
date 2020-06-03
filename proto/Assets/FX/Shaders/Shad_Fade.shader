Shader "Custom/Shad_Fade"
{
    Properties
    {
        [HDR]_MainCol("텍스쳐 컬러", Color) = (1,1,1,1)
        _MainTex ("메인 텍스쳐", 2D) = "white" {}
        _DisTex("투명하게 할 텍스쳐", 2D) = "white" {}
        _DisRange("강도", Range(0, 100)) = 100 //높을 수록 _DisTex가 더 커집니다. 10~1 사이를 사용하는걸 추천합니다.
        _AlphaRange("알파값 조절", Range(0, 1)) = 1 //알파값은 _MainCol에서 가져오셔도 되고, 이걸 사용하셔도 됩니다.

        _ScrTex("스크롤 텍스쳐", 2D) = "white" {}
        _ScrSpd1("스크롤 속도(세로)", float) = 1.0
        _ScrSpd2("스크롤 속도(가로)", float) = 0.0
        _ScrPow("구김정도", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        blend SrcAlpha OneMinusSrcAlpha
        //blend SrcAlpha One
        cull off

        CGPROGRAM
        #pragma surface surf Lambert nofog noambient alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DisTex;
        sampler2D _ScrTex;

        float4 _MainCol;

        float _DisRange;
        float _AlphaRange;
        float _ScrSpd1;
        float _ScrSpd2;
        float _ScrPow;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DisTex;
            float2 uv_ScrTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 Scr = tex2D(_ScrTex, float2(IN.uv_ScrTex.x - _Time.y * _ScrSpd2, IN.uv_ScrTex.y - _Time.y * _ScrSpd1));

            float MTex = tex2D (_MainTex, IN.uv_MainTex + Scr * _ScrPow).a;
            float DTex = tex2D (_DisTex, IN.uv_DisTex).a;
            float Dis = saturate(MTex - DTex * _DisRange);

            float4 TCol = Dis * _MainCol;

            o.Emission = TCol.rgb;
            o.Alpha = TCol.a * _AlphaRange;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
