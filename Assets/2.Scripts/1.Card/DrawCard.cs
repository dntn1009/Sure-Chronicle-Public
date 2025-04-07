using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DefineHelper;
using TMPro;
using UnityEngine.UI;
using System;

public class DrawCard : MonoBehaviour, IPointerClickHandler
{
    [Header("Hero UI")]
    [SerializeField] Image _background;
    [SerializeField] Image _heroImg;
    [SerializeField] TextMeshProUGUI _heroName;
    [SerializeField] TextMeshProUGUI _heroType;
    [SerializeField] Color[] _typeColors;
    [Header("OPEN ANIMATION")]
    [SerializeField] Animator _animCtr;
    [SerializeField] TextMeshProUGUI _openText;

    bool _isOpen = false;
    public event Action<DrawCard> OnCardOpened;
    public bool IsOpen { get { return _isOpen; } }

    public DrawCard Initialize(CardData cardData)
    {
        _background.color = _typeColors[(int)cardData._cardType];
        DataManager.Instance.LoadAddressable<Sprite>(cardData._heroSprite, (sprite) =>
        {
            _heroImg.sprite = sprite;
        });
        _heroName.text = cardData._name;
        _heroType.text = cardData._cardType.ToString();
        return this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenCard();
    }

    public void OpenCard()
    {
        if(!_isOpen)
        {
            AudioManager.Instance.LoadSFX("SFX/OpenCard.wav");
            _isOpen = true;
            _openText.gameObject.SetActive(false);
            _animCtr.SetTrigger("OPEN");
            OnCardOpened?.Invoke(this);
        }
    }
}
