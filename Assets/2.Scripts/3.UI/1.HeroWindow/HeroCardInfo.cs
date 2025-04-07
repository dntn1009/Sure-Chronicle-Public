using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HeroCardInfo : MonoBehaviour
{
    [Header("UI")]
    [Header("Hero UI")]
    [SerializeField] TextMeshProUGUI _heroNameText;
    [SerializeField] Image _heroImg;
    [SerializeField] TextMeshProUGUI _heroPowerText;
    [SerializeField] TextMeshProUGUI _heroHPText;
    [SerializeField] TextMeshProUGUI _heroAttText;
    [SerializeField] TextMeshProUGUI _heroDefText;
    [SerializeField] TextMeshProUGUI _heroAttSpeedText;
    [SerializeField] TextMeshProUGUI _heroCritChanceText;
    [SerializeField] TextMeshProUGUI _heroCritDamageText;
    [SerializeField] TextMeshProUGUI _heroLifeStealText;
    [Header("Button UI")]
    [SerializeField] TextMeshProUGUI _enforceText;
    [SerializeField] TextMeshProUGUI _lvupText;
    [SerializeField] Button _enforceBtn;
    [SerializeField] Button _lvupBtn;
    [SerializeField] Button _backBtn;
    [SerializeField] Image _backImg;

    [Header("Upgrade & Failed Card Notify UI")]

    [Header("Upgrade UI")]
    [SerializeField] CardUpgradePanel _LvUpPanel;
    [SerializeField] CardUpgradePanel _starUpPanel;

    Card _card;
    public Card SelectCard { get { return _card; } }

    #region [Init & CardInfoPanel Methods]

    public void Initialize()
    {
        _backBtn.onClick.AddListener(Back);
        _enforceBtn.onClick.AddListener(SetStarBtn);
        _lvupBtn.onClick.AddListener(SetLvUpBtn);
        _LvUpPanel.Initialize(this);
        _starUpPanel.Initialize(this);
    }

    public void CardInfoSetCard(Card card)
    {
        _card = card;
        _backImg.color = _card.TypeColor;
        _heroNameText.text = _card.CardName;
        DataManager.Instance.LoadAddressable<Sprite>(_card.CardImg, (sprite) =>
        {
            _heroImg.sprite = sprite;
        });

        var powerValue = (int)((_card.MyStatus._hp * 0.8f) + (_card.MyStatus._def * 1f) + (_card.MyStatus._att * 1.5f));
        _heroPowerText.text = powerValue.ToString("N0");
        _heroHPText.text = _card.MyStatus._hp.ToString("N0");
        _heroAttText.text = _card.MyStatus._att.ToString("N0");
        _heroDefText.text = _card.MyStatus._def.ToString("N0");
        _heroAttSpeedText.text = _card.MyStatus._attSpeed.ToString("N0");
        _heroCritChanceText.text = _card.MyStatus._critChance.ToString("N0");
        _heroCritDamageText.text = _card.MyStatus._critDamage.ToString("N0");
        _heroLifeStealText.text = _card.MyStatus._LifeSteal.ToString("N0");
        if (_card.MyStar < 5)
            _enforceText.text = "초월(" + _card.Count + "/" + _card.CountMax + ")";
        else
            _enforceText.text = "초월(" + _card.Count + "/" + "MAX" + ")";

        _lvupText.text = "레벨(" + _card.MyLevel + "/" + "30)";
        _LvUpPanel.ClosePanel();
        _starUpPanel.ClosePanel();

        if (_card.Count < _card.CountMax)
            _enforceBtn.interactable = false;
        else
            _enforceBtn.interactable = true;
    }

    public void CardInfoSetCard()
    {
        _backImg.color = _card.TypeColor;
        _heroNameText.text = _card.CardName;
        DataManager.Instance.LoadAddressable<Sprite>(_card.CardImg, (sprite) =>
        {
            _heroImg.sprite = sprite;
        });
        var powerValue = (int)((_card.MyStatus._hp * 0.8f) + (_card.MyStatus._def * 1f) + (_card.MyStatus._att * 1.5f));
        _heroPowerText.text = powerValue.ToString("N0");
        _heroHPText.text = _card.MyStatus._hp.ToString("N0");
        _heroAttText.text = _card.MyStatus._att.ToString("N0");
        _heroDefText.text = _card.MyStatus._def.ToString("N0");
        _heroAttSpeedText.text = _card.MyStatus._attSpeed.ToString("N0");
        _heroCritChanceText.text = _card.MyStatus._critChance.ToString("N0");
        _heroCritDamageText.text = _card.MyStatus._critDamage.ToString("N0");
        _heroLifeStealText.text = _card.MyStatus._LifeSteal.ToString("N0");
        if (_card.MyStar < 5)
            _enforceText.text = "초월(" + _card.Count + "/" + _card.CountMax + ")";
        else
            _enforceText.text = "초월(" + _card.Count + "/" + "MAX" + ")";

        _lvupText.text = "레벨(" + _card.MyLevel + "/" + "30)";
        _LvUpPanel.ClosePanel();
        _starUpPanel.ClosePanel();

        if (_card.Count < _card.CountMax)
            _enforceBtn.interactable = false;
        else
            _enforceBtn.interactable = true;
    }

    public void Back()
    {
        if (_card != null)
        {
            _card._isSelected = false;
            _card = null;
        }
        gameObject.SetActive(false);
    }

    #endregion [[Init & CardInfoPanel Methods]]

    #region [Upgrade & Failed Message UI Mehods]
    public void SetStarBtn()
    {
        if (_card != null)
            _starUpPanel.OpenPanel(_card);
    }

    public void SetLvUpBtn()
    {
        if (_card != null)
            _LvUpPanel.OpenPanel(_card);
    }
    #endregion [Upgrade & Failed Message UI Mehods]
}
