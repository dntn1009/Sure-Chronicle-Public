using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


public class SpriteSortingLayerChange : EditorWindow
{
    private GameObject[] targetObjects = new GameObject[0]; // 여러 오브젝트를 담을 배열
    private string sortingLayerName = "Default"; // 기본 Sorting Layer
    private int sortingOrder = 0; // 기본 Sorting Order

    [MenuItem("Tools/Batch Sorting Group Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSortingLayerChange>("SpriteSortingLayerChange Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Sorting Group Settings", EditorStyles.boldLabel);

        // 배열 크기 설정
        int arraySize = Mathf.Max(0, EditorGUILayout.IntField("Number of Objects", targetObjects.Length));
        if (arraySize != targetObjects.Length)
        {
            // 배열 크기 조정
            GameObject[] newTargetObjects = new GameObject[arraySize];
            for (int i = 0; i < Mathf.Min(arraySize, targetObjects.Length); i++)
            {
                newTargetObjects[i] = targetObjects[i];
            }
            targetObjects = newTargetObjects;
        }

        // 오브젝트 배열 입력
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i] = (GameObject)EditorGUILayout.ObjectField($"Target Object {i + 1}", targetObjects[i], typeof(GameObject), true);
        }

        // Sorting Layer 및 Order 설정
        sortingLayerName = EditorGUILayout.TextField("Sorting Layer", sortingLayerName);
        sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

        // Apply 버튼
        if (GUILayout.Button("Apply to All"))
        {
            ApplySortingGroupSettings();
        }
    }

    private void ApplySortingGroupSettings()
    {
        if (targetObjects == null || targetObjects.Length == 0)
        {
            Debug.LogWarning("적용할 오브젝트가 없습니다.");
            return;
        }

        int updatedCount = 0;

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            // SortingGroup 컴포넌트 가져오기 또는 추가
            SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = obj.AddComponent<SortingGroup>();
            }

            // 변경 사항 적용
            Undo.RecordObject(sortingGroup, "Change SortingGroup Settings");
            sortingGroup.sortingLayerName = sortingLayerName;
            sortingGroup.sortingOrder = sortingOrder;
            EditorUtility.SetDirty(sortingGroup);

            updatedCount++;
        }

        Debug.Log($"총 {updatedCount}개의 오브젝트에 SortingGroup 설정이 적용되었습니다.");
    }
}
