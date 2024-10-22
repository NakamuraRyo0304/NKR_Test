/**************************************************
* File:           HideInspectorForPrefabDrawer.cs
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


[CustomPropertyDrawer(typeof(HideInspectorForPrefabAttribute))]
public class HideInspectorForPrefabDrawer : PropertyDrawer
{
    // 編集不可能フィールドの設定
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // エディタモードかどうかをチェックする
        var obj = property.serializedObject.targetObject;
        bool isInPrefabEditMode = PrefabStageUtility.GetCurrentPrefabStage() ||
                                  PrefabUtility.IsPartOfPrefabAsset(obj);

        // プレハブエディタじゃないときは表示しない
        if (!isInPrefabEditMode) return;
     
        EditorGUI.PropertyField(position, property, label, true);
    }

    // プロパティの高さを設定する
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // エディタモードかどうかをチェックする
        var obj = property.serializedObject.targetObject;
        bool isInPrefabEditMode = PrefabStageUtility.GetCurrentPrefabStage() ||
                                  PrefabUtility.IsPartOfPrefabAsset(obj);

        // プレハブエディタじゃないときは表示しない
        if (!isInPrefabEditMode) return 0;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
