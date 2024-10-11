Shader "Custom/HDRPWall"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MainTex("Base Map", 2D) = "white" {}
        _StencilVal("Stencil Value", Int) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        
        Pass
        {
            Name "Forward"
            ZWrite On
            Cull Back

            Stencil
            {
                Ref [_StencilVal]   // Valor que escribimos en el shader de la ventana
                Comp NotEqual       // Solo dibujar si el stencil NO es igual a este valor
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _MainTex_ST;

            // Estructura de entrada (atributos)
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Estructura de salida (varyings)
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Vertex Shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 worldPos = mul(GetObjectToWorldMatrix(), float4(IN.positionOS, 1.0));  // De objeto a espacio mundial
                OUT.positionCS = mul(UNITY_MATRIX_VP, worldPos);                             // De espacio mundial a espacio de clip
                OUT.uv = IN.uv;
                return OUT;
            }

            // Fragment Shader
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 baseColor = tex2D(_MainTex, uv) * _BaseColor;  // Combinar textura y color base
                return baseColor;
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
