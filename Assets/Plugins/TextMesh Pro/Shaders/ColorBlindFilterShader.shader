Shader "Unlit/ColorBlindFilterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Type ("Filter Type", Range(0, 3)) = 0 // 0: None, 1: Protanopia, 2: Deuteranopia, 3: Tritanopia
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Type;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 ApplyColorBlindFilter(float3 color, float filterType)
            {
                if (filterType == 1.0) // Protanopia
                {
                    return float3(
                        0.56667 * color.r + 0.43333 * color.g,
                        0.55833 * color.g + 0.44167 * color.b,
                        1.0 * color.b
                    );
                }
                else if (filterType == 2.0) // Deuteranopia
                {
                    return float3(
                        0.625 * color.r + 0.375 * color.g,
                        0.7 * color.g + 0.3 * color.b,
                        1.0 * color.b
                    );
                }
                else if (filterType == 3.0) // Tritanopia
                {
                    return float3(
                        0.95 * color.r + 0.05 * color.g,
                        0.43333 * color.g + 0.56667 * color.b,
                        0.475 * color.r + 0.525 * color.b
                    );
                }
                return color; // None (default)
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 originalColor = tex2D(_MainTex, i.uv).rgb;
                float3 filteredColor = ApplyColorBlindFilter(originalColor, _Type);
                return float4(filteredColor, 1.0);
            }
            ENDCG
        }
    }
}
