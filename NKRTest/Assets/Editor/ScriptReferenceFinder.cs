using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptReferenceFinder : EditorWindow
{
    // �����ΏۂƂȂ�X�N���v�g
    private MonoScript targetScript;

    // �Q�ƌ��ʂ��i�[���郊�X�g�i�I�u�W�F�N�g�j
    private List<string> sceneObjectReferences = new List<string>();
    private List<string> prefabObjectReferences = new List<string>();

    // �G�f�B�^�E�B���h�E��\�����邽�߂̃��j���[���ڂ�ǉ�
    [MenuItem("NKR Editor/ScriptReferenceFinder")]
    public static void ShowWindow()
    {
        // �G�f�B�^�E�B���h�E���쐬���A�^�C�g����ݒ�
        GetWindow<ScriptReferenceFinder>("ScriptReferenceFinder");
    }

    // �E�B���h�E��GUI��`�悷��
    void OnGUI()
    {
        // �������x����\��
        EditorGUILayout.LabelField("�����ɃX�N���v�g���h���b�O���h���b�v���Ă�������");

        // �h���b�O���h���b�v�p�̃G���A��ݒ�
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "�h���b�v�G���A");

        // �h���b�O�C�x���g�̏���
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // �}�E�X�ʒu���h���b�v�G���A���ɂ��邩�m�F
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                // �h���b�O���̃r�W���A���t�B�[�h�o�b�N
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                // �h���b�v�����������Ƃ��̏���
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    // �h���b�v���ꂽ�I�u�W�F�N�g�̏���
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is MonoScript)
                        {
                            // �X�N���v�g���^�[�Q�b�g�ɐݒ肵�A�Q�Ƃ�����
                            targetScript = draggedObject as MonoScript;
                            FindReferences();
                        }
                    }
                }
                break;
        }

        // �X�N���v�g���ݒ肳��Ă���ꍇ�A�������ʂ�\��
        if (targetScript != null)
        {
            EditorGUILayout.LabelField("��������:", EditorStyles.boldLabel);


            // �v���n�u��\�����Ă���V�[���I�u�W�F�N�g��\������
            foreach (string reference in prefabObjectReferences)
                EditorGUILayout.LabelField(reference);
            foreach (string reference in sceneObjectReferences)
                EditorGUILayout.LabelField(reference);
        }
    }

    // �X�N���v�g�̎Q�Ƃ���������
    private void FindReferences()
    {
        // �����̌������ʂ��N���A
        sceneObjectReferences.Clear();
        prefabObjectReferences.Clear();

        // �V�[������GameObject���������A�K�w���܂߂ĕ\��
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            // �I�u�W�F�N�g�ɃA�^�b�`����Ă���R���|�[�l���g���擾
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                // �R���|�[�l���g���^�[�Q�b�g�X�N���v�g�̃N���X�ƈ�v����ꍇ
                if (component != null && component.GetType() == targetScript.GetClass())
                {
                    string fullPath = GetGameObjectPath(obj);
                    string sceneName = obj.scene.name;

                    if (sceneName != null)// �V�[����������ꍇ�̓I�u�W�F�N�g
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

    // �I�u�W�F�N�g�̃t���p�X�i�e�I�u�W�F�N�g/�q�I�u�W�F�N�g�j���擾����
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        // �e�I�u�W�F�N�g�����݂���ꍇ�́A���̐e�܂ł����̂ڂ��ăp�X���쐬
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
