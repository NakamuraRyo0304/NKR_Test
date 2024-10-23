/**************************************************
* File:           PrefabInspectorAttribute.cs
*
* Description:    プレハブの時のみ編集可能な属性
*
* Update:         2024 / 10 / 23
*
* Author:         Ryo Nakamura
***************************************************/


using System;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class PrefabInspectorAttribute : PropertyAttribute
{
}
