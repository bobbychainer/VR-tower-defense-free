Shader "Custom/TransparentShader" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            float4 _MainColor;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                float3 normalDir = normalize(i.normal);
                float3 upDir = float3(0, 1, 0); // Set the direction of the top face
                float3 downDir = float3(0, -1, 0); // Set the direction of the bottom face

                // Check if the normal direction is the same as the bottom face direction
                float isBottomFace = dot(normalDir, downDir) > 0.9 ? 1.0 : 0.0;

                // If it's the bottom face, use the main color, otherwise, make it fully transparent
                half4 color = isBottomFace * _MainColor + (1.0 - isBottomFace) * half4(0, 0, 0, 0);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
