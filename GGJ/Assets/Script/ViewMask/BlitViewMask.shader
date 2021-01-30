Shader "Unlit/BlitShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Off
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            sampler2D _BackgroundRT;
            sampler2D _ForgroundMask;
            CBUFFER_START(UnityPerMaterial)
                float4 _ForgroundMask_ST;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ForgroundMask);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_ForgroundMask, i.uv);
                half4 bg = tex2D(_BackgroundRT, i.uv);
                return half4(bg.rgb * 0.3, 1.0 - col.r);
            }
            ENDHLSL
        }
    }
}
