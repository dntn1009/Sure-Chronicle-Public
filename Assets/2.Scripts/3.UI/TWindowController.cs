using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using UnityEngine.UI;

public class TWindowController : MonoBehaviour, IWindowFacadePattern, ICompositePattern
{
    [Header("Position & Back")]
    [SerializeField] float _hideWindowPos;
    [SerializeField] Button _backBtn;

    RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }
    public void CloseWindow()
    {
        AudioManager.Instance.LoadSFX("SFX/Close.wav");
        _rect.offsetMin = new Vector2(_hideWindowPos, 0);
        _rect.offsetMax = new Vector2(_hideWindowPos, 0);
    }

    public virtual void OpenWindow()
    {
        AudioManager.Instance.LoadSFX("SFX/TopButton.wav");
        _rect.offsetMin = new Vector2(0, 0);
        _rect.offsetMax = new Vector2(0, 0);
    }

    public virtual void Operate()
    {
        _backBtn.onClick.AddListener(() => CloseWindow());
        CloseWindow();
    }

}
