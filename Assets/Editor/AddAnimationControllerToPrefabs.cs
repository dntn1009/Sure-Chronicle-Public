using UnityEngine;
using UnityEditor;

public class AddAnimationControllerToPrefabs : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // 프리팹 배열 (사용자가 등록)
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    [MenuItem("Tools/Prefab AnimationController Adder")]
    public static void ShowWindow()
    {
        GetWindow<AddAnimationControllerToPrefabs>("Prefab AnimationController Adder");
    }

    private void OnEnable()
    {
        // SerializedObject 초기화
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        // UI 갱신
        serializedObjectReference.Update();

        GUILayout.Label("Prefab AnimationController Adder", EditorStyles.boldLabel);

        // Prefab 배열 입력 필드
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        GUILayout.Space(10);

        // 실행 버튼
        if (GUILayout.Button("Add AnimationController to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // 데이터 동기화
            PrefabAnimationControllerAdder();
        }

        serializedObjectReference.ApplyModifiedProperties(); // 데이터 최종 동기화
    }

    private void PrefabAnimationControllerAdder()
    {
        if (prefabObjects == null || prefabObjects.Length == 0)
        {
            Debug.LogWarning("Prefab array is empty. Please assign prefabs.");
            return;
        }

        int updatedCount = 0;

        foreach (GameObject prefab in prefabObjects)
        {
            if (prefab == null) continue;

            // Prefab 경로 가져오기
            string path = AssetDatabase.GetAssetPath(prefab);

            // Prefab을 로드
            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(path);

            // UnitRoot 탐색 (재귀적으로 찾기)
            Transform unitRoot = FindChildRecursive(loadedPrefab.transform, "UnitRoot");
            if (unitRoot != null)
            {
                // AnimationController 스크립트 추가
                if (unitRoot.GetComponent<AnimationController>() == null)
                {
                    unitRoot.gameObject.AddComponent<AnimationController>();
                    Debug.Log($"AnimationController added to UnitRoot in prefab: {prefab.name}");
                }
                else
                {
                    Debug.Log($"AnimationController already exists in UnitRoot of prefab: {prefab.name}");
                }
            }
            else
            {
                Debug.LogWarning($"UnitRoot not found in prefab: {prefab.name}");
            }

            // Prefab 저장 및 언로드
            PrefabUtility.SaveAsPrefabAsset(loadedPrefab, path);
            PrefabUtility.UnloadPrefabContents(loadedPrefab);

            updatedCount++;
        }

        Debug.Log($"총 {updatedCount}개의 프리팹에 AnimationController가 추가되었습니다.");
    }

    // 하위 계층구조에서 재귀적으로 오브젝트를 찾는 메서드
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform found = FindChildRecursive(child, childName); // 자식의 자식 탐색
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}