Shader "Custom/BrushTip"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0, 1)) = 1.0
        _Softness ("Softness", Range(0, 1)) = 0.5
        _Aspect ("Aspect", Range(0.1, 2.0)) = 1.0
        _Angle ("Angle", Range(0, 360)) = 0.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float3 _Color;
            float _Opacity;
            float _Softness;
            float _Aspect;
            float _Angle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 rotateUV(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                float2 centered = uv - 0.5;
                float2 rotated = float2(centered.x * c - centered.y * s, centered.x * s + centered.y * c);
                return rotated + 0.5;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = rotateUV(i.uv, radians(_Angle));
                uv.x = (uv.x - 0.5) / _Aspect + 0.5;

                float dist = distance(uv, float2(0.5, 0.5));
                float alpha = smoothstep(0.5 - _Softness * 0.5, 0.5, dist);
                return fixed4(_Color, (1.0 - alpha) * _Opacity);
            }
            ENDCG
        }
    }
}
