/**************************************************
* File:           EGTV_Effect.cs
*
* Description:    走査線エフェクト
*
* Update:         2024 / 10 / 23
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class EGTV_Effect : MonoBehaviour
{
    [Tooltip("走査線エフェクト用のテクスチャ"), SerializeField]    private RenderTexture drawTexture;
    [Tooltip("走査線エフェクト用のマテリアル"), SerializeField]    private Material scanlineMaterial;
    [Tooltip("画像オブジェクト"), SerializeField]                  private RawImage screen;

    [Header("通常の走査線")]
    [Tooltip("走査線の速度"), SerializeField] private float scanlineSpeed = 1.0f;
    [Tooltip("走査線の強度"), SerializeField] private float scanlineIntensity = 0.5f;
    [Tooltip("走査線の太さ"), SerializeField] private float scanlineThickness = 0.05f;
    [Tooltip("全体の色調"), SerializeField] private Color tintColor = Color.green;

    [Header("太い走査線")]
    [Tooltip("太い線の速度"), SerializeField] private float bigLineSpeed = 1.0f;
    [Tooltip("太い線の太さ"), SerializeField] private float bigLineThickness = 0.02f;
    [Tooltip("太い線の間隔"), SerializeField] private float bigLineSpan = 0.05f;
    [Tooltip("太い線の色"), SerializeField] private Color bigLineColor = Color.green;


    void Start()
    {
        // 設定
        InitialSetting();
    }

    private void InitialSetting()
    {
        // スクリーンの初期設定
        if (screen != null)
        {
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, 1);

            if (scanlineMaterial != null)
            {
                scanlineMaterial.SetTexture("_MainTex", drawTexture); // RenderTextureを設定
                scanlineMaterial.SetFloat("_OpacityScanLine", scanlineIntensity); // 強度を設定
                scanlineMaterial.SetFloat("_ScanlineThickness", scanlineThickness); // 太さを設定
                scanlineMaterial.SetColor("_TintColor", tintColor); // 色調を設定

                // 太い線のパラメータを設定
                scanlineMaterial.SetFloat("_BigLineSpeed", bigLineSpeed); // 太い線の速度を設定
                scanlineMaterial.SetFloat("_BigLineSpan", bigLineSpan); // 太い線の間隔を設定
                scanlineMaterial.SetColor("_BigLineColor", bigLineColor); // 太い線の色を設定
                scanlineMaterial.SetFloat("_BigLineThickness", bigLineThickness); // 太い線の太さを設定
                scanlineMaterial.SetFloat("_Alpha", 1); // 初期アルファ値を0に設定
            }
        }

        screen.material = scanlineMaterial;
    }


    void Update()
    {
        // 明るさの変化を時間に応じて設定
        float brightness = Mathf.PingPong(Time.time * scanlineSpeed, 1.0f) * scanlineIntensity;
        scanlineMaterial.SetFloat("_OpacityScanLine", brightness);
    }

    public void OnDestroy()
    {
        _ = (screen?.DOKill());
    }
}
