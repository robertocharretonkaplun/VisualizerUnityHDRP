Shader "Custom/Table"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry+4" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;
            float _Glossiness;
            float _Metallic;

            float4 Frag(Varyings IN) : SV_Target
            {
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                return float4(albedo.rgb, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "HDRP/Unlit"
}