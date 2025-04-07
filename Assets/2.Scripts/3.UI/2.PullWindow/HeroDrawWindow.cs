using DesignPattern;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDrawWindow : MonoBehaviour, IWindowFacadePattern
{
    [Header("UI")]
    [SerializeField] Button _allOpenBtn;
    [SerializeField] Button _closeBtn;
    [SerializeField] Transform _listContent;

    [Header("Position")]
    [SerializeField] float _hideWindowPos;
    [SerializeField] RectTransform _rect;

    [Header("DrawCard")]
    [SerializeField] DrawCard _drawCard;

    List<DrawCard> _drawList;

    private void Awake()
    {
        _drawList = new List<DrawCard>();
        CloseWindow();
        _allOpenBtn.onClick.AddListener(AllOpenBtnClick);
        _closeBtn.onClick.AddListener(CloseBtnClick);
    }
    #region [Initialize Methods]
    public void SetDrawWindow(List<CardData> codeList)
    {
        OpenWindow();
        foreach (CardData cardData in codeList)
        {
            var obj = Instantiate(_drawCard, _listContent).Initialize(cardData);
            obj.OnCardOpened += OnCardOpenedHandler;
           _drawList.Add(obj);
        }
    }

    #endregion [Initialize Methods]

    #region [IWindowFacadePattern & UI Methods]
    public void CloseWindow()
    {
        AudioManager.Instance.LoadSFX("SFX/Close.wav");

        if (_drawList.Count > 0)
        {
            for (int i = _drawList.Count - 1; i >= 0; i--)
            {
                _drawList[i].OnCardOpened -= OnCardOpenedHandler;
                Destroy(_drawList[i].gameObject);
                _drawList.RemoveAt(i);
            }
        }
        _rect.offsetMin = new Vector2(0, _hideWindowPos);
        _rect.offsetMax = new Vector2(0, _hideWindowPos);
        ButtonsSetActives();
    }

    public void OpenWindow()
    {
        _rect.offsetMin = new Vector2(0, 0);
        _rect.offsetMax = new Vector2(0, 0);
        ButtonsSetActives();
    }

    public void ButtonsSetActives(bool _open = true)
    {
        _allOpenBtn.gameObject.SetActive(_open);
        _closeBtn.gameObject.SetActive(!_open);
    }

    public void AllOpenBtnClick()
    {
        foreach (var card in _drawList)
        {
            card.OpenCard();
        }
    }
    public void CloseBtnClick()
    {
        CloseWindow();
    }
    #endregion [IWindowFacadePattern & UI Methods]

    private void OnCardOpenedHandler(DrawCard openedCard)
    {
        // 모든 카드가 열렸는지 확인
        CheckAllCardsOpened();
    }

    // 모든 카드가 열렸는지 체크하고 버튼 활성화 처리
    private void CheckAllCardsOpened()
    {
        bool allOpened = true;

        foreach (var card in _drawList)
        {
            if (!card.IsOpen)
            {
                allOpened = false;
                break;
            }
        }
        // 모든 카드가 열렸다면 버튼 활성화
        if (allOpened)
        {
            ButtonsSetActives(false);
        }
    }

}
