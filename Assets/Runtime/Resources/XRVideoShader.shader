﻿//-------------------------------------------------------------------------------------------------------
// <copyright file="XRVideoShader.shader" createdby="gblikas">
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
Shader "Unlit/XRVideoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EnvironmentDepth("Texture", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Ztest Always
        ZWrite On

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
            sampler2D _EnvironmentDepth;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            fixed4 frag (v2f i, out float depth: SV_Depth) : SV_Target
            {
                // Rotate the texture 90 degrees clockwise    
                float originalX = i.uv.x;
                float originalY = i.uv.y;
                i.uv.x = 1.0 - i.uv.y;
                i.uv.y = originalX;

                // sample the texture
                depth = 1 - tex2D(_EnvironmentDepth, i.uv).r;
                
                
                return tex2D(_MainTex, float2(originalX, originalY));

            }


            ENDCG
        }
    }
}


