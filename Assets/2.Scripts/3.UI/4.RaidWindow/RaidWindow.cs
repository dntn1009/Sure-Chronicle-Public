using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidWindow : WindowController
{
    [SerializeField] RaidList[] _raidList;

    private void Start()
    {
        //InitializeSet();
    }

    public void InitializeSet()
    {
        SetGoldJewel();
        var ticketData = DataManager.Instance.TicketData;
        foreach(RaidList list in _raidList)
        {
            list.Initialize(ticketData);
        }
    }


    public override void Operate()
    {
        base.Operate();
        InitializeSet();
    }

    #region [Observer Pattern Methods]


    #endregion [Observer Pattern Methods]
}
