using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

[CreateAssetMenu(menuName = "Scriptable Object/BoxData")]
public class BoxData : ScriptableObject
{
    public BoxType _boxType;
    public string _title;
    public Sprite _boxSprite;
    public Sprite _paySprite;
    public int _pay;
    public string _btnStr;
    public Color _imgColor;

    [Header("Draw Card")]
    public int _count;
    public bool _isSpecial;
    [Header("ExChange Gold")]
    public int _gold;
    [Header("Exchange Jewel")]
    public int _jewel;
}
