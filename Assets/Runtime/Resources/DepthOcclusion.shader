//-------------------------------------------------------------------------------------------------------
// <copyright file="DepthOcclusion.shader" createdby="gblikas">
// 
// XR Remote
// Copyright(C) 2020  YOUAR, INC.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// https://www.gnu.org/licenses/agpl-3.0.html
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see
// <http://www.gnu.org/licenses/>.
//
// </copyright>
//-------------------------------------------------------------------------------------------------------

Shader "Custom/DepthOcclusion"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
        _MinDistance ("Min Distance", Float) = 0.0
        _MaxDistance ("Max Distance", Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "ForceNoShadowCasting" = "True"
        }

        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite Off
            Lighting Off
            LOD 100
            Tags
            {
                "LightMode" = "Always"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define real half
            #define real3 half3
            #define real4 half4
            #define TransformObjectToHClip UnityObjectToClipPos

            #define DECLARE_TEXTURE2D_FLOAT(texture) UNITY_DECLARE_TEX2D_FLOAT(texture)
            #define DECLARE_SAMPLER_FLOAT(sampler)
            #define SAMPLE_TEXTURE2D(texture,sampler,texcoord) UNITY_SAMPLE_TEX2D(texture,texcoord)

            struct appdata
            {
                float3 position : POSITION;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 clipPosition: TEXCOORD3;
                float2 texcoord : TEXCOORD0;
                float depth : TEXCOORD1;
                float3 worldPose: TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct fragment_output
            {
                real4 color : SV_Target;
            };

            CBUFFER_START(DisplayRotationPerFrame)
            float4x4 _DisplayRotationPerFrame;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPose = mul(UNITY_MATRIX_M, float4(v.position, 1.0)).xyz; 

                o.position = TransformObjectToHClip(v.position);
                o.texcoord = mul(float3(v.texcoord, 1.0f), _DisplayRotationPerFrame).xy;

                o.clipPosition = UnityObjectToClipPos(v.position);
                o.depth = length(o.worldPose - _WorldSpaceCameraPos.xyz);  //(o.clipPosition.z / o.clipPosition.w) * 0.5 + 0.5; 

                return o;
            }

            real3 HSVtoRGB(real3 arg1)
            {
                real4 K = real4(1.0h, 2.0h / 3.0h, 1.0h / 3.0h, 3.0h);
                real3 P = abs(frac(arg1.xxx + K.xyz) * 6.0h - K.www);
                return arg1.z * lerp(K.xxx, saturate(P - K.xxx), arg1.y);
            }

            DECLARE_TEXTURE2D_FLOAT(_MainTex);
            DECLARE_SAMPLER_FLOAT(sampler_MainTex);

            real _MinDistance;
            real _MaxDistance;

            fragment_output frag (v2f i)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                // Convert to NDC
                float2 ndc;
                ndc.x = i.clipPosition.x / i.clipPosition.w;
                ndc.y = i.clipPosition.y / i.clipPosition.w;

                // Convert to texture space
                float2 texCoord = (ndc.xy + 1.0) / 2.0;
                texCoord.y = 1.0 - texCoord.y;

                // Rotate the texture 90 degrees clockwise    
                float originalX = texCoord.x;
                texCoord.x = 1.0 - texCoord.y;
                texCoord.y = originalX;

                // Sample the depth texture
                float envDistance = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord).r;

                real lerpFactor = (envDistance - _MinDistance) / (_MaxDistance - _MinDistance);
                //real lerpFactor = envDistance;
                if(envDistance < i.depth){
                    discard;
                }

                real hue = lerp(0.70h, -0.15h, saturate(lerpFactor));
                if (hue < 0.0h)
                {
                    hue += 1.0h;
                }
                real3 color = real3(hue, 0.9h, 0.6h);

                fragment_output o;
                o.color = real4(HSVtoRGB(color), 1.0h);
                return o;
            }
            ENDHLSL
            
        }
    }
}
