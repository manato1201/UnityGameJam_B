Shader "Custom/WaveShader"
{
    Properties
    {
        _Speed ("Wave Speed", Float) = 1.0 // �g�̑��x�i���Ԃ̌o�߂ɂ��ω��j
        _Amplitude ("Wave Amplitude", Float) = 0.1 // �g�̐U���i�g�̍����j
        _Frequency ("Wave Frequency", Float) = 5.0 // �g�̎��g���i�g�̖��x�j
        _ThresholdMin ("Height Threshold Min", Float) = 0.0 // �g��K�p���鍂����臒l
        _ThresholdMax ("Height Threshold Max", Float) = 0.0 // �g��K�p���鍂����臒l
        _Color ("Main Color", Color) = (1,1,1,1) // Color �v���p�e�B�[ (�f�t�H���g�͔�) 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // ���_�f�[�^�̍\���́i���́j
            struct appdata_t
            {
                float4 vertex : POSITION; // ���_���W
                float2 uv : TEXCOORD0; // �e�N�X�`�����W
            };

            // ���_�V�F�[�_�[�̏o�̓f�[�^�\����
            struct v2f
            {
                float4 vertex : SV_POSITION; // �ϊ���̒��_���W
                float2 uv : TEXCOORD0; // �e�N�X�`�����W
            };

            // �g�̃p�����[�^�iUnity����ݒ�\�j
            float _Speed, _Amplitude, _Frequency, _ThresholdMin, _ThresholdMax;
            fixed4 _Color; // �}�e���A������̃J���[   a____

            // ���_�V�F�[�_�[�F���_�̈ʒu��g�̓����ɉ����ĕω�������
            v2f vert (appdata_t v)
            {
                v2f o;
                if (v.vertex.y > _ThresholdMin && v.vertex.y < _ThresholdMax) // y���W��臒l�𒴂��Ă���ꍇ
                {
                    float wave = sin(_Time.y * _Speed + v.vertex.x * _Frequency) * _Amplitude;
                    v.vertex.y += wave; // �g��K�p
                }
                o.vertex = UnityObjectToClipPos(v.vertex); // �N���b�v��Ԃɕϊ�
                o.uv = v.uv; // �e�N�X�`�����W�������p��
                return o;

            }

            // �t���O�����g�V�F�[�_�[�F�V���v����UV�x�[�X�̐F���o��
            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; 
            }
            ENDCG
        }
    }
}