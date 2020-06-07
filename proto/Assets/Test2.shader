Shader "Custom/Test2"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Ps //noambient

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };


        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        float4 LightingPs(SurfaceOutput s, float3 lightDir, float atten)
        {

        float NdotL = dot(s.Normal, lightDir) * 0.5 + 0.4;

        float4 finalColor;

        finalColor.rgb = s.Albedo.rgb * NdotL * atten * _LightColor0.rgb;
        finalColor.a = s.Alpha;

        return finalColor;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
