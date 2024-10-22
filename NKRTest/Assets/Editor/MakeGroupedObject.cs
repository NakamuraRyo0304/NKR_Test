using UnityEditor;
using UnityEngine;

public static class MakeGroupedObject
{
    [MenuItem("NKR Editor/Make GroupedObject %g")]
    public static void GroupSelected() // グループにまとめる
    {
        // 選択されているオブジェクトを取得
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            // Undoの登録
            Undo.SetCurrentGroupName("Group Objects");
            int groupIndex = Undo.GetCurrentGroup();

            // 新しいGameObjectを作成してグループ化
            GameObject group = new GameObject($"{selectedObjects[0].name}'s Group");

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
                // Undoの記録
                Undo.SetTransformParent(obj.transform, group.transform, "Parent to Group");
                obj.transform.SetParent(group.transform);
            }

            // Undoグループを終了
            Undo.CollapseUndoOperations(groupIndex);
        }
    }
}
