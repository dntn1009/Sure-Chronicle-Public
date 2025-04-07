using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility
{
    public static class DamageCalculator //  ���� Ŭ���� �߰�
    {
        public static int CalculateDamage(MonsterData attacker, CardInfo defender, bool isCritical) //  static �޼���
        {
            float baseDamage = attacker._att * (100f / (100f + defender._def));

            float criticalDamage = baseDamage * attacker._critDamage;

            float finalDamage = isCritical ? Mathf.Max(baseDamage, criticalDamage) : baseDamage;

            return Mathf.RoundToInt(finalDamage);
        }

        public static int CalculateDamage(CardInfo attacker, MonsterData defender, bool isCritical)
        {
            // �⺻ ������ ���
            float baseDamage = attacker._att * (100f / (100f + defender._def));

            // ũ��Ƽ�� ������ ���
            float criticalDamage = baseDamage * attacker._critDamage;

            // ���� ������: ũ��Ƽ���� ������ ���� ���������� �۴ٸ� ���� �������� ����
            float finalDamage = isCritical ? Mathf.Max(baseDamage, criticalDamage) : baseDamage;

            return Mathf.RoundToInt(finalDamage);
        }
    }

}
