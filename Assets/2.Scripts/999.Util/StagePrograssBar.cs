using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StagePrograssBar : PrograssBar
{
    [Header("StageText")]
    [SerializeField] TextMeshProUGUI _stageText;
    [SerializeField] TextMeshProUGUI _roundText;

    public void ChangeAmountFromStageByUserData(StageData stageData)
    {
        int max = DataManager.Instance._roundMAX;
        ChangeAmountFromStage(stageData._roundNumber, max);
        _stageText.text = stageData._stageNumber.ToString();
    }

    public override void ChangeAmountFromStage(int current, int max)
    {
        base.ChangeAmountFromStage(current, max);
        _roundText.text = current + "/" + max;
    }
}
