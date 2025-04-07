using UnityEditor;
using UnityEngine;

public class PrefabMissingReferencesChecker : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // �˻��� ������ �迭
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    [MenuItem("Tools/Prefab Missing References Checker")]
    public static void ShowWindow()
    {
        GetWindow<PrefabMissingReferencesChecker>("Prefab Missing References Checker");
    }

    private void OnEnable()
    {
        // SerializedObject �ʱ�ȭ
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        // UI ����
        serializedObjectReference.Update();

        GUILayout.Label("Prefab Missing References Checker", EditorStyles.boldLabel);

        // Prefab �迭 �Է� �ʵ�
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        GUILayout.Space(10);

        // ���� ��ư
        if (GUILayout.Button("Check Missing References in Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // ������ ����ȭ
            CheckMissingReferencesInPrefabs();
        }

        serializedObjectReference.ApplyModifiedProperties(); // ������ ���� ����ȭ
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

            // Prefab ��� ��������
            string path = AssetDatabase.GetAssetPath(prefab);

            // Prefab�� �ε�
            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(path);

            Debug.Log($"Checking Prefab: {prefab.name}");

            // Missing Reference �˻�
            CheckMissingReferencesInGameObject(loadedPrefab);

            // Prefab ���� �� ��ε�
            PrefabUtility.UnloadPrefabContents(loadedPrefab);
        }

        Debug.Log("Missing References �˻� �Ϸ�!");
    }

    private void CheckMissingReferencesInGameObject(GameObject obj)
    {
        // ���� GameObject�� ������Ʈ �˻�
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null)
            {
                Debug.LogError($"[Missing Component] Missing in GameObject: {obj.name}", obj);
                continue;
            }

            // SerializedObject�� ����Ͽ� �ʵ��� Missing Reference �˻�
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

        // ���� ������Ʈ �˻� (��� ȣ��)
        foreach (Transform child in obj.transform)
        {
            CheckMissingReferencesInGameObject(child.gameObject);
        }
    }
}