/**************************************************
* File:           PrefabInspectorAttribute.cs
*
* Description:    �v���n�u�̎��̂ݕҏW�\�ȑ���
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
