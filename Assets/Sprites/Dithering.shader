Shader "UI/DitheredCheckerboard"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (1, 1, 1, 1)
        _ColorB ("Color B", Color) = (0, 0, 0, 1)
        _DitherSpread ("Dither Spread", Range(0.01, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 screenPos: TEXCOORD1;
            };

            fixed4 _ColorA;
            fixed4 _ColorB;
            float _DitherSpread;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1. Calculate checker pattern value from scrolled UVs
                float u = i.uv.x * 6.2831853;
                float v = i.uv.y * 6.2831853;
                float wave = sin(u) * sin(v);
                float blend = clamp((wave + _DitherSpread) / (2.0 * _DitherSpread), 0.0, 1.0);

                // 2. Sample 4x4 Bayer matrix using screen pixel coordinates
                float2 pixelPos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;
                int2 bayerCoord = int2(fmod(pixelPos, 4.0));

                const float bayer[16] = {
                     0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                    12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
                     3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                    15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
                };
                float threshold = bayer[bayerCoord.y * 4 + bayerCoord.x];

                // 3. Threshold between colors
                return (blend >= threshold) ? _ColorA : _ColorB;
            }
            ENDCG
        }
    }
}