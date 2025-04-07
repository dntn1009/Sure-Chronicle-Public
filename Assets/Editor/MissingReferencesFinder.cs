using UnityEditor;
using UnityEngine;

public class PrefabMissingReferencesChecker : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // 검사할 프리팹 배열
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    [MenuItem("Tools/Prefab Missing References Checker")]
    public static void ShowWindow()
    {
        GetWindow<PrefabMissingReferencesChecker>("Prefab Missing References Checker");
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

        GUILayout.Label("Prefab Missing References Checker", EditorStyles.boldLabel);

        // Prefab 배열 입력 필드
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        GUILayout.Space(10);

        // 실행 버튼
        if (GUILayout.Button("Check Missing References in Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // 데이터 동기화
            CheckMissingReferencesInPrefabs();
        }

        serializedObjectReference.ApplyModifiedProperties(); // 데이터 최종 동기화
    }

    private void CheckMissingReferencesInPrefabs()
    {
        if (prefabObjects == null || prefabObjects.Length == 0)
        {
            Debug.LogWarning("Prefab array is empty. Please assign prefabs.");
            return;
        }

        foreach (GameObject prefab in prefabObjects)
        {
            if (prefab == null) continue;

            // Prefab 경로 가져오기
            string path = AssetDatabase.GetAssetPath(prefab);

            // Prefab을 로드
            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(path);

            Debug.Log($"Checking Prefab: {prefab.name}");

            // Missing Reference 검사
            CheckMissingReferencesInGameObject(loadedPrefab);

            // Prefab 저장 및 언로드
            PrefabUtility.UnloadPrefabContents(loadedPrefab);
        }

        Debug.Log("Missing References 검사 완료!");
    }

    private void CheckMissingReferencesInGameObject(GameObject obj)
    {
        // 현재 GameObject의 컴포넌트 검사
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null)
            {
                Debug.LogError($"[Missing Component] Missing in GameObject: {obj.name}", obj);
                continue;
            }

            // SerializedObject를 사용하여 필드의 Missing Reference 검사
            SerializedObject so = new SerializedObject(component);
            SerializedProperty prop = so.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                {
                    Debug.LogError($"[Missing Reference] Found in Component: {component.GetType().Name} on GameObject: {obj.name}", obj);
                }
            }
        }

        // 하위 오브젝트 검사 (재귀 호출)
        foreach (Transform child in obj.transform)
        {
            CheckMissingReferencesInGameObject(child.gameObject);
        }
    }
}