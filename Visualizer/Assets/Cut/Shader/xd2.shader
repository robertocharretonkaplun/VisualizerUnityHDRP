Shader "Custom/HDRP_ObjShader"
{
    Properties
    {
        _ColorExt("External color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorInt("Internal color", Color) = (1.0, 1.0, 1.0, 1.0)
        _EdgeWidth("Edge width", Range(0.99, 0.5)) = 0.9

        [HideInInspector] _PlaneCenter("Plane Center", Vector) = (0, 0, 0, 1)
        [HideInInspector] _PlaneNormal("Plane Normal", Vector) = (0, 0, -1, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" "Queue" = "Geometry" }

        Pass
        {
            Name "ForwardLit"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            float4 _ColorExt;

            float4 frag(Varyings IN) : SV_Target
            {
                clip(-Distance2Plane(IN.worldPos.xyz));
                return _ColorExt;
            }

            float Distance2Plane(float3 pt)
            {
                float3 n = _PlaneNormal.xyz;
                float3 pt2 = _PlaneCenter.xyz;
                float d = (n.x * (pt.x - pt2.x)) + (n.y * (pt.y - pt2.y)) + (n.z * (pt.z - pt2.z)) / sqrt(n.x * n.x + n.y * n.y + n.z * n.z);
                return d;
            }

            ENDHLSL
        }

        Pass
        {
            Name "Internal"
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            float4 _ColorInt;

            float4 frag(Varyings IN) : SV_Target
            {
                clip(-Distance2Plane(IN.worldPos.xyz));
                return _ColorInt;
            }

            ENDHLSL
        }

        Pass
        {
            Name "Edge"
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            float4 _ColorInt;
            float _EdgeWidth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                IN.positionOS *= _EdgeWidth;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                clip(-Distance2Plane(IN.worldPos.xyz));
                return _ColorInt;
            }

            ENDHLSL
        }

        Pass
        {
            Name "EdgeGeometry"
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            struct GeomVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD0;
                float3 bary : TEXCOORD1;
            };

            float4 _ColorInt;
            float4 _ColorExt;
            float _EdgeWidth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                IN.positionOS *= _EdgeWidth;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS);
                return OUT;
            }

            [maxvertexcount(3)]
            void geom(triangle Varyings IN[3], inout TriangleStream<GeomVaryings> triStream)
            {
                GeomVaryings OUT;
                float3 param = float3(0., 0., 0.);

                float edge1 = length(IN[0].worldPos - IN[1].worldPos);
                float edge2 = length(IN[1].worldPos - IN[2].worldPos);
                float edge3 = length(IN[2].worldPos - IN[0].worldPos);

                if (edge1 > edge2 && edge1 > edge3)
                    param.y = 1.;
                else if (edge2 > edge3 && edge2 > edge1)
                    param.x = 1.;
                else
                    param.z = 1.;

                OUT.positionCS = mul(UNITY_MATRIX_VP, IN[0].worldPos);
                OUT.bary = float3(1., 0., 0.) + param;
                OUT.worldPos = IN[0].worldPos;
                triStream.Append(OUT);

                OUT.positionCS = mul(UNITY_MATRIX_VP, IN[1].worldPos);
                OUT.bary = float3(0., 0., 1.) + param;
                OUT.worldPos = IN[1].worldPos;
                triStream.Append(OUT);

                OUT.positionCS = mul(UNITY_MATRIX_VP, IN[2].worldPos);
                OUT.bary = float3(0., 1., 0.) + param;
                OUT.worldPos = IN[2].worldPos;
                triStream.Append(OUT);
            }

            float4 frag(GeomVaryings IN) : SV_Target
            {
                clip(-Distance2Plane(IN.worldPos.xyz));

                float minBary = min(min(IN.bary.x, IN.bary.y), IN.bary.z);
                float maxBary = max(max(IN.bary.x, IN.bary.y), IN.bary.z);

                if (minBary >= (1.0 - _EdgeWidth))
                    return _ColorExt;

                return _ColorInt;
            }

            ENDHLSL
        }
    }

    Fallback Off
}
