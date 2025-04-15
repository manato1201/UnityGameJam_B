Shader "Custom/WaveShader"
{
    Properties
    {
        _Speed ("Wave Speed", Float) = 1.0 // 波の速度（時間の経過による変化）
        _Amplitude ("Wave Amplitude", Float) = 0.1 // 波の振幅（波の高さ）
        _Frequency ("Wave Frequency", Float) = 5.0 // 波の周波数（波の密度）
        _ThresholdMin ("Height Threshold Min", Float) = 0.0 // 波を適用する高さの閾値
        _ThresholdMax ("Height Threshold Max", Float) = 0.0 // 波を適用する高さの閾値
        _Color ("Main Color", Color) = (1,1,1,1) // Color プロパティー (デフォルトは白) 
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

            // 頂点データの構造体（入力）
            struct appdata_t
            {
                float4 vertex : POSITION; // 頂点座標
                float2 uv : TEXCOORD0; // テクスチャ座標
            };

            // 頂点シェーダーの出力データ構造体
            struct v2f
            {
                float4 vertex : SV_POSITION; // 変換後の頂点座標
                float2 uv : TEXCOORD0; // テクスチャ座標
            };

            // 波のパラメータ（Unityから設定可能）
            float _Speed, _Amplitude, _Frequency, _ThresholdMin, _ThresholdMax;
            fixed4 _Color; // マテリアルからのカラー   a____

            // 頂点シェーダー：頂点の位置を波の動きに応じて変化させる
            v2f vert (appdata_t v)
            {
                v2f o;
                if (v.vertex.y > _ThresholdMin && v.vertex.y < _ThresholdMax) // y座標が閾値を超えている場合
                {
                    float wave = sin(_Time.y * _Speed + v.vertex.x * _Frequency) * _Amplitude;
                    v.vertex.y += wave; // 波を適用
                }
                o.vertex = UnityObjectToClipPos(v.vertex); // クリップ空間に変換
                o.uv = v.uv; // テクスチャ座標を引き継ぐ
                return o;

            }

            // フラグメントシェーダー：シンプルなUVベースの色を出力
            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; 
            }
            ENDCG
        }
    }
}