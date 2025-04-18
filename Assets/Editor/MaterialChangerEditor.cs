using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialChangerEditor : EditorWindow
{
    private GameObject[] prefabs = new GameObject[0]; // 여러 개의 프리팹을 선택 가능
    private Material targetMaterial; // 변경 대상이 되는 Material
    private Material newMaterial; // 새롭게 적용할 Material

    [MenuItem("Tools/Batch Material Changer")]
    public static void ShowWindow()
    {
        GetWindow<MaterialChangerEditor>("Batch Material Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Material Changer", EditorStyles.boldLabel);

        // 배열 크기 설정
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

        // 오브젝트 배열 입력
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", prefabs[i], typeof(GameObject), false);
        }

        targetMaterial = (Material)EditorGUILayout.ObjectField("Target Material", targetMaterial, typeof(Material), false);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);

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
            SpriteRenderer[] spriteRenderers = prefabInstance.GetComponentsInChildren<SpriteRenderer>();

            int changeCount = 0;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.sharedMaterial == targetMaterial)
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

        Debug.Log($"총 {totalChanges}개의 SpriteRenderer의 Material이 변경되었습니다. 변경된 프리팹: {string.Join(", ", modifiedPrefabs)}");
    }
}
