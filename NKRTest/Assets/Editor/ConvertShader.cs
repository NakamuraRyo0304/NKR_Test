/**************************************************
* File:           ConvertShader.cs
*
* Description:    シェーダーグラフをシェーダーラボに変換する
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
        GUILayout.Label("シェーダーをここにドラッグアンドドロップ", EditorStyles.boldLabel);
        shaderFile = EditorGUILayout.ObjectField("Shader File", shaderFile, typeof(Object), false);

        if (shaderFile != null)
        {
            // 変換したファイルを新しく書きだす（上書き保存はしない）
            if (GUILayout.Button("変換する"))
            {

            }
        }
    }

    private void ShaderGraphToLabo(Object graph)
    {

    }

}
