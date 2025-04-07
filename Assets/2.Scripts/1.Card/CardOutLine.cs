using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOutLine : MonoBehaviour
{
    [Header("UI Var")]
    [SerializeField] Button _equipBtn;
    [SerializeField] Button _infoBtn;
    [SerializeField] TextMeshProUGUI _equipBtnText;
    [Header("Parent Card")]
    [SerializeField] Card _card;

    private void OnEnable()
    {
        _equipBtn.onClick.RemoveAllListeners();
        _infoBtn.onClick.RemoveAllListeners();
        _equipBtn.onClick.AddListener(Equip);
        _infoBtn.onClick.AddListener(Info);
    }

    public void SlotByBtnLock()
    {
        if (_card.EquipIndex == -1)
        {
            _equipBtn.interactable = true;
            _equipBtnText.text = "¿Â¬¯";
        }
        else if(_card.EquipIndex == 0)
        {
            _equipBtn.interactable = false;
            _equipBtnText.text = "¿Â¬¯";
        }
        else
        {
            _equipBtnText.text = "«ÿ¡¶";
        }
    }

    public void Equip()
    {
        HeroWindow hero = UIManager.Instance.GetHeroWindow();
        hero.SetEquipClickCard(_card);
    }
    public void Info()
    {
        HeroWindow hero = UIManager.Instance.GetHeroWindow();
        hero.SelectCardInfo(_card);
    }
}
