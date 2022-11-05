Shader "Unlit/YUVtoRGB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;


            fixed3 YUVtoRGB(fixed3 c)
            {
                fixed3 rgb;
                rgb.r = c.x + c.z * 1.13983;
                rgb.g = c.x + dot(fixed2(-0.39465, -0.58060), c.yz);
                rgb.b = c.x + c.y * 2.03211;
                return rgb;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 yuv = tex2D(_MainTex, i.uv);
                fixed4 rgb = fixed4(YUVtoRGB(yuv.rgb), yuv.a);
                return rgb;
            }
            ENDCG
        }
    }
}