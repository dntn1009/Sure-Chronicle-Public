using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAdminManager
{
    [MenuItem("Build/Build Admin Manager Scene")]
    public static void BuildWindowsScene()
    {
        // ������ Scene ���
        string[] scenes = { "Assets/1.Scenes/2.AdminScene/AdminScene.unity" };

        // ���� �÷����� Windows���� Ȯ��
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            Debug.Log("Switching platform to Windows...");
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
            {
                Debug.LogError("Failed to switch to Windows platform!");
                return;
            }
        }

        // ���� �ɼ� ����
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Build/AdminManager.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        // ���� ����
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // ���� ��� Ȯ��
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded!");
        }
        else
        {
            Debug.LogError($"Build failed: {report.summary.result}");
        }
    }
}
