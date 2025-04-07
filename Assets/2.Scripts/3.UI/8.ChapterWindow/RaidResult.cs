using DefineHelper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaidResult : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _rewardText;
    [SerializeField] Button _stageBtn;
    [SerializeField] Image _rewardImg;
    [SerializeField] Sprite[] _rewardIcons;


    bool _isJewel;

    public void Initialize()
    {
        _stageBtn.onClick.AddListener(() => BackToStage());
        this.gameObject.SetActive(false);
    }

    public void ShowRaidClear(bool isJewel, bool isClear, int reward = 0)
    {
        this.gameObject.SetActive(true);
        UIManager.Instance.WindowOpenOrClose(WindowState.None);
       
        _isJewel = isJewel;
        if (!_isJewel)
            _rewardImg.sprite = _rewardIcons[0];
        else
            _rewardImg.sprite = _rewardIcons[1];

        if (isClear)
        {
            _titleText.text = "레이드 성공";
            _rewardText.text = reward.ToString("N0");
            return;
        }
        _titleText.text = "레이드 실패";
        _rewardText.text = "보상 없음";
    }

    void BackToStage()
    {
        DataManager.Instance.BackToStage(_isJewel);
        this.gameObject.SetActive(false);
    }

}
