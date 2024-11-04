/**************************************************
* File:           DebugFuncCallEditor.cs
*
* Description:    public関数を任意キーで実行する拡張
*
* Update:         2024 / 11 / 04
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using System;
using System.Reflection;


/// <summary>
/// Function Execution with Custom Keyboard Shortcuts.
/// </summary>
[CustomEditor(typeof(DebugFuncCall))]
public class DebugFuncCallEditor : Editor
{
    // キーコード選択用のドロップダウン
    private KeyCodeDropdown keyCodeDropdown;
    private int indexToRemove = -1; // 削除するインデックスを保持

    private void OnEnable()
    {
        // キーコードドロップダウンの初期化
        keyCodeDropdown = new KeyCodeDropdown(new AdvancedDropdownState());
    }

    public override void OnInspectorGUI()
    {
        DebugFuncCall executor = (DebugFuncCall)target;

        EditorGUI.BeginChangeCheck();

        // keyFunctionPairsがnullの場合、初期化する
        if (executor.keyFunctionPairs == null)
        {
            executor.keyFunctionPairs = new DebugFuncCall.KeyFunctionPair[0];
        }

        // キーと関数のペアの数を設定
        int size = EditorGUILayout.IntField("Size", executor.keyFunctionPairs.Length);

        // 変更されたら再設定を行う
        if (size != executor.keyFunctionPairs.Length)
        {
            Array.Resize(ref executor.keyFunctionPairs, size);
        }

        // 各ペアの設定
        for (int i = 0; i < executor.keyFunctionPairs.Length; i++)
        {
            // 各要素がnullの場合、新しいインスタンスを作成
            if (executor.keyFunctionPairs[i] == null)
            {
                executor.keyFunctionPairs[i] = new DebugFuncCall.KeyFunctionPair();
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);

            // ペアのラベルを表示
            EditorGUILayout.LabelField($"Pair {i + 1}", EditorStyles.boldLabel);

            // 右クリックメニューの処理
            Rect pairRect = GUILayoutUtility.GetLastRect(); // 最後に描画された矩形を取得
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1 && pairRect.Contains(currentEvent.mousePosition))
            {
                int index = i;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => indexToRemove = index);
                menu.ShowAsContext();
                currentEvent.Use();
            }

            // キー選択用のカスタムフィールド
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Key");
            if (GUILayout.Button(executor.keyFunctionPairs[i].key.ToString(), EditorStyles.popup))
            {
                int index = i;
                keyCodeDropdown.Show(GUILayoutUtility.GetLastRect(),
                (selectedKey) =>
                {
                    executor.keyFunctionPairs[index].key = selectedKey;
                    Repaint();
                });
            }
            EditorGUILayout.EndHorizontal();

            // ターゲットオブジェクトの設定
            executor.keyFunctionPairs[i].targetObject = (GameObject)EditorGUILayout.ObjectField(
                "Target Object", executor.keyFunctionPairs[i].targetObject,
                typeof(GameObject), true
            );

            // ターゲットオブジェクトがあればComponent、Functionの設定
            if (executor.keyFunctionPairs[i].targetObject != null)
            {
                // コンポーネントの選択
                var components = executor.keyFunctionPairs[i].targetObject.GetComponents<MonoBehaviour>();
                var componentNames = components.Select(c => c.GetType().Name).ToArray();

                // 実行コンポーネントをコンポーネントに登録
                int selectedComponentIndex = Array.IndexOf(componentNames, executor.keyFunctionPairs[i].componentName);
                selectedComponentIndex = EditorGUILayout.Popup("Component", selectedComponentIndex, componentNames);

                // 選択されたコンポーネントが範囲内かチェック
                if (selectedComponentIndex >= 0 && selectedComponentIndex < componentNames.Length)
                {
                    executor.keyFunctionPairs[i].componentName = componentNames[selectedComponentIndex];

                    // コンポーネントの選択
                    var selectedComponent = components[selectedComponentIndex];
                    var methods = selectedComponent.GetType().GetMethods(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.GetParameters().Length == 0 && !m.IsSpecialName)
                        .Select(m => m.Name)
                        .ToArray();

                    // 実行関数をファンクションに登録
                    int selectedMethodIndex = Array.IndexOf(methods, executor.keyFunctionPairs[i].functionName);
                    selectedMethodIndex = EditorGUILayout.Popup("Function", selectedMethodIndex, methods);

                    // 選択された関数が範囲内かチェック
                    if (selectedMethodIndex >= 0 && selectedMethodIndex < methods.Length)
                    {
                        executor.keyFunctionPairs[i].functionName = methods[selectedMethodIndex];
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        // 削除処理
        if (indexToRemove != -1)
        {
            var list = executor.keyFunctionPairs.ToList();
            list.RemoveAt(indexToRemove);
            executor.keyFunctionPairs = list.ToArray();
            indexToRemove = -1; // 削除後にリセット
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // 状態が変更されたことをUnityに通知
        }
    }
}

// キーコード選択用のカスタムドロップダウン
public class KeyCodeDropdown : AdvancedDropdown
{
    private Action<KeyCode> onKeySelected;

    public KeyCodeDropdown(AdvancedDropdownState state) : base(state)
    {
        minimumSize = new Vector2(200, 300);
    }

    // ドロップダウンを表示し、選択時のコールバックを設定
    public void Show(Rect rect, Action<KeyCode> onKeySelected)
    {
        this.onKeySelected = onKeySelected;
        Show(rect);
    }

    // ドロップダウンの項目を構築
    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("KeyCodes");

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            root.AddChild(new AdvancedDropdownItem(keyCode.ToString())
            {
                id = (int)keyCode
            });
        }

        return root;
    }

    // 項目が選択されたときの処理
    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        onKeySelected?.Invoke((KeyCode)item.id);
    }
}
