using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipCard : MonoBehaviour
{
    [Header("UI Var")]
    [SerializeField] Image _backgroundImg;
    [SerializeField] Image _heroImg;
    [SerializeField] TextMeshProUGUI _nameText;

    
    public EquipCard Initialize(Card card)
    {
        _backgroundImg.color = card.TypeColor;
        DataManager.Instance.LoadAddressable<Sprite>(card.CardImg, (sprite) =>
        {
            _heroImg.sprite = sprite;
        });
        _nameText.text = card.CardName;
        return this;
    }
}
