Shader "Custom/ScanLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // テクスチャ
        _OpacityScanLine ("Opacity ScanLine", Range(0, 2)) = 0.8 // 通常の細かい線
        _OpacityNoise ("Opacity Noise", Range(0, 1)) = 0.1 // ノイズの強さ
        _FlickeringSpeed ("Flickering Speed", Range(0, 1000)) = 600 // 流れる速度
        _FlickeringStrength ("Flickering Strength", Range(0, 0.1)) = 0.02 // 流れる激しさ
        _ScanlineThickness ("Scanline Thickness", Range(0.001, 1.0)) = 0.02 // 太さ
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1) // 色調用のプロパティ
        _BigLineSpeed ("Big Line Speed", Range(0, 10)) = 1.0 // 太い線の速度
        _BigLineSpan ("Big Line Span", Range(0.01, 0.1)) = 0.05 // 太い線の間隔
        _BigLineColor ("Big Line Color", Color) = (1, 0, 0, 1) // 太い線の色
        _BigLineThickness ("Big Line Thickness", Range(0.001, 1.0)) = 0.02 // 太い線の太さ
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0 // アルファの値
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex; // テクスチャ
            half _OpacityScanLine; // 走査線
            half _OpacityNoise; // ノイズの強さ
            half _FlickeringSpeed; // 流れる速度
            half _FlickeringStrength;  // 流れる激しさ
            half _ScanlineThickness; // 走査線の太さ
            float4 _TintColor; // フィルター色
            float _Alpha; // 全体のアルファ
            half _BigLineSpeed; // 太い線の速度
            half _BigLineSpan; // 太い線の間隔
            float4 _BigLineColor; // 太い線の色
            half _BigLineThickness; // 太い線の太さ

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // サンプリング
                fixed4 img = tex2D(_MainTex, i.uv);

                // UVの座標更新
                float posY = i.uv.y + _Time.y * 0.5; 
                posY = frac(posY);

                // 走査線の計算
                float pattern = sin(posY * 200.0);
                float scanline = (pattern > (0.5 - _ScanlineThickness * 0.5) && pattern < (0.5 + _ScanlineThickness * 0.5)) ? 1.0 : 0.0;

                // 走査線の色の計算
                float3 col = float3(scanline, scanline, scanline) * _OpacityScanLine;
                float n = random(i.uv * _Time.y);
                col += float3(n, n, n) * _OpacityNoise;

                // 画面の点滅
                float flash = (sin(_FlickeringSpeed * _Time.y) + 1) * 0.5;
                col += float3(flash, flash, flash) * _FlickeringStrength;

                // 太い線の計算
                float bigLineY = i.uv.y + _Time.y * _BigLineSpeed;
                float bigLinePattern = sin(bigLineY * (1.0 / _BigLineSpan));
                float bigLine = (bigLinePattern > (0.5 - _BigLineThickness * 0.5) &&
                bigLinePattern < (0.5 + _BigLineThickness * 0.5)) ? 1.0 : 0.0;

                // 元のテクスチャに色調を適用
                fixed4 finalColor = img * _TintColor;
                
                // 太い線を元のテクスチャに乗算
                finalColor.rgb = lerp(finalColor.rgb, finalColor.rgb * _BigLineColor.rgb, bigLine); 

                // 走査線の色を元のテクスチャに加算
                finalColor.rgb += col;
                finalColor.a = _Alpha;

                return finalColor;
            }

            ENDCG
        }
    }
}
