using DefineHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardInfo
{
    public int _hp;
    public int _att;
    public int _def;
    public float _attSpeed;
    public float _critChance;
    public float _critDamage;
    public float _LifeSteal;
    public AttackType _attackType;

    public CardInfo(CardInfo info, SharedEnforceData enforce, UserInfo uinfo)
    {
        // �⺻ ���� ����
        _hp = info._hp;
        _attSpeed = info._attSpeed;
        _critChance = info._critChance;
        _critDamage = info._critDamage;
        _LifeSteal = info._LifeSteal;
        _attackType = info._attackType;

        // ���ݷ� �� ���� ��ȭ ��ġ ����
        float attMultiplier = GetEnforceMultiplier(enforce._attLv);
        float defMultiplier = GetEnforceMultiplier(enforce._defLv);
        float hpMultiplier = GetEnforceMultiplier(enforce._hpLv);
        _att = Mathf.RoundToInt(info._att * (1 + attMultiplier));
        _def = Mathf.RoundToInt(info._def * (1 + defMultiplier));
        _hp = Mathf.RoundToInt(info._hp * (1 + hpMultiplier));
        // ���� �� ��Ÿ ���� ���� (�⺻������ 1.0 �������� ����)
        float levelMultiplier = 1.0f + (uinfo._level - 1) * 0.1f; // ���� 10% �� 5%�� ���� (�뷱�� ����)
        float starMultiplier = 1.0f + (uinfo._star - 1) * 0.5f; // ���� 50% �� 30%�� ����

        // ���� ���� ����
        float totalMultiplier = levelMultiplier * starMultiplier;
        _hp = Mathf.RoundToInt(_hp * totalMultiplier);
        _att = Mathf.RoundToInt(_att * totalMultiplier);
        _def = Mathf.RoundToInt(_def * totalMultiplier);

        // ũ��Ƽ�� ���� ������ ������ ����
        _critChance *= totalMultiplier;
        _critDamage *= totalMultiplier;
        _LifeSteal *= totalMultiplier;
    }

    private static float GetEnforceMultiplier(int level)
    {
        return level * 0.1f;  // ���� 10%(��ȭ ������ �ʹ� ���� �� ����)
    }

}
