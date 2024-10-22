using UnityEditor;
using UnityEngine;

public static class MakeGroupedObject
{
    [MenuItem("MyEditor/Make GroupedObject %g")]
    public static void GroupSelected()
    {
        // 選択されているオブジェクトを取得
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            // 新しいGameObjectを作成してグループ化
            GameObject group = new GameObject("New Grouped Object");

            // 選択されたオブジェクトの平均座標を計算
            Vector3 averagePosition = Vector3.zero;
            foreach (GameObject obj in selectedObjects)
            {
                averagePosition += obj.transform.position;
            }
            averagePosition /= selectedObjects.Length;

            // グループの位置を平均座標に設定
            group.transform.position = averagePosition;

            // 選択されたオブジェクトをグループの子オブジェクトにする
            foreach (GameObject obj in selectedObjects)
            {
                obj.transform.SetParent(group.transform);
            }
        }
    }
}
