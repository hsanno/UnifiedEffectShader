﻿Shader "Effect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)

        _IsVertexColorEnabled("Is Vertex Color Enabled", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma target 3.5

            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : Color;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : TEXCOORD1;

                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            fixed _IsVertexColorEnabled;

            v2f vert (appdata v)
            {
                v2f o;

                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = _IsVertexColorEnabled == 1.0 ? _Color * v.color : _Color;

                UNITY_TRANSFER_FOG(o, o.position);

                return o;
            }

            half4 frag (v2f i) : SV_Target0
            {
                half4 dst = tex2D(_MainTex, i.uv) * i.color;

                UNITY_APPLY_FOG(i.fogCoord, dst);

                return dst;
            }

            ENDHLSL
        }
    }

    Fallback Off
    CustomEditor "UnifiedEffect.EffectShader"
}
