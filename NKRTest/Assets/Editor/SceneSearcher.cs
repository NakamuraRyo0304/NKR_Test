using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSearcher : EditorWindow
{
    // フォルダ
    private readonly Dictionary<string, List<string>> folders = new();
    private string folderPath = "Assets/Scenes/";
    private bool showAllFlag = true;

    // コピーするシーン
    private string copyScenePath = "Assets/Scenes/";
    private string copySceneName = "SampleScene";

    // 新しく作成するシーン
    private string newScenePath = "Assets/Scenes/";
    private string newSceneName = "SampleScene";
    private const string EXTENSION = ".unity";

    // スクロールに使う
    private Vector2 scenesScroll = Vector2.zero;
    private Vector2 buildsScroll = Vector2.zero;

    // ウィンドウを開く
    [MenuItem("NKR Editor/SceneSearcher")]
    static void WindowOpen() { GetWindow<SceneSearcher>(); }

    private void OnGUI()    // GUI表示をする
    {
        // シーンのディレクトリを指定
        FolderPathField();
        ShowAllScenesToggle();
        Separate(2.0f);

        // シーン作成ボタン
        CreateSceneButton();
        Separate(2.0f);

        // シーンをフォルダごとにボタンで表示（押したら切り替える）
        LoadSceneFile();
        scenesScroll = EditorGUILayout.BeginScrollView(scenesScroll, GUILayout.Height(300));
        FoldersAndScenes();
        EditorGUILayout.EndScrollView();
        
        // ビルドシーンの管理（有無チェックと追加・削除）
        Separate(2.0f);
        CheckBuildScenesButton();
        buildsScroll = EditorGUILayout.BeginScrollView(buildsScroll, GUILayout.Height(150));
        DrawBuildScene();
        EditorGUILayout.EndScrollView();
        AddAndRemoveBuildSettings();
    }

    private void Separate(float height) // 区切り線を引く
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
    }

    private void FolderPathField()    // フォルダパス入力フィールド
    {
        folderPath = EditorGUILayout.TextField("シーンディレクトリ", folderPath);
    }

    private void ShowAllScenesToggle()    // 全てのシーンを表示するトグル
    {
        showAllFlag = EditorGUILayout.Toggle("すべてのシーンを表示", showAllFlag);
    }

    private void LoadSceneFile()    // シーンを読み込む
    {
        folders.Clear();
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("指定されたパスが存在しません。");
            return;
        }

        // 検索オプション：全てのディレクトリを開くかそのフォルダのディレクトリを開くか決定
        SearchOption searchOption = showAllFlag ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] scenePaths = Directory.GetFiles(folderPath, "*.unity", searchOption);

        // パスを検索するフォルダ名を取得して該当のフォルダの中にパスを入れる
        foreach (string scenePath in scenePaths)
        {
            string sceneFolder = Path.GetDirectoryName(scenePath);
            if (!folders.ContainsKey(sceneFolder))
            {
                folders.Add(sceneFolder, new List<string>());
            }
            folders[sceneFolder].Add(Path.GetFileNameWithoutExtension(scenePath));
        }
    }

    private void FoldersAndScenes()  // フォルダとシーンを描画する
    {
        // 何も見つからなければ描画しない
        if (folders == null || folders.Count == 0)
        {
            EditorGUILayout.LabelField("シーンが見つかりません。");
            return;
        }

        // フォルダ毎に線で区切り、シーンボタンを表示
        foreach (var folder in folders)
        {
            Separate(1.0f);
            EditorGUILayout.LabelField(folder.Key);
            foreach (string sceneName in folder.Value)
            {
                if (GUILayout.Button(sceneName))
                {
                    try
                    {
                        string scenePath = Path.Combine(folder.Key, sceneName + EXTENSION);
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

    private void OpenScene(string scenePath)    // シーンを開く
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            if (File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogError($"指定されたシーンが存在しません: {scenePath}");
            }
        }
    }

    private void CheckBuildScenesButton()   // ビルドシーンを確認するボタン
    {
        if (GUILayout.Button("ビルドセッティング内のシーンをチェック"))
        {
            CheckAliveBuildScenes();
        }
    }

    private void CheckAliveBuildScenes() // ビルドシーンが存在するか確認する
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        List<string> missingScenes = new List<string>();

        foreach (var scene in buildScenes)
        {
            if (!File.Exists(scene.path))
            {
                missingScenes.Add(scene.path);
            }
        }

        if (missingScenes.Count > 0)
        {
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

    private void HandleMissingScene(string missingScenePath)    // シーンが含まれなければ除外する
    {
        // 存在しないシーンを自動的に除外
        RemoveSceneFromBuild(missingScenePath);
        Debug.Log($"シーン '{missingScenePath}' が存在しませんでした。ビルドセッティングから自動的に除外しました。");
    }


    private void CreateSceneButton()    // 新しくシーンを作成する
    {
        GUILayout.Label("コピー元からコピー先に複製します");
        GUILayout.Label($"コピー元：{copyScenePath + copySceneName + EXTENSION}");
        GUILayout.Label($"コピー先：{newScenePath + newSceneName + EXTENSION}");

        if (GUILayout.Button("新しくシーンを複製"))
        {
            CopyScene();
        }

        // コピー元フルパス ==> シーン保存先/シーン名.unity に保存
        copyScenePath = EditorGUILayout.TextField("コピー元フォルダパス", copyScenePath);
        copySceneName = EditorGUILayout.TextField("コピー元シーンパス", copySceneName);
        newScenePath = EditorGUILayout.TextField("コピー先フォルダパス", newScenePath);
        newSceneName = EditorGUILayout.TextField("コピー先シーンパス", newSceneName);
    }

    private void CopyScene() // シーンをコピーする
    {
        if (string.IsNullOrEmpty(newScenePath))
        {
            Debug.LogError("ファイルパスを入力してください。");
            return;
        }

        string sceneNameWithNumber = GetUniqueSceneName(newScenePath, newSceneName);
        string fullNewScenePath = newScenePath + sceneNameWithNumber + EXTENSION;

        EditorSceneManager.SaveScene(
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene),
            fullNewScenePath);

        CopyDefaultSceneContents(fullNewScenePath);
    }

    private string GetUniqueSceneName(string path, string baseName) // シーンに連番を振る
    {
        string uniqueName = baseName;
        int counter = 1;

        while (File.Exists(path + uniqueName + EXTENSION))
        {
            uniqueName = baseName + "(" + counter + ")";
            counter++;
        }

        return uniqueName;
    }

    private void CopyDefaultSceneContents(string path) // デフォルトのシーンをコピーする
    {
        var fullPath = copyScenePath + copySceneName + EXTENSION;
        if (!File.Exists(fullPath))
        {
            Debug.LogError("デフォルトのシーンが見つかりません。");
            return;
        }

        var defaultScene = EditorSceneManager.OpenScene(fullPath, OpenSceneMode.Single);
        EditorSceneManager.SaveScene(defaultScene, path);
    }

    private void AddAndRemoveBuildSettings() // 追加と削除ボタン
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ビルドに追加"))
        {
            AddSceneToBuild(EditorSceneManager.GetActiveScene().path);
        }

        if (GUILayout.Button("ビルドから削除"))
        {
            RemoveSceneFromBuild(EditorSceneManager.GetActiveScene().path);
        }
        GUILayout.EndHorizontal();
    }

    private void AddSceneToBuild(string scenePath) // ビルドに追加する
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            if (buildScene.path == scenePath)
            {
                Debug.LogWarning("このシーンは既にビルド設定に追加されています。");
                return;
            }
        }

        ArrayUtility.Add(ref buildScenes, new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = buildScenes;
    }

    private void RemoveSceneFromBuild(string scenePath) // ビルドからさくじょする
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        for (int i = 0; i < buildScenes.Length; i++)
        {
            if (buildScenes[i].path == scenePath)
            {
                ArrayUtility.RemoveAt(ref buildScenes, i);
                EditorBuildSettings.scenes = buildScenes;
                return;
            }
        }

        Debug.LogWarning("このシーンはビルド設定に含まれていません。");
    }

    private void DrawBuildScene() // ビルドシーンを描画する
    {
        EditorGUILayout.LabelField("ビルドセッティングに設定されているシーン:");
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            EditorGUILayout.LabelField(buildScene.path);
        }
    }
}
