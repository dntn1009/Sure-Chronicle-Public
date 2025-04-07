using UnityEngine;
using UnityEditor;

public class AddAnimationControllerToPrefabs : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // ������ �迭 (����ڰ� ���)
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    [MenuItem("Tools/Prefab AnimationController Adder")]
    public static void ShowWindow()
    {
        GetWindow<AddAnimationControllerToPrefabs>("Prefab AnimationController Adder");
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

        GUILayout.Label("Prefab AnimationController Adder", EditorStyles.boldLabel);

        // Prefab �迭 �Է� �ʵ�
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        GUILayout.Space(10);

        // ���� ��ư
        if (GUILayout.Button("Add AnimationController to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // ������ ����ȭ
            PrefabAnimationControllerAdder();
        }

        serializedObjectReference.ApplyModifiedProperties(); // ������ ���� ����ȭ
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

            // Prefab ��� ��������
            string path = AssetDatabase.GetAssetPath(prefab);

            // Prefab�� �ε�
            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(path);

            // UnitRoot Ž�� (��������� ã��)
            Transform unitRoot = FindChildRecursive(loadedPrefab.transform, "UnitRoot");
            if (unitRoot != null)
            {
                // AnimationController ��ũ��Ʈ �߰�
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

            // Prefab ���� �� ��ε�
            PrefabUtility.SaveAsPrefabAsset(loadedPrefab, path);
            PrefabUtility.UnloadPrefabContents(loadedPrefab);

            updatedCount++;
        }

        Debug.Log($"�� {updatedCount}���� �����տ� AnimationController�� �߰��Ǿ����ϴ�.");
    }

    // ���� ������������ ��������� ������Ʈ�� ã�� �޼���
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform found = FindChildRecursive(child, childName); // �ڽ��� �ڽ� Ž��
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}