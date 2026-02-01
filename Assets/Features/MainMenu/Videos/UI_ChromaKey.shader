Shader "UI/ChromaKey"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0,1,0,1)
        _Threshold ("Threshold", Range(0,1)) = 0.4
        _Softness ("Softness", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;     // <-- UI color (includes alpha)
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;     // <-- pass UI color to fragment
            };

            sampler2D _MainTex;
            float4 _KeyColor;
            float _Threshold;
            float _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;         // <-- carry UI tint/alpha
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float diff = distance(col.rgb, _KeyColor.rgb);
                float keyAlpha = smoothstep(_Threshold, _Threshold + _Softness, diff);

                col.a *= keyAlpha;

                // Multiply by UI color (so RawImage Color + Alpha works!)
                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
