using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BatchMaterialTargetNameChanger : EditorWindow
{
    private GameObject[] prefabs = new GameObject[0]; // ���� ���� �������� ���� ����
    private Material targetMaterial; // ���� ����� �Ǵ� Material
    private Material newMaterial; // ���Ӱ� ������ Material
    private string targetObjectName = "Shadow"; // Ư�� �̸��� ���� ������Ʈ�� ����

    [MenuItem("Tools/Batch Material Target Name Changer")]
    public static void ShowWindow()
    {
        GetWindow<BatchMaterialTargetNameChanger>("Batch Material TargetName Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Material TargetName Changer", EditorStyles.boldLabel);

        // �迭 ũ�� ����
        int arraySize = Mathf.Max(0, EditorGUILayout.IntField("Number of Prefabs", prefabs.Length));
        if (arraySize != prefabs.Length)
        {
            GameObject[] newPrefabs = new GameObject[arraySize];
            for (int i = 0; i < Mathf.Min(arraySize, prefabs.Length); i++)
            {
                newPrefabs[i] = prefabs[i];
            }
            prefabs = newPrefabs;
        }

        // ������Ʈ �迭 �Է�
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", prefabs[i], typeof(GameObject), false);
        }

        targetMaterial = (Material)EditorGUILayout.ObjectField("Target Material", targetMaterial, typeof(Material), false);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);
        targetObjectName = EditorGUILayout.TextField("Target Object Name", targetObjectName);

        if (GUILayout.Button("Apply Material to Matching SpriteRenderers") && prefabs.Length > 0 && targetMaterial != null && newMaterial != null)
        {
            ApplyMaterialToPrefabs();
        }
    }

    private void ApplyMaterialToPrefabs()
    {
        int totalChanges = 0;
        List<string> modifiedPrefabs = new List<string>();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            SpriteRenderer[] spriteRenderers = prefabInstance.GetComponentsInChildren<SpriteRenderer>(true);

            int changeCount = 0;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.gameObject.name == targetObjectName && spriteRenderer.sharedMaterial == targetMaterial)
                {
                    spriteRenderer.material = newMaterial;
                    changeCount++;
                }
            }

            if (changeCount > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                modifiedPrefabs.Add(prefab.name);
                totalChanges += changeCount;
            }

            DestroyImmediate(prefabInstance);
        }

        Debug.Log($"�� {totalChanges}���� '{targetObjectName}' ������Ʈ�� Material�� ����Ǿ����ϴ�. ����� ������: {string.Join(", ", modifiedPrefabs)}");
    }
}
