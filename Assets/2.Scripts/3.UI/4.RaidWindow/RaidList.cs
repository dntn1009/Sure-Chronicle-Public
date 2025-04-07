using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;

public class RaidList : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _raidImg;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] Button _raidBtn;

    [Header("Type")]
    [SerializeField] RaidType _myType;

    TicketData _ticketData;

    private void Awake()
    {
        _raidBtn.onClick.AddListener(() => raidBtnClick(_myType));
    }

    public void Initialize(TicketData ticketData)
    {
        _ticketData = ticketData;
        SetTickCount();
    }

    public void SetTickCount()
    {
        switch (_myType)
        {
            case RaidType.Gold:
                _countText.text = _ticketData._goldRaidTicket.ToString("N0");
                break;
            case RaidType.Jewel:
                _countText.text = _ticketData._jewelRaidTicket.ToString("N0");
                break;
        }
    }

    public void raidBtnClick(RaidType type)
    {
        int count = 0;
        switch(type)
        {
            case RaidType.Gold:
                count = _ticketData._goldRaidTicket;
                break;
            case RaidType.Jewel:
                count = _ticketData._jewelRaidTicket;
                break;
        }

        if (count <= 0 || DataManager.Instance.IsRaid)
            return;

        switch (type)
        {
            case RaidType.Gold:
                count--;
                _ticketData._goldRaidTicket = count;
                MoveRaidMap("GoldRaid");
                break;
            case RaidType.Jewel:
                count--;
                _ticketData._jewelRaidTicket = count;
                MoveRaidMap("JewelRaid");
                break;
        }
        AudioManager.Instance.LoadSFX("SFX/RaidButton.wav");
        SetTickCount();
        DataManager.Instance.SaveDataAndUINotifyObservers();
    }

    public void MoveRaidMap(string sceneName)
    {
        UIManager.Instance.WindowOpenOrClose(WindowState.None);
        DataManager.Instance.IsRaid = true;
        string removeName = string.Empty;
        if (DataManager.Instance.StageData._stageNumber % 2 == 1)
            removeName = "Stage1";
        else
            removeName = "Stage2";

        StartCoroutine(GameSceneManager.Instance.StageAddtive(sceneName, removeName));
    }

}
