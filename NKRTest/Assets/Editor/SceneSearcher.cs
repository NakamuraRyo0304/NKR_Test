using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSearcher : EditorWindow
{
    // �t�H���_
    private readonly Dictionary<string, List<string>> folders = new();
    private string folderPath = "Assets/Scenes/";
    private bool showAllFlag = true;

    // �R�s�[����V�[��
    private string copyScenePath = "Assets/Scenes/";
    private string copySceneName = "SampleScene";

    // �V�����쐬����V�[��
    private string newScenePath = "Assets/Scenes/";
    private string newSceneName = "SampleScene";
    private const string EXTENSION = ".unity";

    // �X�N���[���Ɏg��
    private Vector2 scenesScroll = Vector2.zero;
    private Vector2 buildsScroll = Vector2.zero;

    // �E�B���h�E���J��
    [MenuItem("NKR Editor/SceneSearcher")]
    static void WindowOpen() { GetWindow<SceneSearcher>(); }

    private void OnGUI()    // GUI�\��������
    {
        // �V�[���̃f�B���N�g�����w��
        FolderPathField();
        ShowAllScenesToggle();
        Separate(2.0f);

        // �V�[���쐬�{�^��
        CreateSceneButton();
        Separate(2.0f);

        // �V�[�����t�H���_���ƂɃ{�^���ŕ\���i��������؂�ւ���j
        LoadSceneFile();
        scenesScroll = EditorGUILayout.BeginScrollView(scenesScroll, GUILayout.Height(300));
        FoldersAndScenes();
        EditorGUILayout.EndScrollView();
        
        // �r���h�V�[���̊Ǘ��i�L���`�F�b�N�ƒǉ��E�폜�j
        Separate(2.0f);
        CheckBuildScenesButton();
        buildsScroll = EditorGUILayout.BeginScrollView(buildsScroll, GUILayout.Height(150));
        DrawBuildScene();
        EditorGUILayout.EndScrollView();
        AddAndRemoveBuildSettings();
    }

    private void Separate(float height) // ��؂��������
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
    }

    private void FolderPathField()    // �t�H���_�p�X���̓t�B�[���h
    {
        folderPath = EditorGUILayout.TextField("�V�[���f�B���N�g��", folderPath);
    }

    private void ShowAllScenesToggle()    // �S�ẴV�[����\������g�O��
    {
        showAllFlag = EditorGUILayout.Toggle("���ׂẴV�[����\��", showAllFlag);
    }

    private void LoadSceneFile()    // �V�[����ǂݍ���
    {
        folders.Clear();
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("�w�肳�ꂽ�p�X�����݂��܂���B");
            return;
        }

        // �����I�v�V�����F�S�Ẵf�B���N�g�����J�������̃t�H���_�̃f�B���N�g�����J��������
        SearchOption searchOption = showAllFlag ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] scenePaths = Directory.GetFiles(folderPath, "*.unity", searchOption);

        // �p�X����������t�H���_�����擾���ĊY���̃t�H���_�̒��Ƀp�X������
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

    private void FoldersAndScenes()  // �t�H���_�ƃV�[����`�悷��
    {
        // ����������Ȃ���Ε`�悵�Ȃ�
        if (folders == null || folders.Count == 0)
        {
            EditorGUILayout.LabelField("�V�[����������܂���B");
            return;
        }

        // �t�H���_���ɐ��ŋ�؂�A�V�[���{�^����\��
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
                        Debug.LogError($"�V�[�����J���ۂɃG���[���������܂���: {e.Message}");
                    }
                }
            }
        }
    }

    private void OpenScene(string scenePath)    // �V�[�����J��
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            if (File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogError($"�w�肳�ꂽ�V�[�������݂��܂���: {scenePath}");
            }
        }
    }

    private void CheckBuildScenesButton()   // �r���h�V�[�����m�F����{�^��
    {
        if (GUILayout.Button("�r���h�Z�b�e�B���O���̃V�[�����`�F�b�N"))
        {
            CheckAliveBuildScenes();
        }
    }

    private void CheckAliveBuildScenes() // �r���h�V�[�������݂��邩�m�F����
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
            EditorUtility.DisplayDialog("�m�F����", "���ׂẴV�[�������݂��܂��B", "OK");
        }
    }

    private void HandleMissingScene(string missingScenePath)    // �V�[�����܂܂�Ȃ���Ώ��O����
    {
        // ���݂��Ȃ��V�[���������I�ɏ��O
        RemoveSceneFromBuild(missingScenePath);
        Debug.Log($"�V�[�� '{missingScenePath}' �����݂��܂���ł����B�r���h�Z�b�e�B���O���玩���I�ɏ��O���܂����B");
    }


    private void CreateSceneButton()    // �V�����V�[�����쐬����
    {
        GUILayout.Label("�R�s�[������R�s�[��ɕ������܂�");
        GUILayout.Label($"�R�s�[���F{copyScenePath + copySceneName + EXTENSION}");
        GUILayout.Label($"�R�s�[��F{newScenePath + newSceneName + EXTENSION}");

        if (GUILayout.Button("�V�����V�[���𕡐�"))
        {
            CopyScene();
        }

        // �R�s�[���t���p�X ==> �V�[���ۑ���/�V�[����.unity �ɕۑ�
        copyScenePath = EditorGUILayout.TextField("�R�s�[���t�H���_�p�X", copyScenePath);
        copySceneName = EditorGUILayout.TextField("�R�s�[���V�[���p�X", copySceneName);
        newScenePath = EditorGUILayout.TextField("�R�s�[��t�H���_�p�X", newScenePath);
        newSceneName = EditorGUILayout.TextField("�R�s�[��V�[���p�X", newSceneName);
    }

    private void CopyScene() // �V�[�����R�s�[����
    {
        if (string.IsNullOrEmpty(newScenePath))
        {
            Debug.LogError("�t�@�C���p�X����͂��Ă��������B");
            return;
        }

        string sceneNameWithNumber = GetUniqueSceneName(newScenePath, newSceneName);
        string fullNewScenePath = newScenePath + sceneNameWithNumber + EXTENSION;

        EditorSceneManager.SaveScene(
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene),
            fullNewScenePath);

        CopyDefaultSceneContents(fullNewScenePath);
    }

    private string GetUniqueSceneName(string path, string baseName) // �V�[���ɘA�Ԃ�U��
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

    private void CopyDefaultSceneContents(string path) // �f�t�H���g�̃V�[�����R�s�[����
    {
        var fullPath = copyScenePath + copySceneName + EXTENSION;
        if (!File.Exists(fullPath))
        {
            Debug.LogError("�f�t�H���g�̃V�[����������܂���B");
            return;
        }

        var defaultScene = EditorSceneManager.OpenScene(fullPath, OpenSceneMode.Single);
        EditorSceneManager.SaveScene(defaultScene, path);
    }

    private void AddAndRemoveBuildSettings() // �ǉ��ƍ폜�{�^��
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("�r���h�ɒǉ�"))
        {
            AddSceneToBuild(EditorSceneManager.GetActiveScene().path);
        }

        if (GUILayout.Button("�r���h����폜"))
        {
            RemoveSceneFromBuild(EditorSceneManager.GetActiveScene().path);
        }
        GUILayout.EndHorizontal();
    }

    private void AddSceneToBuild(string scenePath) // �r���h�ɒǉ�����
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            if (buildScene.path == scenePath)
            {
                Debug.LogWarning("���̃V�[���͊��Ƀr���h�ݒ�ɒǉ�����Ă��܂��B");
                return;
            }
        }

        ArrayUtility.Add(ref buildScenes, new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = buildScenes;
    }

    private void RemoveSceneFromBuild(string scenePath) // �r���h���炳�����傷��
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

        Debug.LogWarning("���̃V�[���̓r���h�ݒ�Ɋ܂܂�Ă��܂���B");
    }

    private void DrawBuildScene() // �r���h�V�[����`�悷��
    {
        EditorGUILayout.LabelField("�r���h�Z�b�e�B���O�ɐݒ肳��Ă���V�[��:");
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            EditorGUILayout.LabelField(buildScene.path);
        }
    }
}
