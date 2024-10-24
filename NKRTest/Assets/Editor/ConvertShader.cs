/**************************************************
* File:           ConvertShader.cs
*
* Description:    �V�F�[�_�[�O���t���V�F�[�_�[���{�ɕϊ�����
*
* Update:         2024 / 10 / 23
*
* Author:         Ryo Nakamura
***************************************************/


using UnityEditor;
using UnityEngine;
using UnityEditor.Graphs;


public class ConvertShader : EditorWindow
{
    [MenuItem("NKR Editor/ConvertShader")]
    public static void ShowWindow()
    {
        GetWindow<ConvertShader>("ConvertShader");
    }

    private Object shaderFile;

    private void OnGUI()
    {
        GUILayout.Label("�V�F�[�_�[�������Ƀh���b�O�A���h�h���b�v", EditorStyles.boldLabel);
        shaderFile = EditorGUILayout.ObjectField("Shader File", shaderFile, typeof(Object), false);

        if (shaderFile != null)
        {
            // �ϊ������t�@�C����V�������������i�㏑���ۑ��͂��Ȃ��j
            if (GUILayout.Button("�ϊ�����"))
            {

            }
        }
    }

    private void ShaderGraphToLabo(Object graph)
    {

    }

}
