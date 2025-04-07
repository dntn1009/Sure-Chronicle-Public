using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility
{
    public static class DamageCalculator //  정적 클래스 추가
    {
        public static int CalculateDamage(MonsterData attacker, CardInfo defender, bool isCritical) //  static 메서드
        {
            float baseDamage = attacker._att * (100f / (100f + defender._def));

            float criticalDamage = baseDamage * attacker._critDamage;

            float finalDamage = isCritical ? Mathf.Max(baseDamage, criticalDamage) : baseDamage;

            return Mathf.RoundToInt(finalDamage);
        }

        public static int CalculateDamage(CardInfo attacker, MonsterData defender, bool isCritical)
        {
            // 기본 데미지 계산
            float baseDamage = attacker._att * (100f / (100f + defender._def));

            // 크리티컬 데미지 계산
            float criticalDamage = baseDamage * attacker._critDamage;

            // 최종 데미지: 크리티컬이 터져도 원래 데미지보다 작다면 원래 데미지를 유지
            float finalDamage = isCritical ? Mathf.Max(baseDamage, criticalDamage) : baseDamage;

            return Mathf.RoundToInt(finalDamage);
        }
    }

}
