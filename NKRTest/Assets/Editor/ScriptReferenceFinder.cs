using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptReferenceFinder : EditorWindow
{
    private MonoScript targetScript;
    private List<string> references = new List<string>();

    [MenuItem("MyEditor/ScriptReferenceFinder")]
    public static void ShowWindow()
    {
        GetWindow<ScriptReferenceFinder>("Script Reference Finder");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("�����ɃX�N���v�g���h���b�O���h���b�v���Ă�������");

        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

        GUI.Box(dropArea, "�h���b�v�G���A");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is MonoScript)
                        {
                            targetScript = draggedObject as MonoScript;
                            FindReferences();
                        }
                    }
                }
                break;
        }

        if (targetScript != null)
        {
            EditorGUILayout.LabelField("��������:", EditorStyles.boldLabel);
            foreach (string reference in references)
            {
                EditorGUILayout.LabelField(reference);
            }
        }
    }

    void FindReferences()
    {
        references.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Scene t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object[] deps = EditorUtility.CollectDependencies(new Object[] { AssetDatabase.LoadAssetAtPath(path, typeof(Object)) });

            foreach (Object dep in deps)
            {
                if (dep == targetScript)
                {
                    if (path.EndsWith(".unity"))
                    {
                        references.Add("�V�[��: " + System.IO.Path.GetFileNameWithoutExtension(path));
                    }
                    else if (path.EndsWith(".prefab"))
                    {
                        references.Add("�v���n�u: " + System.IO.Path.GetFileNameWithoutExtension(path));
                    }
                    break;
                }
            }
        }

        // �V�[������GameObject������
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != null && component.GetType() == targetScript.GetClass())
                {
                    references.Add("�I�u�W�F�N�g: " + obj.name);
                    break;
                }
            }
        }
    }
}
