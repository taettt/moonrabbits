Shader "Custom/Toonshader01"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("NormalMap", 2D) = "Bump" {}
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        _EdgeWidth ("Edge width", Range (.002, 0.1)) = .005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Front
        //1st pass
        CGPROGRAM
        #pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				
				fixed4 _EdgeColor;
				fixed _EdgeWidth;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 Pos : POSITION;
				};

				v2f vert(a2v v) {
					v2f o;
					o.Pos = UnityObjectToClipPos(v.vertex);
					fixed3 norm = mul((float3x3)UNITY_MATRIX_MV, v.normal);
					norm.x *= UNITY_MATRIX_P[0][0];
					norm.y *= UNITY_MATRIX_P[1][1];
					o.Pos.xy += norm.xy * _EdgeWidth;
					return o;
				}

				half4 frag(v2f i) : COLOR {
					return _EdgeColor;
				}

            ENDCG


            




            cull back
            //2nd pass


            CGPROGRAM
            #pragma surface surf Toon 
            
            sampler2D _MainTex;
            sampler2D _BumpMap;




            struct Input 
            {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            };

            void surf (Input IN, inout SurfaceOutput o)
            {
                 float4 c = tex2D(_MainTex, IN.uv_MainTex);
                 o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
                 o.Albedo = c.rgb;
                 o.Alpha = c.a;
			}


            float4 LightingToon (SurfaceOutput s, float3 lightDir, float atten)
            {
                 float NdotL = (dot(s.Normal, lightDir)) * 0.5 + 0.5;

                 if (NdotL > 0.8)
                 {
                        NdotL = 1;
                        
                       
				 }
                 
                 else if (NdotL > 0.25)
                 {
                          NdotL = 0.6;        
				 }
                 else
                 {
                          NdotL = 0.2;        
				 }

                 float4 final;
                 final.rgb = s.Albedo * NdotL * _LightColor0.rgb;// * atten;
                 final.a = s.Alpha;

                 return final;

                 

        }
        ENDCG
    }
    FallBack "Diffuse"
}
