Shader "Unlit/ScanLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineWidth ("Line Width", Range(0, 1)) = 0.5
        _LineSpacing ("Line Spacing", Range(1, 100)) = 10
        _Alpha ("Alpha", Range(0, 1)) = 0.15
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _LineWidth;
            float _LineSpacing;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lines = frac(i.uv.y * _LineSpacing);
                float scanline = step(lines, _LineWidth);
                return float4(0, 0, 0, scanline * _Alpha);
            }
            ENDCG
        }
    }
}