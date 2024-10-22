using UnityEditor;
using UnityEngine;

public static class MakeGroupedObject
{
    [MenuItem("MyEditor/Make GroupedObject %g")]
    public static void GroupSelected()
    {
        // �I������Ă���I�u�W�F�N�g���擾
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length > 0)
        {
            // �V����GameObject���쐬���ăO���[�v��
            GameObject group = new GameObject("New Grouped Object");

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
                obj.transform.SetParent(group.transform);
            }
        }
    }
}
