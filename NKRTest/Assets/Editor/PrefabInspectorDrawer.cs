/**************************************************
* File:           PrefabInspectorDrawer.cs
*
* Description:    プレハブの時のみ編集可能な属性
*
* Update:         2024 / 10 / 23
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomPropertyDrawer(typeof(PrefabInspectorAttribute))]
public class PrefabInspectorDrawer : PropertyDrawer
{
    // 編集不可能フィールドの設定
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // プレハブを開いていたら実行
        if (IsEditorMode(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    // プロパティの高さを設定する
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // エディタモードなら高さを作成する
        // そうでなければ高さを０にしてつぶす
        return IsEditorMode(property) ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
    }

    private bool IsEditorMode(SerializedProperty property)
    {
        // エディタモードかどうかをチェックする
        var obj = property.serializedObject.targetObject;
        bool isInPrefabEditMode = PrefabStageUtility.GetCurrentPrefabStage() ||
                                  PrefabUtility.IsPartOfPrefabAsset(obj);

        // エディタモードか判定を返す
        return isInPrefabEditMode;
    }
}
