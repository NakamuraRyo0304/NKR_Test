/**************************************************
* File:           Rotate.cs
*
* Description:    回転させるクラス
*
* Update:         2024 / 10 / 23
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;


public class Rotate : MonoBehaviour
{
    [SerializeField, PrefabInspector] private float rotate_speed = 15f;

    void Start()
    {
        // 何か初期化を行う。
    }

    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotate_speed, 0, Space.Self);
    }
}
