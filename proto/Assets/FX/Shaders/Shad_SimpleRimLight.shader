Shader "Custom/Shad_SimpleRimLight" {

    Properties{
      _MainTex("Texture", 2D) = "white" {}
      _BumpMap("Bumpmap", 2D) = "bump" {}
      [HDR]_RimColor("Rim Color", Color) = (1,1,1,1)
      _RimPower("Rim Power", Range(0.1,8.0)) = 1.0
    }

        SubShader{
          Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
          blend SrcAlpha OneMinusSrcAlpha
          zwrite off

          CGPROGRAM

          #pragma surface surf Lambert nofog alpha:blend

          struct Input {
              float2 uv_MainTex;
              float2 uv_BumpMap;

              float3 viewDir;
          };

          sampler2D _MainTex;
          sampler2D _BumpMap;

          float4 _RimColor;
          float _RimPower;

          void surf(Input IN, inout SurfaceOutput o) {
              float4 MTex = tex2D(_MainTex, IN.uv_MainTex);
              o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

              half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

              o.Emission = _RimColor.rgb * pow(rim, _RimPower);
              o.Alpha = 0.5;
          }
          ENDCG
      }
          Fallback "Diffuse"
}