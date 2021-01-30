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
                return half4(0, 0, 0, 1.0 - col.r);
            }
            ENDHLSL
        }
    }
}
