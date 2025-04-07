using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using DefineHelper;
using TMPro;

public class WindowController : MonoBehaviour, IWindowFacadePattern, ICompositePattern, IUIObserver
{
    [Header("Position")]
    [SerializeField] WindowState _myState;
    [SerializeField] float _hideWindowPos;

    [Header("Window Common UI")]
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _jewelText;
    RectTransform _rect;

    public WindowState MyState { get{ return _myState; } }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        //Operate();
    }
    public virtual void CloseWindow()
    {
        _rect.offsetMin = new Vector2(_hideWindowPos, 0);
        _rect.offsetMax = new Vector2(_hideWindowPos, 0);//Hero
    }

    public void OpenWindow()
    {
        AudioManager.Instance.LoadSFX("SFX/Button.wav");
        _rect.offsetMin = new Vector2(0, 0);
        _rect.offsetMax = new Vector2(0, 0);//Hero
    }

    public void SetGoldJewel()
    {
        _goldText.text = DataManager.Instance.Gold.ToString("N0");
        _jewelText.text = DataManager.Instance.Jewel.ToString("N0");

    }

    public void SetGoldJewel(UserData userdata)
    {
        if (_goldText != null && _jewelText != null)
        {
            _goldText.text = userdata._gold.ToString("N0");
            _jewelText.text = userdata._jewel.ToString("N0");
        }
    }


    #region [Composite Pattern Methods]
    public virtual void Operate()
    {
        CloseWindow();
    }

    #endregion [Composite Pattern Methods]


    #region [Observer Pattern Methods]
    public virtual void Notify(UserData userdata)
    {
        Debug.Log("Observer " + this.name + "ผ๖วเ");
        SetGoldJewel(userdata);
    }

    #endregion [Observer Pattern Methods]
}
