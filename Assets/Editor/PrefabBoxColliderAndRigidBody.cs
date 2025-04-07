using UnityEngine;
using UnityEditor;

public class PrefabBoxColliderAndRigidBody : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // 직렬화된 프리팹 배열
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    private Vector3 colliderCenter = Vector3.zero; // BoxCollider Center
    private Vector3 colliderSize = Vector3.one;   // BoxCollider Size

    [MenuItem("Tools/Prefab Array Collider & Rigidbody (Serialized)")]
    public static void ShowWindow()
    {
        GetWindow<PrefabBoxColliderAndRigidBody>("Prefab Array Editor (Serialized)");
    }

    private void OnEnable()
    {
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        serializedObjectReference.Update();

        GUILayout.Label("Prefab Array Editor (Serialized)", EditorStyles.boldLabel);

        // Prefab 배열 입력
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        // BoxCollider 설정
        GUILayout.Label("BoxCollider Settings", EditorStyles.boldLabel);
        colliderCenter = EditorGUILayout.Vector3Field("Collider Center", colliderCenter);
        colliderSize = EditorGUILayout.Vector3Field("Collider Size", colliderSize);

        // 실행 버튼
        if (GUILayout.Button("Add Components to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // 데이터 동기화
            AddComponentsToPrefabs();
        }

        serializedObjectReference.ApplyModifiedProperties(); // 데이터 최종 동기화
    }

    private void AddComponentsToPrefabs()
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

            // BoxCollider 추가
            BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = prefab.AddComponent<BoxCollider>();
            }
            boxCollider.center = colliderCenter;
            boxCollider.size = colliderSize;

            // Rigidbody 추가 및 IsKinematic 활성화
            Rigidbody rb = prefab.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = prefab.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;

            updatedCount++;
        }

        Debug.Log($"총 {updatedCount}개의 프리팹에 BoxCollider와 Rigidbody가 추가되었습니다.");
    }
}
