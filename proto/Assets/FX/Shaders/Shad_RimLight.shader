Shader "Custom/Shad_RimLight"
{
    Properties
    {
        _MainCol("Core Color", Color) = (0,0,0,0)
        _BumpMap("Rim Bump", 2D) = "Bump" {}
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(0.0,10.0)) = 5
        _RimScrSpd("Rim Scroll Speed", float) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
            LOD 200
            blend SrcAlpha One

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows

            #pragma target 3.0

            float4 _MainCol;
            sampler2D _BumpMap;
            float4 _RimColor;
            float _RimPower;
            float _RimScrSpd;

        struct Input
        {
            float2 uv_BumpMap;
            float3 viewDir;
        };


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + _Time.y * _RimScrSpd));
            half Rim = 1.0 - saturate(dot(IN.viewDir, o.Normal));

            o.Emission = _RimColor.rgb * pow(Rim, _RimPower);
            o.Albedo = _MainCol.rgb;
            o.Alpha = _MainCol.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
