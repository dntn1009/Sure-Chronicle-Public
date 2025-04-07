using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;

public class EnforceList : MonoBehaviour
{
    [SerializeField] EnforceListType _myType;
    [SerializeField] TextMeshProUGUI _LvText;
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] Button _EnforceBtn;

    int _mylevel;
    int _enforceCost;

    SharedEnforceData _enforceData;

    public int MyLevel { get { return _mylevel; } }
    public int EnforceCost { get { return _enforceCost; } }

    private void Awake()
    {
        _EnforceBtn.onClick.AddListener(() => EnforceClick(_enforceData));
    }

    public void InitializeSet(SharedEnforceData enforceData)
    {
        _enforceData = enforceData;
        SelectCostFromType(_enforceData);
        //Text
        SetUIText();
    }

    void SetUIText()
    {
        _LvText.text = "Lv." + _mylevel;
        if (_mylevel >= 20)
            _LvText.text += "(MAX)";
        _costText.text = _enforceCost.ToString("N0");
    }

    void SelectCostFromType(SharedEnforceData enforceData)
    {
        switch(_myType)
        {
            case EnforceListType.Attack:
                _mylevel = enforceData._attLv;
                break;
            case EnforceListType.Defence:
                _mylevel = enforceData._defLv;
                break;
            case EnforceListType.HP:
                _mylevel = enforceData._hpLv;
                break;
            case EnforceListType.Gold:
                _mylevel = enforceData._goldLv;
                break;
        }
        _enforceCost = _mylevel * DataManager.Instance.PublicCost._sharedUpgradeCost;
    }

    void UpgradeLevelFromType(SharedEnforceData enforceData)
    {
        if (_mylevel >= 20)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        DataManager.Instance.Gold -= _enforceCost;
        _mylevel++;
        _enforceCost = _mylevel * DataManager.Instance.PublicCost._sharedUpgradeCost;
        switch (_myType)
        {
            case EnforceListType.Attack:
                enforceData._attLv = _mylevel ;
                break;
            case EnforceListType.Defence:
                enforceData._defLv = _mylevel;
                break;
            case EnforceListType.HP:
                enforceData._hpLv = _mylevel;
                break;
            case EnforceListType.Gold:
                enforceData._goldLv = _mylevel;
                break;
        }

        SetUIText();
    }

    public void EnforceClick(SharedEnforceData enforceData)
    {
        if (DataManager.Instance.Gold < _enforceCost)
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }
        AudioManager.Instance.LoadSFX("SFX/UpgradeButton.wav");
        UpgradeLevelFromType(enforceData);
        DataManager.Instance.SaveDataAndUINotifyObservers();

    }
}
