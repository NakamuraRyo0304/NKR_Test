using UnityEditor;
using UnityEngine;

public static class MakeGroupedObject
{
    [MenuItem("NKR Editor/Make GroupedObject %g")]
    public static void GroupSelected() // �O���[�v�ɂ܂Ƃ߂�
    {
        // �I������Ă���I�u�W�F�N�g���擾
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            // Undo�̓o�^
            Undo.SetCurrentGroupName("Group Objects");
            int groupIndex = Undo.GetCurrentGroup();

            // �V����GameObject���쐬���ăO���[�v��
            GameObject group = new GameObject($"{selectedObjects[0].name}'s Group");

            // �I�����ꂽ�I�u�W�F�N�g�̕��ύ��W���v�Z
            Vector3 averagePosition = Vector3.zero;
            foreach (GameObject obj in selectedObjects)
            {
                averagePosition += obj.transform.position;
            }
            averagePosition /= selectedObjects.Length;

            // �O���[�v�̈ʒu�𕽋ύ��W�ɐݒ�
            group.transform.position = averagePosition;

            // �I�����ꂽ�I�u�W�F�N�g���O���[�v�̎q�I�u�W�F�N�g�ɂ���
            foreach (GameObject obj in selectedObjects)
            {
                // Undo�̋L�^
                Undo.SetTransformParent(obj.transform, group.transform, "Parent to Group");
                obj.transform.SetParent(group.transform);
            }

            // Undo�O���[�v���I��
            Undo.CollapseUndoOperations(groupIndex);
        }
    }
}
