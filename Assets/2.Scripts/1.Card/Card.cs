using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DefineHelper;


public class Card : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Setting")]
    [SerializeField] Image _groundImg;
    [SerializeField] Image _heroImg;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _heroText;
    [SerializeField] Image[] _starImgs;
    [SerializeField] TextMeshProUGUI _ownCardText;
    [SerializeField] Color[] _typeColors;
    [SerializeField] GameObject _equipImg;
    [SerializeField] CardOutLine _outLine;


    [Header("Card Data")]
    [SerializeField] CardData _cardData;
    [SerializeField] UserInfo _userInfo;

    CardList _listType;
    public bool _isSelected;


    CardInfo _myStatus;

    //UserInfo
    public int EquipIndex { get { return _userInfo._equipIndex; } set { _userInfo._equipIndex = value; } }
    public int MyStar { get { return _userInfo._star; } }
    public int MyLevel { get { return _userInfo._level; } }
    public int Count { get { return _userInfo._count; } }
    public int CountMax { get { return _userInfo.CountMax; } }

    //CardData
    public CardType MyCardType { get { return _cardData._cardType; } }
    public int CardCode { get { return _cardData._cardCode; } }
    public string CardName { get { return _cardData._name; } }
    public string CardImg { get { return _cardData._heroSprite; } }
    public CardInfo MyStatus { get { return _myStatus; } }
    public string HeroPrefabs { get { return _cardData._heroPrefab; } }

    //Color
    public Color TypeColor;

    #region [Initialize & Data Methods]

    public Card Initialize(CardData card, UserCardData user, CardList list)
    {
        _cardData = card;
        _userInfo = new UserInfo(user);
        _myStatus = new CardInfo(_cardData._info, DataManager.Instance.SharedEnforceData, _userInfo);
        _groundImg.color = TypeColor = _typeColors[(int)_cardData._cardType];
        DataManager.Instance.LoadAddressable<Sprite>(_cardData._heroSprite, (sprite) =>
        {
            _heroImg.sprite = sprite;
        });

        _levelText.text = _userInfo._level.ToString();
        for (int i = 0; i < user._star; i++)
        {
            _starImgs[i].gameObject.SetActive(true);
        }
        _heroText.text = _cardData._name;

        if(_userInfo._star < 5)
            _ownCardText.text = _userInfo._count + " / " + _userInfo.CountMax;
        else
            _ownCardText.text = _userInfo._count + " / " + "MAX";

        _listType = list;

        EquipSetActive();

        return this;
    }

    public void UpdateData(UserCardData card, UserData user)
    {
        
        _userInfo = new UserInfo(card);
        _myStatus = new CardInfo(_cardData._info, user._sharedEnforceData, _userInfo);
        _levelText.text = _userInfo._level.ToString();
        for (int i = 0; i < card._star; i++)
        {
            _starImgs[i].gameObject.SetActive(true);
        }
        if (_userInfo._star < 5)
            _ownCardText.text = _userInfo._count + " / " + _userInfo.CountMax;
        else
            _ownCardText.text = _userInfo._count + " / " + "MAX";
    }

    #endregion [Initialize & Data Methods]

    #region [Card Click & OutLine & Equip Methods]
    public void CardClick()
    {
        HeroWindow hero = UIManager.Instance.GetHeroWindow();
        switch (_listType)
        {
            case CardList.List:
                hero.ListAllDeSelected();
                _isSelected = true;
                hero.SelectCardInfo(this);
                //카드 정보창 불러오기
                break;
            case CardList.Forming:
                hero.FormingAllDeSelected();
                _isSelected = true;
                UpdateSelected();
                break;
        }
    }

    public void EquipSetActive()
    {
        if (_userInfo._equipIndex != -1)
            _equipImg.SetActive(true);
        else
            _equipImg.SetActive(false);
    }
    public void DeSelect()
    {
        _isSelected = false;
        switch (_listType)
        {
            case CardList.List:
                break;
            case CardList.Forming:
                UpdateSelected();
                break;
        }

        EquipSetActive();
    }
    void UpdateSelected()
    {
        _outLine.SlotByBtnLock();
        _outLine.gameObject.SetActive(_isSelected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardClick();
    }
    #endregion [Card Click & OutLine Methods]
}
