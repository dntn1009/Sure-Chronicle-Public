using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildAdminManager
{
    [MenuItem("Build/Build Admin Manager Scene")]
    public static void BuildWindowsScene()
    {
        // 관리자 Scene 경로
        string[] scenes = { "Assets/1.Scenes/2.AdminScene/AdminScene.unity" };

        // 현재 플랫폼이 Windows인지 확인
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            Debug.Log("Switching platform to Windows...");
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
            {
                Debug.LogError("Failed to switch to Windows platform!");
                return;
            }
        }

        // 빌드 옵션 설정
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Build/AdminManager.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        // 빌드 실행
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // 빌드 결과 확인
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
