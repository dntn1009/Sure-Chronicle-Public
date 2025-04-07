using DefineHelper;
using DesignPattern;
using fsmStatePattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroWindow : WindowController
{
    [Header("히어로 윈도우")]

    [Header("Button & Panel")]
    [SerializeField] Button _listBtn;
    [SerializeField] Button _formingBtn;
    [SerializeField] RectTransform _list;
    [SerializeField] RectTransform _forming;
    [SerializeField] float _hidePanelPos;
    [SerializeField] TextMeshProUGUI _listOwnText;
    [SerializeField] TextMeshProUGUI _formingOwnText;
    [SerializeField] HeroCardInfo _heroCardInfo;

    [Header("Card List")]
    [SerializeField] Transform _listContent;
    [SerializeField] Transform _formingContent;
    [SerializeField] List<Card> _listCards;
    [SerializeField] List<Card> _formingCards;

    [Header("Equip Slots")]
    [SerializeField] GameObject _equipNotfiy;
    [SerializeField] EquipSlot[] _equipSlots;

    [Header("Color")]
    [SerializeField] Color _initColor;
    [SerializeField] Color _pushColor;

    HashSet<int> _existCardCodes;

    Card _equipClickCard;

    public Card EquipClickCard { get { return _equipClickCard; } }

    private void Start()
    {
        //InitCardList();
    }

    public override void Operate()
    {
        base.Operate();
        InitSet();
        StartCoroutine(InitCardList());
    }

    #region [Init Methods]
    void InitSet()
    {

        _listBtn.onClick.AddListener(() => ChangePanel(_listBtn));
        _formingBtn.onClick.AddListener(() => ChangePanel(_formingBtn));
        _list.offsetMin = new Vector2(0, 0);
        _list.offsetMax = new Vector2(0, 0);
        _listBtn.GetComponent<Image>().color = _pushColor;//list
        _forming.offsetMin = new Vector2(_hidePanelPos, 0);
        _forming.offsetMax = new Vector2(_hidePanelPos, 0);
        _formingBtn.GetComponent<Image>().color = _initColor;//forming
        _heroCardInfo.Initialize();
        SetAdjustHeight();
    }

    void ChangePanel(Button button)
    {
        if (button == _listBtn)
        {
            _list.offsetMin = new Vector2(0, 0);
            _list.offsetMax = new Vector2(0, 0);
            _listBtn.GetComponent<Image>().color = _pushColor;//list
            _forming.offsetMin = new Vector2(_hidePanelPos, 0);
            _forming.offsetMax = new Vector2(_hidePanelPos, 0);
            _formingBtn.GetComponent<Image>().color = _initColor;//forming
            FormingAllDeSelected();
        }
        else if (button == _formingBtn)
        {
            _forming.offsetMin = new Vector2(0, 0);
            _forming.offsetMax = new Vector2(0, 0);
            _formingBtn.GetComponent<Image>().color = _pushColor;//list
            _list.offsetMin = new Vector2(_hidePanelPos, 0);
            _list.offsetMax = new Vector2(_hidePanelPos, 0);
            _listBtn.GetComponent<Image>().color = _initColor;//forming
            ListAllDeSelected();
        }

    }
    public void InitEquipSlots()
    {
        _equipNotfiy.SetActive(false);
        foreach (EquipSlot slot in _equipSlots)
        {
            slot.Initialize();
        }
    }

    void SetAdjustHeight()
    {
        int rowCount = _listCards.Count / 4;
        if (_listCards.Count % 4 > 0)
            rowCount += 1;  // 나머지가 있으면 추가 행

        float totalHeight = 380 * rowCount;
        // 행별로 다른 높이 적용
        _listContent.GetComponent<RectTransform>().sizeDelta = new Vector2(_listContent.GetComponent<RectTransform>().sizeDelta.x, totalHeight);
        _formingContent.GetComponent<RectTransform>().sizeDelta = new Vector2(_formingContent.GetComponent<RectTransform>().sizeDelta.x, totalHeight);
    }

    public void SetEquipClickCard(Card card)
    {
        if (card.EquipIndex == -1)
        {
            _equipClickCard = card;
            _equipNotfiy.SetActive(true);
        }
        else
        {
            _equipClickCard = null;
            _equipSlots[card.EquipIndex].Initialize();
            DataManager.Instance.EquipCardToSlot(card);
            FinishEquipClick();
        }
    }
    public void FinishEquipClick()
    {
        _equipNotfiy.SetActive(false);
        FormingAllDeSelected();
        ListAllDeSelected();
    }

    public bool EquipCardClickCheck()
    {
        if (_equipClickCard != null)
            return true;
        else
            return false;
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        ListAllDeSelected();
        FormingAllDeSelected();
        _heroCardInfo.Back();
    }

    public void SetAutoHeroObj(IState<HeroController> state)
    {
        foreach (EquipSlot slot in _equipSlots)
        {
            if (slot.HeroObj == null)
                continue;

            var hero = slot.HeroObj;
            if (!hero.IsDeath)
                hero.ChangeAnyFromState(state);
        }
    }
    #endregion [Init Methods]

    #region [Data Enter & List Methods]
    public IEnumerator CardListSort()
    {
        _listCards = _listCards.OrderBy(obj => obj.EquipIndex == -1)
       .ThenBy(obj => obj.EquipIndex)
       .ThenByDescending(obj => obj.MyCardType)
       .ThenByDescending(obj => obj.MyStar)
       .ThenByDescending(obj => obj.MyLevel)
       .ToList();

        _formingCards = _formingCards.OrderBy(obj => obj.EquipIndex == -1)
            .ThenBy(obj => obj.EquipIndex)
            .ThenByDescending(obj => obj.MyCardType)
            .ThenByDescending(obj => obj.MyStar)
            .ThenByDescending(obj => obj.MyLevel)
            .ToList();

        for (int i = 0; i < _listCards.Count; i++)
        {
            _listCards[i].transform.SetSiblingIndex(i);
            _formingCards[i].transform.SetSiblingIndex(i);
        }

        SetAdjustHeight();

        yield return null; // 한 프레임 대기 (비동기 처리 보장)
    } // 카드 리스트 순위 조건 순으로 정렬

    public IEnumerator InitCardList()
    {
        SetGoldJewel();
        InitEquipSlots(); // 초기화시켜주고,
        _listCards = new List<Card>();
        _formingCards = new List<Card>();
        _existCardCodes = new HashSet<int>();

        var list = DataManager.Instance.UserCardList;
        var cardDatas = DataManager.Instance.CardDatas;
        var card = DataManager.Instance.Card;

        UpdateHeroCount(list, cardDatas.Length);

        foreach (UserCardData usercardData in list)
        {
            AddCardList(usercardData, cardDatas, card);
            _existCardCodes.Add(usercardData._cardCode);
        }

        yield return StartCoroutine(CardListSort());

        yield return InitFormingEquip();

        InitHeroAuto(UIManager.Instance.InitAutoRefresh());
    }

    IEnumerator InitFormingEquip()
    {
        foreach (Card card in _formingCards)
        {
            if (card.EquipIndex != -1)
            {
                if(_equipSlots[card.EquipIndex] == null)
                {
                    Debug.Log(card.EquipIndex + "에러 인덱스");
                }

                yield return StartCoroutine(_equipSlots[card.EquipIndex].InitializeCoroutine(card));
            }
            yield return null;
        }
    }
    public void InitHeroAuto(bool autoCheck)
    {
        if (autoCheck)
            SetAutoHeroObj(new hSearchState());
        else
            SetAutoHeroObj(new hIdleState());
    }
    public void UpdateHeroCount(List<UserCardData> list, int cardMax)
    {
        _listOwnText.text = "( " + list.Count + " / " + cardMax + " )";
        _formingOwnText.text = "( " + list.Count + " / " + cardMax + " )";
    }

    public void UpdateHeroCount()
    {
        var list = DataManager.Instance.UserCardList;
        int cardMax = DataManager.Instance.CardDatas.Length;
        _listOwnText.text = "( " + list.Count + " / " + cardMax + " )";
        _formingOwnText.text = "( " + list.Count + " / " + cardMax + " )";
    }

    public void AddCardList(UserCardData userCardData, CardData[] cardDatas, Card card)
    {
        _listCards.Add(Instantiate(card, _listContent).Initialize(cardDatas[userCardData._cardCode], userCardData, CardList.List));
        _formingCards.Add(Instantiate(card, _formingContent).Initialize(cardDatas[userCardData._cardCode], userCardData, CardList.Forming));
    } //처음 인게임 진입 시 카드 생성


    public int FindEquipIndex(EquipSlot slot)
    {
        return System.Array.IndexOf(_equipSlots, slot);
    }

    #endregion [Data Enter Methods]

    #region [Observer Pattern Methods]

    public override void Notify(UserData userdata)
    {
        base.Notify(userdata);
        UpdateHeroCount(userdata._userCardList, DataManager.Instance.CardDatas.Length);
        foreach (UserCardData usercarddata in userdata._userCardList)
        {
            if (_existCardCodes.Contains(usercarddata._cardCode))
            {
                Card listexistCard = _listCards.FirstOrDefault(c => c.CardCode == usercarddata._cardCode);
                listexistCard.UpdateData(usercarddata, userdata);
                Card formingexistCard = _formingCards.FirstOrDefault(c => c.CardCode == usercarddata._cardCode);
                formingexistCard.UpdateData(usercarddata, userdata);
            }
            else
            {
                var card = DataManager.Instance.Card;
                var cardDatas = DataManager.Instance.CardDatas;
                AddCardList(usercarddata, cardDatas, card);
                _existCardCodes.Add(usercarddata._cardCode);
            }
        }

        StartCoroutine(CardListSort());

        if (_heroCardInfo.gameObject.activeSelf)
        {
            _heroCardInfo.CardInfoSetCard();
        }
        FinishEquipClick();
    }

    #endregion [Observer Pattern Methods]

    #region [Card Clicked Methods]

    public void SelectCardInfo(Card card)
    {
        _heroCardInfo.gameObject.SetActive(true);
        _heroCardInfo.CardInfoSetCard(card);
    }

    public void ListAllDeSelected()
    {
        foreach (Card card in _listCards)
        {
            card.DeSelect();
        }
    }

    public void FormingAllDeSelected()
    {
        _equipNotfiy.SetActive(false);
        _equipClickCard = null;
        foreach (Card card in _formingCards)
        {
            card.DeSelect();
        }
    }

    #endregion [Card Clicked Methods]
}
