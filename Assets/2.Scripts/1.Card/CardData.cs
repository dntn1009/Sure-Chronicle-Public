using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

[CreateAssetMenu(menuName = "Scriptable Object/CardData")]
public class CardData : ScriptableObject
{
    public CardType _cardType;
    public string _heroSprite;
    public string _heroPrefab;
    public CardInfo _info;
    public string _name;
    public string _explane;

    [Header("cardCode")]
    public int _cardCode;
}
