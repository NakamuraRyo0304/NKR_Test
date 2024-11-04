/**************************************************
* File:           DebugFuncCall.cs
*
* Description:    public関数を任意キーで実行する拡張
*
* Update:         2024 / 11 / 04
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;
using System;


/// <summary>
/// Function Execution with Custom Keyboard Shortcuts.
/// </summary>
#if DEBUG

public class DebugFuncCall : MonoBehaviour
{
    [Serializable]
    public class KeyFunctionPair
    {
        public KeyCode key;             // キーコード
        public GameObject targetObject; // 実行したいオブジェクト
        public string componentName;    // Component名
        public string functionName;     // Function名
    }

    // 配列で複数のキーと関数のマッピングを設定可能
    [Header("関数とキーマップ"), SerializeField]
    public KeyFunctionPair[] keyFunctionPairs;

    private void Update()
    {
        // 設定されたすべてのキーをチェック
        foreach (var pair in keyFunctionPairs)
        {
            if (Input.GetKeyDown(pair.key))
            {
                RunFunction(pair);
                // 実行したら抜ける
                break;
            }
        }
    }

    // 設定された関数をすべて実行
    private void RunFunction(KeyFunctionPair pair)
    {
        // 必要な情報がすべて設定されているか確認
        if (pair.targetObject != null &&
            !string.IsNullOrEmpty(pair.componentName) &&
            !string.IsNullOrEmpty(pair.functionName))
        {
            // 指定されたコンポーネントを取得
            Component component = pair.targetObject.GetComponent(pair.componentName);
            if (component != null)
            {
                // リフレクションを使用して指定された関数を取得
                var method = component.GetType().GetMethod(
                    pair.functionName,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic
                );
                if (method != null)
                {
                    // 関数を実行
                    method.Invoke(component, null);
                    return;
                }
            }
        }
    }
}

#endif