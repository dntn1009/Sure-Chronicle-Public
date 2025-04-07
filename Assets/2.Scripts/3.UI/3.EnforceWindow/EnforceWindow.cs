using UnityEngine;

public class EnforceWindow : WindowController
{
    [Header("SharedEnforceData Lists")]
    [SerializeField] EnforceList[] _enforceList;

    private void Start()
    {
        //InitializeSet();
    }

    public override void Operate()
    {
        base.Operate();
        InitializeSet();
    }

    public void InitializeSet()
    {
        SetGoldJewel();
        var EnforceData = DataManager.Instance.SharedEnforceData;
        foreach (EnforceList list in _enforceList)
        {
            list.InitializeSet(EnforceData);
        }
    }



    private void ListByInit()
    {

    }

    #region [Observer Pattern Methods]
    public override void Notify(UserData userdata)
    {
        base.Notify(userdata);
    }

    #endregion [Observer Pattern Methods]
}
