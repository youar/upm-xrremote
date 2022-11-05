Shader "Unlit/yuv420"
{
    Properties
    {
        _MainTex("Texture", 2D) = "gray" {}
        _CRCB("CRCB",2D) ="gray" {}
        _Y("Y",2D) ="gray" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CRCB;
            sampler2D _Y;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            static const float3x3 yuvCoef = {
                1.164f,  1.164f, 1.164f,
                0.000f, -0.392f, 2.017f,
                1.596f, -0.813f, 0.000f};
            fixed4 frag (v2f i) : SV_Target
            {
                i.uv=frac(float2(-i.uv.y,-i.uv.x)); 
                //if(fmod(_Time.y,2)>1){
                //i.uv.y*=2;
                //i.uv+=1;
                //if(i.uv.y>0){
                //    return tex2D(_Y,frac(i.uv));
                //}else{
                //    return tex2D(_CRCB,frac(i.uv));
                //}}

                float3 yuv = float3(
                tex2D(_Y,i.uv).x,
                tex2D(_CRCB,i.uv).xy);
                yuv -= float3(0.0625f, 0.5f, 0.5f);
                yuv = mul(yuv, yuvCoef); 
                yuv = saturate(yuv);
                return float4(yuv, 1.0f);
            }
            ENDCG
        }
    }
}
