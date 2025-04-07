using UnityEditor;
using UnityEngine;

public class UtilsWindow : EditorWindow
{
    /// <summary>
    /// ������ ����
    /// </summary>
    [MenuItem("Tools/Utils", priority = 1)]
    public static void ShowWindow() => GetWindow(typeof(UtilsWindow));

    private void OnGUI()
    {
        if (GUILayout.Button("��ü ����"))
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}