/**************************************************
* File:           Rotate.cs
*
* Description:    回転させるクラス
*
* Update:         2024 / 11 / 02
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;


public class Rotate : MonoBehaviour
{
    [SerializeField, SerializePrefab] private float rotate_speed = 15f;

    private void Start()
    {
        // 何か初期化を行う。
    }

    private void Update()
    {
        transform.Rotate(0, rotate_speed * Time.deltaTime, 0, Space.Self);
    }
}
