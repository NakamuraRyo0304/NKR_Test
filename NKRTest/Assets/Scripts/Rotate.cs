/**************************************************
* File:           Rotate.cs
*
* Description:    ‰ñ“]‚³‚¹‚éƒNƒ‰ƒX
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
        
    }

    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotate_speed, 0, Space.Self);
    }
}
