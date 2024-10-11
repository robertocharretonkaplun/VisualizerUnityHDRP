Shader "Custom/HDRPWindow"
{
    Properties
    {
        _StencilVal("Stencil Value", Int) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        
        Pass
        {
            Name "StencilPass"
            ZWrite Off
            ColorMask 0  // No queremos renderizar color, solo modificar el stencil buffer

            Stencil
            {
                Ref [_StencilVal]   // Valor de referencia del stencil
                Comp Always         // Siempre escribir en el stencil
                Pass Replace        // Reemplazar el valor del stencil con el de referencia
            }

            HLSLPROGRAM
            #pragma only_renderers d3d11  // Para HDRP
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            // Estructura de entrada (atributos)
            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            // Estructura de salida (varyings)
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            // Vertex Shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Utilizamos unity_ObjectToWorld para transformar de espacio de objeto a espacio mundial
                float4 worldPos = mul(unity_ObjectToWorld, float4(IN.positionOS, 1.0));
                // Luego multiplicamos por UNITY_MATRIX_VP para ir de espacio mundial a espacio de clip
                OUT.positionCS = mul(UNITY_MATRIX_VP, worldPos);
                return OUT;
            }

            // Fragment Shader
            float4 frag(Varyings IN) : SV_Target
            {
                return float4(0, 0, 0, 0);  // No dibujamos nada visible, solo modificamos el stencil buffer
            }

            ENDHLSL
        }
    }
}
