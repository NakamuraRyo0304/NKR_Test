using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSearcher : EditorWindow
{
    // シーンカテゴリを保存する
    private readonly Dictionary<string, List<string>> categories = new();

    // 検索するシーンファイルのパス
    private string folderPath = "Assets/Scenes/";

    // すべてのシーン表示フラグ
    private bool showAllFlag = true;

    // 新しいシーンの保存先パス,名前,拡張子
    private string newScenePath = "Assets/Scenes/";
    private string newSceneName = "SampleScene";
    private string copyStageName = "Assets/Scenes/SampleScene.unity";
    private const string EXTENSION = ".unity";


    // ウィンドウを開くメニューアイテム
    [MenuItem("MyEditor/SceneSearcher")]
    static void WindowOpen()
    {
        // ウィンドウを開く
        GetWindow<SceneSearcher>();
    }



    // ウィンドウのGUIを描画
    private void OnGUI()
    {
        // シーン一覧の表示
        DrawPathField();                // 入力フィールドを表示
        DrawShowAllScenesCheckbox();    // 全表示チェックボックスを表示

        DrawSeparate(2.0f);             // 区切り線
        DrawCreateSceneButton();        // 新規作成ボタンを表示

        DrawSeparate(2.0f);             // 区切り線
        LoadSceneFile();                // シーンファイルを検索してカテゴリ分け
        DrawCategoriesAndScenes();      // カテゴリ分けに基づいてシーン一覧を表示

        DrawSeparate(2.0f);             // 区切り線
        DrawCheckBuildScenesButton();   // 存在確認ボタン
        DrawBuildScene();               // ビルドセッティングに登録したシーン一覧を表示
        DrawAddAndRemoveButtons();      // ビルド追加ボタンと削除ボタンを表示
    }


    // 区切り線を引く
    private void DrawSeparate(float height)
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
    }
    // 入力フィールドを表示
    private void DrawPathField()
    {
        // ファイルパスの入力フィールドを表示
        folderPath = EditorGUILayout.TextField("シーンディレクトリ", folderPath);
    }
    // 全表示チェックボックスを表示
    private void DrawShowAllScenesCheckbox()
    {
        // チェックボックスの状態を更新
        showAllFlag = EditorGUILayout.Toggle("すべてのシーンを表示", showAllFlag);
    }
    // シーンファイルを検索してカテゴリ分け
    private void LoadSceneFile()
    {
        // シーンファイルを保存する辞書を初期化
        categories.Clear();

        // 指定されたパスが存在するか確認
        if (!System.IO.Directory.Exists(folderPath))
        {
            // エラーログを出力して処理を中断
            Debug.LogError("指定されたパスが存在しません。");
            return;
        }

        // 検索オプションを設定
        System.IO.SearchOption searchOption =
            showAllFlag ?
            System.IO.SearchOption.AllDirectories :
            System.IO.SearchOption.TopDirectoryOnly;

        // 指定されたフォルダ内のパスを取得
        string[] scenePaths = System.IO.Directory.GetFiles(folderPath, "*.unity", searchOption);

        foreach (string scenePath in scenePaths)
        {
            // フォルダをカテゴリとして取得
            string sceneFolder = System.IO.Path.GetDirectoryName(scenePath);

            // カテゴリを辞書に追加または更新
            if (!categories.ContainsKey(sceneFolder))
            {
                categories.Add(sceneFolder, new List<string>());
            }
            // カテゴリに追加
            categories[sceneFolder].Add(System.IO.Path.GetFileNameWithoutExtension(scenePath));
        }
    }
    // カテゴリ分けに基づいてシーン一覧を表示
    private void DrawCategoriesAndScenes()
    {
        // シーンファイルが読み込まれているか確認
        if (categories == null || categories.Count == 0)
        {
            EditorGUILayout.LabelField("シーンが見つかりません。");
            return;
        }

        // カテゴリごとに処理
        foreach (var category in categories)
        {
            // 小さい区切り
            DrawSeparate(1.0f);
            // カテゴリの見出しを表示
            EditorGUILayout.LabelField(category.Key);
            // カテゴリ内のシーンを表示
            foreach (string sceneName in category.Value)
            {
                if (GUILayout.Button(sceneName))
                {
                    try
                    {
                        // ボタンを押されたシーンを開く
                        string scenePath = System.IO.Path.Combine(category.Key, sceneName + EXTENSION);
                        OpenScene(scenePath);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"シーンを開く際にエラーが発生しました: {e.Message}");
                    }
                }
            }
        }
    }

    // シーンを開く
    private void OpenScene(string scenePath)
    {
        // 変更があれば保存するか確認
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // シーンファイルが存在するか確認
            if (System.IO.File.Exists(scenePath))
            {
                // シーンを開く
                EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                // エラーログを出力
                Debug.LogError($"指定されたシーンが存在しません: {scenePath}");
            }
        }
    }






    ///////////////////////////////////////////////////////////////
    // シーンの存在確認をし、ビルドセッティングの変更をする
    ///////////////////////////////////////////////////////////////

    // ビルドセッティング内のシーンをチェックするボタン
    private void DrawCheckBuildScenesButton()
    {
        if (GUILayout.Button("ビルドセッティング内のシーンをチェック"))
        {
            CheckBuildScenes();
        }
    }

    // ビルドセッティング内のシーンが存在するかチェック
    private void CheckBuildScenes()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        List<string> missingScenes = new List<string>();

        // 各ビルドシーンが存在するか確認
        foreach (var scene in buildScenes)
        {
            if (!File.Exists(scene.path))
            {
                missingScenes.Add(scene.path);
            }
        }

        if (missingScenes.Count > 0)
        {
            // 存在しないシーンがあれば確認ダイアログを表示
            foreach (string missingScene in missingScenes)
            {
                HandleMissingScene(missingScene);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("確認結果", "すべてのシーンが存在します。", "OK");
        }
    }

    // 存在しないシーンの対応を確認するダイアログ
    private void HandleMissingScene(string missingScenePath)
    {
        bool shouldRemove = EditorUtility.DisplayDialog(
            "存在しないシーンを検出",
            $"シーン '{missingScenePath}' が存在しません。ビルドセッティングから除外しますか？",
            "はい",
            "いいえ");

        if (shouldRemove)
        {
            // 「はい」ならビルドセッティングからシーンを除外
            RemoveSceneFromBuild(missingScenePath);
            Debug.Log($"シーン '{missingScenePath}' をビルドセッティングから除外しました。");
        }
        else
        {
            // 「いいえ」なら新しいシーンのパスを入力させる
            string newScenePath = EditorUtility.OpenFilePanel("新しいシーンの選択", "Assets/Scenes", "unity");
            if (!string.IsNullOrEmpty(newScenePath))
            {
                // 新しいシーンパスをビルドに追加
                AddSceneToBuild(newScenePath);
                Debug.Log($"新しいシーン '{newScenePath}' をビルドセッティングに追加しました。");
            }
            else
            {
                Debug.LogWarning("新しいシーンのパスが入力されませんでした。");
            }
        }
    }








    ///////////////////////////////////////////////////////////////
    // シーンの作成、複製する
    ///////////////////////////////////////////////////////////////

    // シーン新規作成
    private void DrawCreateSceneButton()
    {
        if (GUILayout.Button("新しいシーンを作成"))
        {
            CreateAndDuplicateScene();
        }

        // ファイルパスの入力フィールドを表示
        newScenePath = EditorGUILayout.TextField("シーン保存先", newScenePath);
        newSceneName = EditorGUILayout.TextField("シーン名", newSceneName);
        copyStageName = EditorGUILayout.TextField("コピー元シーン名", copyStageName);
    }
    // 新しいシーンを作成して複製
    private void CreateAndDuplicateScene()
    {
        // ファイルパスが空であれば何もしない
        if (string.IsNullOrEmpty(newScenePath))
        {
            Debug.LogError("ファイルパスを入力してください。");
            return;
        }

        // シーン名に連番を追加する処理
        string sceneNameWithNumber = GetUniqueSceneName(newScenePath, newSceneName);

        // シーンを新規作成
        EditorSceneManager.SaveScene(
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene),
            newScenePath + sceneNameWithNumber + EXTENSION);

        // 作成したシーンにデフォルトのシーン内容をコピー
        CopyDefaultSceneContents(newScenePath + sceneNameWithNumber + EXTENSION);
    }
    // シーン名に連番を付与して一意の名前を生成
    private string GetUniqueSceneName(string path, string baseName)
    {
        string uniqueName = baseName;
        int counter = 1;

        // 連番付きでシーンが存在しないか確認
        while (File.Exists(path + uniqueName + EXTENSION))
        {
            uniqueName = baseName + "(" + counter + ")";
            counter++;
        }

        return uniqueName;
    }
    // デフォルトのシーンの内容をコピーする
    private void CopyDefaultSceneContents(string path)
    {
        // デフォルトのシーンが存在するか確認
        if (!File.Exists(copyStageName))
        {
            Debug.LogError("デフォルトのシーンが見つかりません。");
            return;
        }

        // デフォルトのシーンを読み込む
        var defaultScene = EditorSceneManager.OpenScene(copyStageName, OpenSceneMode.Single);

        // 作成したシーンにコピーする
        EditorSceneManager.SaveScene(defaultScene, path);
    }








    ///////////////////////////////////////////////////////////////
    // ビルドセッティングを編集する
    ///////////////////////////////////////////////////////////////
    // 追加ボタンと削除ボタンを表示
    private void DrawAddAndRemoveButtons()
    {
        GUILayout.BeginHorizontal();

        // 追加ボタン
        if (GUILayout.Button("ビルドに追加"))
        {
            AddSceneToBuild(EditorSceneManager.GetActiveScene().path);
        }

        // 削除ボタン
        if (GUILayout.Button("ビルドから削除"))
        {
            RemoveSceneFromBuild(EditorSceneManager.GetActiveScene().path);
        }

        GUILayout.EndHorizontal();
    }

    // ビルドセッティングにビルドするシーンを追加する
    private void AddSceneToBuild(string scenePath)
    {
        // 現在のビルド設定を取得
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        // シーンがすでに追加されているか確認
        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            if (buildScene.path == scenePath)
            {
                Debug.LogWarning("このシーンは既にビルド設定に追加されています。");
                return;
            }
        }

        // 新しいビルド設定に追加
        ArrayUtility.Add(ref buildScenes, new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = buildScenes;
    }

    // ビルドセッティングからシーンを削除する
    private void RemoveSceneFromBuild(string scenePath)
    {
        // 現在のビルド設定を取得
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        // シーンがビルド設定に含まれているか確認
        for (int i = 0; i < buildScenes.Length; i++)
        {
            if (buildScenes[i].path == scenePath)
            {
                // シーンを削除
                ArrayUtility.RemoveAt(ref buildScenes, i);
                EditorBuildSettings.scenes = buildScenes;
                return;
            }
        }

        Debug.LogWarning("このシーンはビルド設定に含まれていません。");
    }

    // ビルドセッティングに設定されているシーン一覧を表示
    private void DrawBuildScene()
    {
        EditorGUILayout.LabelField("ビルドセッティングに設定されているシーン:");

        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            EditorGUILayout.LabelField(buildScene.path);
        }
    }
}