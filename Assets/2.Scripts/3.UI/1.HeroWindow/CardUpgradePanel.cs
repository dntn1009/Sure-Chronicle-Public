using DefineHelper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUpgradePanel : MonoBehaviour
{
    [SerializeField] CardUpgradeCostType _upgradeType;
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] Button _upgradeBtn;
    [SerializeField] Button _cancelBtn;

    HeroCardInfo _heroCardInfo;
    int _pay;

    bool _isUpgrade;

    public void Initialize(HeroCardInfo cardInfo)
    {
        this.gameObject.SetActive(false);
        _isUpgrade = false;
        _upgradeBtn.onClick.AddListener(() => UpgradeCard(_heroCardInfo.SelectCard));
        _cancelBtn.onClick.AddListener(() => Cancel());
        _upgradeBtn.interactable = false;
        _heroCardInfo = cardInfo;
    }

    public void OpenPanel(Card card)
    {
        this.gameObject.SetActive(true);
        int gold = DataManager.Instance.Gold;
        switch (_upgradeType)
        {
            case CardUpgradeCostType.Level:
                if (card.MyLevel >= 30)
                {
                    _costText.text = "MAX";
                    _upgradeBtn.interactable = false;
                    return;
                }
                _pay = DataManager.Instance.PublicCost._levelUpgradeCost * card.MyLevel * ((int)card.MyCardType + 1);
                break;
            case CardUpgradeCostType.Star:
                if (card.MyStar >= 5)
                {
                    _costText.text = "MAX";
                    _upgradeBtn.interactable = false;
                    return;
                }
                _pay = DataManager.Instance.PublicCost._starUpgradeCost * card.MyStar * ((int)card.MyCardType + 1);
                break;
        }
        _costText.text = _pay.ToString("N0");
        if (gold < _pay)
            _upgradeBtn.interactable = false;
        else
            _upgradeBtn.interactable = true;
    }

    public void ClosePanel()
    {
        Cancel();
    }

    public void UpgradeCard(Card card)
    {
        AudioManager.Instance.LoadSFX("SFX/UpgradeButton.wav");
        if(!_isUpgrade)
        {
            _isUpgrade = true;

            switch (_upgradeType)
            {
                case CardUpgradeCostType.Level:
                    DataManager.Instance.LvUpCard(card, _pay);
                    break;
                case CardUpgradeCostType.Star:
                    DataManager.Instance.EnforceCardStar(card, _pay);
                    break;

            }
        }
    }

    public void Cancel()
    {
        AudioManager.Instance.LoadSFX("SFX/Button.wav");
        this.gameObject.SetActive(false);
        _upgradeBtn.interactable = false;
        _isUpgrade = false;
    }
}
