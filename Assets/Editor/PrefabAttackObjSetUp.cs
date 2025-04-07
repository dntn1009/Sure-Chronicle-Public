using UnityEngine;
using UnityEditor;

public class PrefabAttackObjSetUp : EditorWindow
{
    [SerializeField]
    private GameObject[] prefabObjects; // 여러 Prefab을 받을 배열

    private SerializedObject serializedObjectReference;
    private SerializedProperty prefabArrayProperty;

    private string childObjectTag = "FindTarget"; // 자식 오브젝트의 태그
    private Vector3 sphereColliderCenter = Vector3.zero; // Sphere Collider의 중심
    private float sphereColliderRadius = 1.0f; // Sphere Collider의 반지름

    [MenuItem("Tools/Prefab Attack Object Setup")]
    public static void ShowWindow()
    {
        GetWindow<PrefabAttackObjSetUp>("Prefab Attack Object Setup");
    }

    private void OnEnable()
    {
        // SerializedObject 초기화
        serializedObjectReference = new SerializedObject(this);
        prefabArrayProperty = serializedObjectReference.FindProperty("prefabObjects");
    }

    private void OnGUI()
    {
        // SerializedObject 업데이트
        serializedObjectReference.Update();

        GUILayout.Label("Prefab Attack Object Setup", EditorStyles.boldLabel);

        // Prefab 배열 입력
        EditorGUILayout.PropertyField(prefabArrayProperty, new GUIContent("Prefab Objects"), true);

        // 자식 오브젝트 태그 설정
        childObjectTag = EditorGUILayout.TextField("Child Object Tag", childObjectTag);

        // Sphere Collider 설정
        GUILayout.Label("Sphere Collider Settings", EditorStyles.boldLabel);
        sphereColliderCenter = EditorGUILayout.Vector3Field("Collider Center", sphereColliderCenter);
        sphereColliderRadius = EditorGUILayout.FloatField("Collider Radius", sphereColliderRadius);

        // 실행 버튼
        if (GUILayout.Button("Add Components to Prefabs"))
        {
            serializedObjectReference.ApplyModifiedProperties(); // 데이터 동기화
            AddComponentsToPrefabs();
        }

        // SerializedObject 최종 업데이트
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

            // Prefab 인스턴스 생성
            GameObject prefabInstance = Instantiate(prefab);
            prefabInstance.name = prefab.name + "_Instance";

            // 자식 오브젝트 생성 및 설정
            GameObject childObject = new GameObject("FindTarget");
            childObject.transform.SetParent(prefabInstance.transform);
            childObject.transform.localPosition = Vector3.zero;
            childObject.tag = childObjectTag;

            // Sphere Collider 추가 및 설정
            SphereCollider sphereCollider = childObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = sphereColliderCenter;
            sphereCollider.radius = sphereColliderRadius;

            // AttackAreUnitFind 스크립트 추가
            childObject.AddComponent<AttackAreUnitFind>();

            // Prefab 저장
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(prefabInstance, prefabPath, InteractionMode.UserAction);

            // Prefab 인스턴스 삭제
            DestroyImmediate(prefabInstance);

            Debug.Log($"Prefab '{prefab.name}'에 자식 오브젝트를 성공적으로 추가했습니다.");
        }
    }
}