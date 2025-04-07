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
        // 기본 스탯 복사
        _hp = info._hp;
        _attSpeed = info._attSpeed;
        _critChance = info._critChance;
        _critDamage = info._critDamage;
        _LifeSteal = info._LifeSteal;
        _attackType = info._attackType;

        // 공격력 및 방어력 강화 수치 적용
        float attMultiplier = GetEnforceMultiplier(enforce._attLv);
        float defMultiplier = GetEnforceMultiplier(enforce._defLv);
        float hpMultiplier = GetEnforceMultiplier(enforce._hpLv);
        _att = Mathf.RoundToInt(info._att * (1 + attMultiplier));
        _def = Mathf.RoundToInt(info._def * (1 + defMultiplier));
        _hp = Mathf.RoundToInt(info._hp * (1 + hpMultiplier));
        // 레벨 및 스타 배율 적용 (기본적으로 1.0 배율에서 시작)
        float levelMultiplier = 1.0f + (uinfo._level - 1) * 0.1f; // 기존 10% → 5%로 감소 (밸런스 조정)
        float starMultiplier = 1.0f + (uinfo._star - 1) * 0.5f; // 기존 50% → 30%로 조정

        // 최종 배율 적용
        float totalMultiplier = levelMultiplier * starMultiplier;
        _hp = Mathf.RoundToInt(_hp * totalMultiplier);
        _att = Mathf.RoundToInt(_att * totalMultiplier);
        _def = Mathf.RoundToInt(_def * totalMultiplier);

        // 크리티컬 관련 스탯은 곱연산 유지
        _critChance *= totalMultiplier;
        _critDamage *= totalMultiplier;
        _LifeSteal *= totalMultiplier;
    }

    private static float GetEnforceMultiplier(int level)
    {
        return level * 0.1f;  // 기존 10%(강화 스탯이 너무 높은 것 방지)
    }

}
