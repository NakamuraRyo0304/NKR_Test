using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSearcher : EditorWindow
{
    // �V�[���J�e�S����ۑ�����
    private readonly Dictionary<string, List<string>> categories = new();

    // ��������V�[���t�@�C���̃p�X
    private string folderPath = "Assets/Scenes/";

    // ���ׂẴV�[���\���t���O
    private bool showAllFlag = true;

    // �V�����V�[���̕ۑ���p�X,���O,�g���q
    private string newScenePath = "Assets/Scenes/";
    private string newSceneName = "SampleScene";
    private string copyStageName = "Assets/Scenes/SampleScene.unity";
    private const string EXTENSION = ".unity";


    // �E�B���h�E���J�����j���[�A�C�e��
    [MenuItem("MyEditor/SceneSearcher")]
    static void WindowOpen()
    {
        // �E�B���h�E���J��
        GetWindow<SceneSearcher>();
    }



    // �E�B���h�E��GUI��`��
    private void OnGUI()
    {
        // �V�[���ꗗ�̕\��
        DrawPathField();                // ���̓t�B�[���h��\��
        DrawShowAllScenesCheckbox();    // �S�\���`�F�b�N�{�b�N�X��\��

        DrawSeparate(2.0f);             // ��؂��
        DrawCreateSceneButton();        // �V�K�쐬�{�^����\��

        DrawSeparate(2.0f);             // ��؂��
        LoadSceneFile();                // �V�[���t�@�C�����������ăJ�e�S������
        DrawCategoriesAndScenes();      // �J�e�S�������Ɋ�Â��ăV�[���ꗗ��\��

        DrawSeparate(2.0f);             // ��؂��
        DrawCheckBuildScenesButton();   // ���݊m�F�{�^��
        DrawBuildScene();               // �r���h�Z�b�e�B���O�ɓo�^�����V�[���ꗗ��\��
        DrawAddAndRemoveButtons();      // �r���h�ǉ��{�^���ƍ폜�{�^����\��
    }


    // ��؂��������
    private void DrawSeparate(float height)
    {
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
    }
    // ���̓t�B�[���h��\��
    private void DrawPathField()
    {
        // �t�@�C���p�X�̓��̓t�B�[���h��\��
        folderPath = EditorGUILayout.TextField("�V�[���f�B���N�g��", folderPath);
    }
    // �S�\���`�F�b�N�{�b�N�X��\��
    private void DrawShowAllScenesCheckbox()
    {
        // �`�F�b�N�{�b�N�X�̏�Ԃ��X�V
        showAllFlag = EditorGUILayout.Toggle("���ׂẴV�[����\��", showAllFlag);
    }
    // �V�[���t�@�C�����������ăJ�e�S������
    private void LoadSceneFile()
    {
        // �V�[���t�@�C����ۑ����鎫����������
        categories.Clear();

        // �w�肳�ꂽ�p�X�����݂��邩�m�F
        if (!System.IO.Directory.Exists(folderPath))
        {
            // �G���[���O���o�͂��ď����𒆒f
            Debug.LogError("�w�肳�ꂽ�p�X�����݂��܂���B");
            return;
        }

        // �����I�v�V������ݒ�
        System.IO.SearchOption searchOption =
            showAllFlag ?
            System.IO.SearchOption.AllDirectories :
            System.IO.SearchOption.TopDirectoryOnly;

        // �w�肳�ꂽ�t�H���_���̃p�X���擾
        string[] scenePaths = System.IO.Directory.GetFiles(folderPath, "*.unity", searchOption);

        foreach (string scenePath in scenePaths)
        {
            // �t�H���_���J�e�S���Ƃ��Ď擾
            string sceneFolder = System.IO.Path.GetDirectoryName(scenePath);

            // �J�e�S���������ɒǉ��܂��͍X�V
            if (!categories.ContainsKey(sceneFolder))
            {
                categories.Add(sceneFolder, new List<string>());
            }
            // �J�e�S���ɒǉ�
            categories[sceneFolder].Add(System.IO.Path.GetFileNameWithoutExtension(scenePath));
        }
    }
    // �J�e�S�������Ɋ�Â��ăV�[���ꗗ��\��
    private void DrawCategoriesAndScenes()
    {
        // �V�[���t�@�C�����ǂݍ��܂�Ă��邩�m�F
        if (categories == null || categories.Count == 0)
        {
            EditorGUILayout.LabelField("�V�[����������܂���B");
            return;
        }

        // �J�e�S�����Ƃɏ���
        foreach (var category in categories)
        {
            // ��������؂�
            DrawSeparate(1.0f);
            // �J�e�S���̌��o����\��
            EditorGUILayout.LabelField(category.Key);
            // �J�e�S�����̃V�[����\��
            foreach (string sceneName in category.Value)
            {
                if (GUILayout.Button(sceneName))
                {
                    try
                    {
                        // �{�^���������ꂽ�V�[�����J��
                        string scenePath = System.IO.Path.Combine(category.Key, sceneName + EXTENSION);
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

    // �V�[�����J��
    private void OpenScene(string scenePath)
    {
        // �ύX������Εۑ����邩�m�F
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // �V�[���t�@�C�������݂��邩�m�F
            if (System.IO.File.Exists(scenePath))
            {
                // �V�[�����J��
                EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                // �G���[���O���o��
                Debug.LogError($"�w�肳�ꂽ�V�[�������݂��܂���: {scenePath}");
            }
        }
    }






    ///////////////////////////////////////////////////////////////
    // �V�[���̑��݊m�F�����A�r���h�Z�b�e�B���O�̕ύX������
    ///////////////////////////////////////////////////////////////

    // �r���h�Z�b�e�B���O���̃V�[�����`�F�b�N����{�^��
    private void DrawCheckBuildScenesButton()
    {
        if (GUILayout.Button("�r���h�Z�b�e�B���O���̃V�[�����`�F�b�N"))
        {
            CheckBuildScenes();
        }
    }

    // �r���h�Z�b�e�B���O���̃V�[�������݂��邩�`�F�b�N
    private void CheckBuildScenes()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        List<string> missingScenes = new List<string>();

        // �e�r���h�V�[�������݂��邩�m�F
        foreach (var scene in buildScenes)
        {
            if (!File.Exists(scene.path))
            {
                missingScenes.Add(scene.path);
            }
        }

        if (missingScenes.Count > 0)
        {
            // ���݂��Ȃ��V�[��������Ίm�F�_�C�A���O��\��
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

    // ���݂��Ȃ��V�[���̑Ή����m�F����_�C�A���O
    private void HandleMissingScene(string missingScenePath)
    {
        bool shouldRemove = EditorUtility.DisplayDialog(
            "���݂��Ȃ��V�[�������o",
            $"�V�[�� '{missingScenePath}' �����݂��܂���B�r���h�Z�b�e�B���O���珜�O���܂����H",
            "�͂�",
            "������");

        if (shouldRemove)
        {
            // �u�͂��v�Ȃ�r���h�Z�b�e�B���O����V�[�������O
            RemoveSceneFromBuild(missingScenePath);
            Debug.Log($"�V�[�� '{missingScenePath}' ���r���h�Z�b�e�B���O���珜�O���܂����B");
        }
        else
        {
            // �u�������v�Ȃ�V�����V�[���̃p�X����͂�����
            string newScenePath = EditorUtility.OpenFilePanel("�V�����V�[���̑I��", "Assets/Scenes", "unity");
            if (!string.IsNullOrEmpty(newScenePath))
            {
                // �V�����V�[���p�X���r���h�ɒǉ�
                AddSceneToBuild(newScenePath);
                Debug.Log($"�V�����V�[�� '{newScenePath}' ���r���h�Z�b�e�B���O�ɒǉ����܂����B");
            }
            else
            {
                Debug.LogWarning("�V�����V�[���̃p�X�����͂���܂���ł����B");
            }
        }
    }








    ///////////////////////////////////////////////////////////////
    // �V�[���̍쐬�A��������
    ///////////////////////////////////////////////////////////////

    // �V�[���V�K�쐬
    private void DrawCreateSceneButton()
    {
        if (GUILayout.Button("�V�����V�[�����쐬"))
        {
            CreateAndDuplicateScene();
        }

        // �t�@�C���p�X�̓��̓t�B�[���h��\��
        newScenePath = EditorGUILayout.TextField("�V�[���ۑ���", newScenePath);
        newSceneName = EditorGUILayout.TextField("�V�[����", newSceneName);
        copyStageName = EditorGUILayout.TextField("�R�s�[���V�[����", copyStageName);
    }
    // �V�����V�[�����쐬���ĕ���
    private void CreateAndDuplicateScene()
    {
        // �t�@�C���p�X����ł���Ή������Ȃ�
        if (string.IsNullOrEmpty(newScenePath))
        {
            Debug.LogError("�t�@�C���p�X����͂��Ă��������B");
            return;
        }

        // �V�[�����ɘA�Ԃ�ǉ����鏈��
        string sceneNameWithNumber = GetUniqueSceneName(newScenePath, newSceneName);

        // �V�[����V�K�쐬
        EditorSceneManager.SaveScene(
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene),
            newScenePath + sceneNameWithNumber + EXTENSION);

        // �쐬�����V�[���Ƀf�t�H���g�̃V�[�����e���R�s�[
        CopyDefaultSceneContents(newScenePath + sceneNameWithNumber + EXTENSION);
    }
    // �V�[�����ɘA�Ԃ�t�^���Ĉ�ӂ̖��O�𐶐�
    private string GetUniqueSceneName(string path, string baseName)
    {
        string uniqueName = baseName;
        int counter = 1;

        // �A�ԕt���ŃV�[�������݂��Ȃ����m�F
        while (File.Exists(path + uniqueName + EXTENSION))
        {
            uniqueName = baseName + "(" + counter + ")";
            counter++;
        }

        return uniqueName;
    }
    // �f�t�H���g�̃V�[���̓��e���R�s�[����
    private void CopyDefaultSceneContents(string path)
    {
        // �f�t�H���g�̃V�[�������݂��邩�m�F
        if (!File.Exists(copyStageName))
        {
            Debug.LogError("�f�t�H���g�̃V�[����������܂���B");
            return;
        }

        // �f�t�H���g�̃V�[����ǂݍ���
        var defaultScene = EditorSceneManager.OpenScene(copyStageName, OpenSceneMode.Single);

        // �쐬�����V�[���ɃR�s�[����
        EditorSceneManager.SaveScene(defaultScene, path);
    }








    ///////////////////////////////////////////////////////////////
    // �r���h�Z�b�e�B���O��ҏW����
    ///////////////////////////////////////////////////////////////
    // �ǉ��{�^���ƍ폜�{�^����\��
    private void DrawAddAndRemoveButtons()
    {
        GUILayout.BeginHorizontal();

        // �ǉ��{�^��
        if (GUILayout.Button("�r���h�ɒǉ�"))
        {
            AddSceneToBuild(EditorSceneManager.GetActiveScene().path);
        }

        // �폜�{�^��
        if (GUILayout.Button("�r���h����폜"))
        {
            RemoveSceneFromBuild(EditorSceneManager.GetActiveScene().path);
        }

        GUILayout.EndHorizontal();
    }

    // �r���h�Z�b�e�B���O�Ƀr���h����V�[����ǉ�����
    private void AddSceneToBuild(string scenePath)
    {
        // ���݂̃r���h�ݒ���擾
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        // �V�[�������łɒǉ�����Ă��邩�m�F
        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            if (buildScene.path == scenePath)
            {
                Debug.LogWarning("���̃V�[���͊��Ƀr���h�ݒ�ɒǉ�����Ă��܂��B");
                return;
            }
        }

        // �V�����r���h�ݒ�ɒǉ�
        ArrayUtility.Add(ref buildScenes, new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = buildScenes;
    }

    // �r���h�Z�b�e�B���O����V�[�����폜����
    private void RemoveSceneFromBuild(string scenePath)
    {
        // ���݂̃r���h�ݒ���擾
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        // �V�[�����r���h�ݒ�Ɋ܂܂�Ă��邩�m�F
        for (int i = 0; i < buildScenes.Length; i++)
        {
            if (buildScenes[i].path == scenePath)
            {
                // �V�[�����폜
                ArrayUtility.RemoveAt(ref buildScenes, i);
                EditorBuildSettings.scenes = buildScenes;
                return;
            }
        }

        Debug.LogWarning("���̃V�[���̓r���h�ݒ�Ɋ܂܂�Ă��܂���B");
    }

    // �r���h�Z�b�e�B���O�ɐݒ肳��Ă���V�[���ꗗ��\��
    private void DrawBuildScene()
    {
        EditorGUILayout.LabelField("�r���h�Z�b�e�B���O�ɐݒ肳��Ă���V�[��:");

        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            EditorGUILayout.LabelField(buildScene.path);
        }
    }
}