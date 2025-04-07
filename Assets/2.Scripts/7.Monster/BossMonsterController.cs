using DefineHelper;
using fsmStatePattern;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterController : MonsterController
{
    bool _isWaiting;

    [SerializeField] bool _isJewel;
    private void Update()
    {
        if (IsDeath || _isWaiting)
            return;

        _fsm.Update();
    }

    private void FixedUpdate()
    {
        if (IsDeath || _isWaiting)
            return;
    }

    #region [Monster Spawn & Initialize Methods]

    public override void InitializeSet()
    {
        base.InitializeSet();
        _isWaiting = true;
        _fsm = new FSM<MonsterController>(this);
        _fsm.ChangeState(new mIdleState());

        Debug.Log("보스 스텟" + _monStat._hp);
    }
    public void WaitingCheck(bool check)
    {
        _isWaiting = check;
    }

    public override void SetDeSpawn()
    {
        HeroGetGold();
        if (_monType == MonsterType.StageBoss)
            UIManager.Instance.ChapterWindow.OpenStageResult(true);
        else
            UIManager.Instance.ChapterWindow.OpenRaidResult(_isJewel, true, _monStat._gold);

        DataManager.Instance.SaveDataAndUINotifyObservers();
        DataManager.Instance.GetHeroManager().SetWaitingHeros(true);
        //다음 챕터를 위한 UI 보여주기
    }

    #endregion [Monster Spawn Methods]

    #region [Monster Idle Methods]

    public override void DoIdle()
    {
        if (_targetHero == null)
        {
            SelectClosestTarget();
        }
        else
        {
            ChangeAnyFromState(new mChaseState());
        }
    }

    public override void EnterIdle(float min, float max)
    {
        PlayStateAnimation(PlayerState.idle);
        StartCoroutine(IsIdleCheck());
    }

    public override void ExitIdle()
    {

    }
    public IEnumerator IsIdleCheck()
    {
        yield return new WaitUntil(() => _isWaiting);
        SelectClosestTarget();
    }
    public void SelectClosestTarget()
    {
        // 전체 씬에서 모든 몬스터를 찾음
        HeroController[] allHeros = FindObjectsOfType<HeroController>();
        HeroController closestHero = null;
        float closestDistance = float.MaxValue;

        // 모든 몬스터와의 거리를 계산하여 가장 가까운 몬스터를 찾음
        foreach (HeroController hero in allHeros)
        {
            if (hero.IsDeath)
                continue;

            float distance = Vector3.Distance(transform.position, hero.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHero = hero;
            }
        }

        if(closestHero == null)
        {
            DataManager.Instance.GetHeroManager().HerosAllDeath();
            if (_monType == MonsterType.StageBoss)
                UIManager.Instance.ChapterWindow.OpenStageResult(false);
            else
                UIManager.Instance.ChapterWindow.OpenRaidResult(_isJewel, false);
        }

        // 가장 가까운 몬스터를 _target으로 설정
        _targetHero = closestHero.gameObject;
    }

    #endregion [Monster Idle Methods]

    #region [Monster Chase Methods]

    public override void DoChase()
    {
        if(TargetHeroController.IsDeath || _targetHero == null)
        {
            _targetHero = null;
            ChangeAnyFromState(new mIdleState());
            Debug.Log("!11!");
        }

        if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
        {
            ChangeAnyFromState(new mAttackState());
            Debug.Log("!22!");
            return;
        }

        ChaseInNavMesh();
    }
    #endregion [Monster Chase Methods]

    #region [Monster Attack Methods]

    public override void DoAttack()
    {
        if (CurrentState == PlayerState.attack)
            return;

        if(_atkFind.UnitList.Count == 0)
        {
            ChangeAnyFromState(new mChaseState());
            return;
        }

        if (_atkFind.UnitList.Count == 0 || _atkFind.RangeIsDeathCehck())
        {
            _targetHero = null;
            ChangeAnyFromState(new mIdleState());
            return;
        }

        SetAttackCoolTime();
    }

    #endregion [Monster Attack Methods]

    public override void HeroGetGold()
    {
        var gold = _monStat._gold;
        if (_isJewel)
            DataManager.Instance.Jewel += _monStat._gold;
        else
            DataManager.Instance.Gold += _monStat._gold;

        UIManager.Instance.CreateGoldFont(gold, _HitPos.position, _isJewel);
    }
}
