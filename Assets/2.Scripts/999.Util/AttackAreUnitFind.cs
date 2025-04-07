using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackAreUnitFind : MonoBehaviour
{
    private HashSet<GameObject> _unitSet = new HashSet<GameObject>();
    public HashSet<GameObject> UnitList => _unitSet;

    private PlayerObj _owner; // ���� Ŭ���� ����

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidTarget(other))
        {
            _unitSet.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidTarget(other))
        {
            if(_unitSet.Contains(other.gameObject))
                _unitSet.Remove(other.gameObject);
        }
    }

    public void Initialize(PlayerObj owner)
    {
        _owner = owner;
    }

    private bool IsValidTarget(Collider other)
    {
        // _owner�� PlayerController�� ��� Monster �±׸� ��ȿ
        if (_owner is HeroController && other.CompareTag("Monster"))
        {
            return true;
        }

        // _owner�� MonsterController�� ��� Player �±׸� ��ȿ
        if (_owner is MonsterController && other.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    public bool RangeIsDeathCehck()
    {
        bool _isCheck = true;

        foreach(GameObject obj in UnitList.ToList())
        {
            if(_owner is HeroController)
            {
                if (obj == null)
                {
                    UnitList.Remove(obj);
                    continue;
                }

                var mon = obj.GetComponent<MonsterController>();
                if (!mon.IsDeath)
                    _isCheck = false;
            }
            else if(_owner is MonsterController)
            {
                var hero = obj.GetComponent<HeroController>();
                if (!hero.IsDeath)
                    _isCheck = false;
            }
        }

        return _isCheck;
    }


    public void UnitListClear()
    {
        UnitList.Clear();
    }
}
