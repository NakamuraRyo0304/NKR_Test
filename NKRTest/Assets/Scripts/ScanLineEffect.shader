Shader "Custom/ScanLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // �e�N�X�`��
        _OpacityScanLine ("Opacity ScanLine", Range(0, 2)) = 0.8 // �ʏ�ׂ̍�����
        _OpacityNoise ("Opacity Noise", Range(0, 1)) = 0.1 // �m�C�Y�̋���
        _FlickeringSpeed ("Flickering Speed", Range(0, 1000)) = 600 // ����鑬�x
        _FlickeringStrength ("Flickering Strength", Range(0, 0.1)) = 0.02 // ����錃����
        _ScanlineThickness ("Scanline Thickness", Range(0.001, 1.0)) = 0.02 // ����
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1) // �F���p�̃v���p�e�B
        _BigLineSpeed ("Big Line Speed", Range(0, 10)) = 1.0 // �������̑��x
        _BigLineSpan ("Big Line Span", Range(0.01, 0.1)) = 0.05 // �������̊Ԋu
        _BigLineColor ("Big Line Color", Color) = (1, 0, 0, 1) // �������̐F
        _BigLineThickness ("Big Line Thickness", Range(0.001, 1.0)) = 0.02 // �������̑���
        _Alpha("Alpha", Range(0.0, 1.0)) = 1.0 // �A���t�@�̒l
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

            sampler2D _MainTex; // �e�N�X�`��
            half _OpacityScanLine; // ������
            half _OpacityNoise; // �m�C�Y�̋���
            half _FlickeringSpeed; // ����鑬�x
            half _FlickeringStrength;  // ����錃����
            half _ScanlineThickness; // �������̑���
            float4 _TintColor; // �t�B���^�[�F
            float _Alpha; // �S�̂̃A���t�@
            half _BigLineSpeed; // �������̑��x
            half _BigLineSpan; // �������̊Ԋu
            float4 _BigLineColor; // �������̐F
            half _BigLineThickness; // �������̑���

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // �T���v�����O
                fixed4 img = tex2D(_MainTex, i.uv);

                // UV�̍��W�X�V
                float posY = i.uv.y + _Time.y * 0.5; 
                posY = frac(posY);

                // �������̌v�Z
                float pattern = sin(posY * 200.0);
                float scanline = (pattern > (0.5 - _ScanlineThickness * 0.5) && pattern < (0.5 + _ScanlineThickness * 0.5)) ? 1.0 : 0.0;

                // �������̐F�̌v�Z
                float3 col = float3(scanline, scanline, scanline) * _OpacityScanLine;
                float n = random(i.uv * _Time.y);
                col += float3(n, n, n) * _OpacityNoise;

                // ��ʂ̓_��
                float flash = (sin(_FlickeringSpeed * _Time.y) + 1) * 0.5;
                col += float3(flash, flash, flash) * _FlickeringStrength;

                // �������̌v�Z
                float bigLineY = i.uv.y + _Time.y * _BigLineSpeed;
                float bigLinePattern = sin(bigLineY * (1.0 / _BigLineSpan));
                float bigLine = (bigLinePattern > (0.5 - _BigLineThickness * 0.5) &&
                bigLinePattern < (0.5 + _BigLineThickness * 0.5)) ? 1.0 : 0.0;

                // ���̃e�N�X�`���ɐF����K�p
                fixed4 finalColor = img * _TintColor;
                
                // �����������̃e�N�X�`���ɏ�Z
                finalColor.rgb = lerp(finalColor.rgb, finalColor.rgb * _BigLineColor.rgb, bigLine); 

                // �������̐F�����̃e�N�X�`���ɉ��Z
                finalColor.rgb += col;
                finalColor.a = _Alpha;

                return finalColor;
            }

            ENDCG
        }
    }
}
