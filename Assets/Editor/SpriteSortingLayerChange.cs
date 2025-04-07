using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


public class SpriteSortingLayerChange : EditorWindow
{
    private GameObject[] targetObjects = new GameObject[0]; // ���� ������Ʈ�� ���� �迭
    private string sortingLayerName = "Default"; // �⺻ Sorting Layer
    private int sortingOrder = 0; // �⺻ Sorting Order

    [MenuItem("Tools/Batch Sorting Group Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSortingLayerChange>("SpriteSortingLayerChange Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Sorting Group Settings", EditorStyles.boldLabel);

        // �迭 ũ�� ����
        int arraySize = Mathf.Max(0, EditorGUILayout.IntField("Number of Objects", targetObjects.Length));
        if (arraySize != targetObjects.Length)
        {
            // �迭 ũ�� ����
            GameObject[] newTargetObjects = new GameObject[arraySize];
            for (int i = 0; i < Mathf.Min(arraySize, targetObjects.Length); i++)
            {
                newTargetObjects[i] = targetObjects[i];
            }
            targetObjects = newTargetObjects;
        }

        // ������Ʈ �迭 �Է�
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i] = (GameObject)EditorGUILayout.ObjectField($"Target Object {i + 1}", targetObjects[i], typeof(GameObject), true);
        }

        // Sorting Layer �� Order ����
        sortingLayerName = EditorGUILayout.TextField("Sorting Layer", sortingLayerName);
        sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

        // Apply ��ư
        if (GUILayout.Button("Apply to All"))
        {
            ApplySortingGroupSettings();
        }
    }

    private void ApplySortingGroupSettings()
    {
        if (targetObjects == null || targetObjects.Length == 0)
        {
            Debug.LogWarning("������ ������Ʈ�� �����ϴ�.");
            return;
        }

        int updatedCount = 0;

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            // SortingGroup ������Ʈ �������� �Ǵ� �߰�
            SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = obj.AddComponent<SortingGroup>();
            }

            // ���� ���� ����
            Undo.RecordObject(sortingGroup, "Change SortingGroup Settings");
            sortingGroup.sortingLayerName = sortingLayerName;
            sortingGroup.sortingOrder = sortingOrder;
            EditorUtility.SetDirty(sortingGroup);

            updatedCount++;
        }

        Debug.Log($"�� {updatedCount}���� ������Ʈ�� SortingGroup ������ ����Ǿ����ϴ�.");
    }
}
