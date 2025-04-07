using UnityEngine;
using UnityEditor;

public class PrefabBoxColliderAndRigidBody : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // ����ȭ�� ������ �迭
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

        // Prefab �迭 �Է�
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        // BoxCollider ����
        GUILayout.Label("BoxCollider Settings", EditorStyles.boldLabel);
        colliderCenter = EditorGUILayout.Vector3Field("Collider Center", colliderCenter);
        colliderSize = EditorGUILayout.Vector3Field("Collider Size", colliderSize);

        // ���� ��ư
        if (GUILayout.Button("Add Components to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // ������ ����ȭ
            AddComponentsToPrefabs();
        }

        serializedObjectReference.ApplyModifiedProperties(); // ������ ���� ����ȭ
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

            // BoxCollider �߰�
            BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = prefab.AddComponent<BoxCollider>();
            }
            boxCollider.center = colliderCenter;
            boxCollider.size = colliderSize;

            // Rigidbody �߰� �� IsKinematic Ȱ��ȭ
            Rigidbody rb = prefab.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = prefab.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;

            updatedCount++;
        }

        Debug.Log($"�� {updatedCount}���� �����տ� BoxCollider�� Rigidbody�� �߰��Ǿ����ϴ�.");
    }
}
