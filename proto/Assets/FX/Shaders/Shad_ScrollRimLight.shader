Shader "Custom/Shad_ScrollRimLight"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _BumpMap("Rim Bump", 2D) = "Bump" {}
        [HDR]_RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(0.0,10.0)) = 0.0
        _RimScrSpd("Rim Scroll Speed", float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
            zwrite off
            blend SrcAlpha One

            CGPROGRAM

            #pragma surface surf Lambert nofog

            sampler2D _MainTex;
            sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

            float4 _RimColor;
            float _RimPower;
            float _RimScrSpd;

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + (_Time.y * _RimScrSpd)));
            half Rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow(Rim, _RimPower);
            o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
