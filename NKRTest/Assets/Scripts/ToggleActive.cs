/**************************************************
* File:           ToggleActive.cs
*
* Description:    アクティブを切り替える
*
* Update:         2024 / 11 / 04
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;


public class ToggleActive : MonoBehaviour
{
    // メッシュレンダラー
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // アクティブを入れ替える
    public void Reverce()
    {
        meshRenderer.enabled = !meshRenderer.enabled;
    }
}