using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PullWindow : WindowController
{
    [Header("UI")]
    [SerializeField] Transform _listContent;

    List<DrawBox> _boxList;

    [Header("DropRateTable")]
    [SerializeField] Button _DropRateTableBtn;
    [SerializeField] GameObject _DropRateTable;
    [SerializeField] Button _DropRateCancelBtn;


    public override void Operate()
    {
        base.Operate();
        InitBoxList();
        InitRateTable();
    }

    #region [Init Methods]

    public void InitRateTable()
    {
        _DropRateTable.SetActive(false);
        _DropRateCancelBtn.onClick.AddListener(() => { _DropRateTable.SetActive(false); });
        _DropRateTableBtn.onClick.AddListener(() => { _DropRateTable.SetActive(true); });
    }

    public void InitBoxList()
    {
        SetGoldJewel();
        _boxList = new List<DrawBox>();
        var drawBox = DataManager.Instance.DrawBox;
        var boxDatas = DataManager.Instance.BoxDatas;
        AddBoxList(drawBox, boxDatas);
    }
    #endregion [Init Methods]

    #region [Data Enter & Content Methods]

    public void AddBoxList(DrawBox drawBox, BoxData[] boxDatas)
    {
        foreach(BoxData boxdata in boxDatas)
        {
            _boxList.Add(Instantiate(drawBox, _listContent).Initialize(boxdata));
        }
    }

    #endregion [Data Enter & Content Methods]


    #region [Observer Pattern Methods]
    public override void Notify(UserData userdata)
    {
        base.Notify(userdata);
    }
    #endregion [Observer Pattern Methods]
}
