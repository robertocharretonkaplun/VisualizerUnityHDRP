Shader "Custom/HDRP_CubeCutout"
{
    Properties
    {
        _CubeMin ("Cube Min Bounds", Vector) = (0,0,0)
        _CubeMax ("Cube Max Bounds", Vector) = (1,1,1)
        _MainTex ("Base Texture", 2D) = "white" {}
    }
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float3 _CubeMin;
            float3 _CubeMax;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float4 worldPosition = mul(GetObjectToWorldMatrix(), float4(input.positionOS, 1.0));
                output.positionCS = TransformWorldToHClip(worldPosition.xyz);
                output.uv = input.uv;
                output.worldPosition = worldPosition.xyz;
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                // Comprobar si el punto está dentro de los límites del cubo (área que queremos recortar)
                if (input.worldPosition.x >= _CubeMin.x && input.worldPosition.x <= _CubeMax.x &&
                    input.worldPosition.y >= _CubeMin.y && input.worldPosition.y <= _CubeMax.y &&
                    input.worldPosition.z >= _CubeMin.z && input.worldPosition.z <= _CubeMax.z)
                {
                    // Si el punto está dentro del cubo, descartamos el fragmento para hacer un agujero
                    discard;
                }

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            }
            ENDHLSL
        }
    }
    FallBack "HDRP/Lit"
}
