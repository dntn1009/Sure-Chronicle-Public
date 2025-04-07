using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class AddHitPosToPrefabs : EditorWindow
{
    [SerializeField] private GameObject[] prefabObjects; // ������ ����Ʈ
    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    [MenuItem("Tools/Add HitPos to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<AddHitPosToPrefabs>("Add HitPos to Prefabs");
    }

    private void OnEnable()
    {
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        serializedObjectReference.Update();

        GUILayout.Label("Add HitPos to Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prefabArrayProperty, true);

        GUILayout.Space(10);

        if (GUILayout.Button("Add HitPos to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties();
            AddHitPosToAllPrefabs();
        }

        serializedObjectReference.ApplyModifiedProperties();
    }

    private void AddHitPosToAllPrefabs()
    {
        if (prefabObjects == null || prefabObjects.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned. Please assign prefabs.");
            return;
        }

        int updatedCount = 0;

        foreach (GameObject prefab in prefabObjects)
        {
            if (prefab == null) continue;

            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(path);

            if (loadedPrefab == null)
            {
                Debug.LogWarning($"Failed to load prefab: {prefab.name}");
                continue;
            }

            // HitPos ã�� (������ ���� ����)
            Transform hitPos = FindChildRecursive(loadedPrefab.transform, "HitPos");

            if (hitPos == null) // HitPos�� ������ �߰�
            {
                hitPos = CreateHitPos(loadedPrefab.transform);
                Debug.Log($"HitPos added to prefab: {prefab.name}");
            }
            else
            {
                Debug.Log($"HitPos already exists in prefab: {prefab.name}");
            }

            // Prefab ���� �� ����
            PrefabUtility.SaveAsPrefabAsset(loadedPrefab, path);
            PrefabUtility.UnloadPrefabContents(loadedPrefab);
            updatedCount++;
        }

        Debug.Log($"Total {updatedCount} prefabs updated with HitPos.");
    }

    /// <summary>
    /// HitPos�� ���� �����ϰ� �����ϴ� �Լ�
    /// </summary>
    private Transform CreateHitPos(Transform parent)
    {
        GameObject hitPosObj = new GameObject("HitPos");
        hitPosObj.transform.SetParent(parent);
        hitPosObj.transform.localPosition = new Vector3(0.25f, 0.65f, 0);
        hitPosObj.transform.localScale = Vector3.one;

        return hitPosObj.transform;
    }

    /// <summary>
    /// Ư�� �̸��� ���� �ڽ� Transform�� ��������� ã�� �Լ�
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null) return null;

        foreach (Transform child in parent)
        {
            if (child == null) continue;

            if (child.name == childName)
            {
                return child;
            }

            Transform found = FindChildRecursive(child, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}