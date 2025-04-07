using UnityEngine;
using UnityEditor;

public class PrefabAttackObjSetUp : EditorWindow
{
    [SerializeField]
    private GameObject[] prefabObjects; // ���� Prefab�� ���� �迭

    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    private string childObjectTag = "FindTarget"; // �ڽ� ������Ʈ�� �±�
    private Vector3 sphereColliderCenter = Vector3.zero; // Sphere Collider�� �߽�
    private float sphereColliderRadius = 1.0f; // Sphere Collider�� ������

    [MenuItem("Tools/Prefab Attack Object Setup")]
    public static void ShowWindow()
    {
        GetWindow<PrefabAttackObjSetUp>("Prefab Attack Object Setup");
    }

    private void OnEnable()
    {
        // SerializedObject �ʱ�ȭ
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        // SerializedObject ������Ʈ
        serializedObjectReference.Update();

        GUILayout.Label("Prefab Attack Object Setup", EditorStyles.boldLabel);

        // Prefab �迭 �Է�
        EditorGUILayout.PropertyField(prefabArrayProperty, new GUIContent("Prefab Objects"), true);

        // �ڽ� ������Ʈ �±� ����
        childObjectTag = EditorGUILayout.TextField("Child Object Tag", childObjectTag);

        // Sphere Collider ����
        GUILayout.Label("Sphere Collider Settings", EditorStyles.boldLabel);
        sphereColliderCenter = EditorGUILayout.Vector3Field("Collider Center", sphereColliderCenter);
        sphereColliderRadius = EditorGUILayout.FloatField("Collider Radius", sphereColliderRadius);

        // ���� ��ư
        if (GUILayout.Button("Add Components to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // ������ ����ȭ
            AddComponentsToPrefabs();
        }

        // SerializedObject ���� ������Ʈ
        serializedObjectReference.ApplyModifiedProperties();
    }

    private void AddComponentsToPrefabs()
    {
        if (prefabObjects == null || prefabObjects.Length == 0)
        {
            Debug.LogWarning("Prefab array is empty. Please assign prefabs.");
            return;
        }

        foreach (GameObject prefab in prefabObjects)
        {
            if (prefab == null) continue;

            // Prefab �ν��Ͻ� ����
            GameObject prefabInstance = Instantiate(prefab);
            prefabInstance.name = prefab.name + "_Instance";

            // �ڽ� ������Ʈ ���� �� ����
            GameObject childObject = new GameObject("FindTarget");
            childObject.transform.SetParent(prefabInstance.transform);
            childObject.transform.localPosition = Vector3.zero;
            childObject.tag = childObjectTag;

            // Sphere Collider �߰� �� ����
            SphereCollider sphereCollider = childObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = sphereColliderCenter;
            sphereCollider.radius = sphereColliderRadius;

            // AttackAreUnitFind ��ũ��Ʈ �߰�
            childObject.AddComponent<AttackAreUnitFind>();

            // Prefab ����
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(prefabInstance, prefabPath, InteractionMode.UserAction);

            // Prefab �ν��Ͻ� ����
            DestroyImmediate(prefabInstance);

            Debug.Log($"Prefab '{prefab.name}'�� �ڽ� ������Ʈ�� ���������� �߰��߽��ϴ�.");
        }
    }
}