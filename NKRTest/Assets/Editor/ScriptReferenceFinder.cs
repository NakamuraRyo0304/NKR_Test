using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptReferenceFinder : EditorWindow
{
    // 検索対象となるスクリプト
    private MonoScript targetScript;

    // 参照結果を格納するリスト（オブジェクト）
    private List<string> sceneObjectReferences = new List<string>();
    private List<string> prefabObjectReferences = new List<string>();

    // エディタウィンドウを表示するためのメニュー項目を追加
    [MenuItem("NKR Editor/ScriptReferenceFinder")]
    public static void ShowWindow()
    {
        // エディタウィンドウを作成し、タイトルを設定
        GetWindow<ScriptReferenceFinder>("ScriptReferenceFinder");
    }

    // ウィンドウのGUIを描画する
    void OnGUI()
    {
        // 説明ラベルを表示
        EditorGUILayout.LabelField("ここにスクリプトをドラッグ＆ドロップしてください");

        // ドラッグ＆ドロップ用のエリアを設定
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "ドロップエリア");

        // ドラッグイベントの処理
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // マウス位置がドロップエリア内にあるか確認
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                // ドラッグ時のビジュアルフィードバック
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                // ドロップが完了したときの処理
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    // ドロップされたオブジェクトの処理
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is MonoScript)
                        {
                            // スクリプトをターゲットに設定し、参照を検索
                            targetScript = draggedObject as MonoScript;
                            FindReferences();
                        }
                    }
                }
                break;
        }

        // スクリプトが設定されている場合、検索結果を表示
        if (targetScript != null)
        {
            EditorGUILayout.LabelField("検索結果:", EditorStyles.boldLabel);


            // プレハブを表示してからシーンオブジェクトを表示する
            foreach (string reference in prefabObjectReferences)
                EditorGUILayout.LabelField(reference);
            foreach (string reference in sceneObjectReferences)
                EditorGUILayout.LabelField(reference);
        }
    }

    // スクリプトの参照を検索する
    private void FindReferences()
    {
        // 既存の検索結果をクリア
        sceneObjectReferences.Clear();
        prefabObjectReferences.Clear();

        // シーン内のGameObjectも検索し、階層を含めて表示
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            // オブジェクトにアタッチされているコンポーネントを取得
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                // コンポーネントがターゲットスクリプトのクラスと一致する場合
                if (component != null && component.GetType() == targetScript.GetClass())
                {
                    string fullPath = GetGameObjectPath(obj);
                    string sceneName = obj.scene.name;

                    if (sceneName != null)// シーン名がある場合はオブジェクト
                    {
                        sceneObjectReferences.Add(sceneName + "/" + fullPath);
                    }
                    else
                    {
                        prefabObjectReferences.Add("Prefabs/" + fullPath);
                    }
                }
            }
        }
    }

    // オブジェクトのフルパス（親オブジェクト/子オブジェクト）を取得する
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        // 親オブジェクトが存在する場合は、その親までさかのぼってパスを作成
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
