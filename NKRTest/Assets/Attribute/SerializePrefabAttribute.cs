/**************************************************
* File:           SerializePrefabAttribute.cs
*
* Description:    プレハブの時のみ編集可能な属性
*
* Update:         2024 / 11 / 01
*
* Author:         Ryo Nakamura
***************************************************/


using System;
using UnityEngine;


/// <summary>
/// Used in conjunction with SerializeField, it can be displayed only when Prefab is being edited.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SerializePrefabAttribute : PropertyAttribute
{
    // 単純なロジックのため特記事項はなし
}
