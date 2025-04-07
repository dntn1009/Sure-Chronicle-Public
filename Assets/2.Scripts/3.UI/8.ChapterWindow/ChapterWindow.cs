using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterWindow : MonoBehaviour
{
    [SerializeField] StageResult _stageResult;
    [SerializeField] RaidResult _raidResult;

    public void Initialize()
    {
        _stageResult.Initialize();
        _raidResult.Initialize();
    }

    #region [StageResult Methods]
    public void OpenStageResult(bool Clear)
    {
        _stageResult.OpenChapterWindow(Clear);
        UIManager.Instance.BOSSSpawn.NextChapterKiiCount();
    }

    #endregion [StageResult Methods]

    #region [RaidResult Methods]

    public void OpenRaidResult(bool isJewel, bool isClear, int reward = 0)
    {
        _raidResult.ShowRaidClear(isJewel, isClear, reward);
    }

    #endregion [RaidResult Methods]
}
