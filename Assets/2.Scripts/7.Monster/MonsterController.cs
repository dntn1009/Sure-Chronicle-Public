using DefineHelper;
using DesignPattern;
using fsmStatePattern;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterController : PlayerObj
{
    [Header("Monster Data & AttackFind")]
    [SerializeField] protected MonsterType _monType;
    [SerializeField] protected MonsterData _monStat;
    [SerializeField] protected AttackAreUnitFind _atkFind;
    [SerializeField] protected Transform _HitPos;

    //Move & Target
    protected NavMeshAgent _navAgent;
    protected GameObject _targetHero;
    protected Vector3 _lastTargetPosition;

    //Spawn Position - Round
    int _myNumber;
    MonsterSpawn _mySpawn;
    protected float _moveRadius;
    protected float _attDuration;
    bool _isIdle;
    bool _isMove;
    //FSM STATE
    protected FSM<MonsterController> _fsm;

    //ProPerty
    public MonsterSpawn MySpawn { get { return _mySpawn; } }
    public GameObject TargetHero { get { return _targetHero; } set { _targetHero = value; } }
    public int MyNumber { get { return _myNumber; } }
    public NavMeshAgent NavAgent { get { return _navAgent; } }
    public float MoveRadius { get { return _moveRadius; } }

    public HashSet<GameObject> HeroList { get { return _atkFind.UnitList; } }
    public HeroController TargetHeroController { get { return _targetHero.GetComponent<HeroController>(); } }

    public bool IsDeath { get { if (CurrentState == PlayerState.death) return true; else return false; } }

    bool _isRemove;

    Coroutine _removeCoroutine;
    private void Update()
    {
        if (MonsterManager.Instance._IsBoss && !_isRemove)
        {
            _isRemove = true;
            MonsterManager.Instance.RemoveMonster(this, true);
            return;
        }

        if (IsDeath)
            return;

        _fsm.Update();
    }

    private void FixedUpdate()
    {
        if (IsDeath)
            return;

    }

    #region [Initialize & State Methods]
    public void ChangeAnyFromState(IState<MonsterController> state)
    {
        _fsm.ChangeState(state);
    }

    public virtual void InitializeSet()
    {
        _isIdle = false;
        _isMove = false;
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false; // 필요 시 설정
        _navAgent.updateUpAxis = false;   // 2D에서 필요 시 설정
        _navAgent.autoRepath = true;
        _atkFind.Initialize(this);
        _monStat = DataManager.Instance.SetMonsterStat(_monType);
    }
    #endregion [Initialize & State Methods]

    #region [Monster Spawn Methods]

    public void GetSpawn(MonsterSpawn spawnPos, float radius, int number)
    {
        _isRemove = false;
        _mySpawn = spawnPos;
        _moveRadius = radius;
        _myNumber = number;
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Vector3 spawnPosition = new Vector3(_mySpawn.transform.position.x + randomPoint.x, 0, _mySpawn.transform.position.z + randomPoint.y);
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        _fsm = new FSM<MonsterController>(this);
        _fsm.ChangeState(new mIdleState()); // 초기 상태 설정
    }

    public virtual void SetDeSpawn()
    {
        DataManager.Instance.StageData._killNumber++;
        HeroGetGold();
        DataManager.Instance.SaveDataAndUINotifyObservers();
        _removeCoroutine = StartCoroutine(MonsterManager.Instance.RemoveMonsterDelay(this));
        UIManager.Instance.BOSSSpawn.killCountIncrease();
    }

    public void StopRemoveCoroutine()
    {
        if (_removeCoroutine != null)
            StopCoroutine(_removeCoroutine);
    }

    #endregion [Monster Spawn Methods]

    #region [Monster Idle Methods]

    public virtual void DoIdle()
    {
        if (SelectTarget())
        {
            ChangeAnyFromState(new mChaseState());
            return;
        }

        if (_isIdle)
        {
            ChangeAnyFromState(new mMoveState());
        }
    }

    public IEnumerator IsIdleCheck(float min, float max)
    {
        _isIdle = false;
        float wait = Random.Range(min, max);
        yield return new WaitForSeconds(wait);
        _isIdle = true;
    }

    public virtual void EnterIdle(float min, float max)
    {
        PlayStateAnimation(PlayerState.idle);
        StartCoroutine(IsIdleCheck(min, max));
    }

    public virtual void ExitIdle()
    {
        _isIdle = false;
    }
    #endregion [Monster Idle Methods]

    #region [Monster Move Methods]

    public void DoMove()
    {
        if (SelectTarget())
        {
            ChangeAnyFromState(new mChaseState());
            return;
        }

        // Move 지속 시간이 지나면 Idle 상태로 전환
        if (_isMove)
        {
            ChangeAnyFromState(new mIdleState());
            return;
        }

        // NavMeshAgent가 목적지에 도달했으면 다시 랜덤 이동
        if (!NavAgent.hasPath)
        {
            RandomMoveInNavMesh();
        }
    }

    public void EnterMove(float min, float max)
    {
        _isMove = false;
        StartCoroutine(IsMoveCheck(min, max));
        RandomMoveInNavMesh(); // 상태 진입 시 랜덤 이동
        PlayStateAnimation(PlayerState.run);

    }

    public void ExitMove()
    {
        _isMove = false;
        NavAgent.ResetPath();
    }

    public IEnumerator IsMoveCheck(float min, float max)
    {
        _isMove = false;
        float wait = Random.Range(min, max);
        yield return new WaitForSeconds(wait);
        _isMove = true;
    }

    public void ChaseInNavMesh()
    {
        Vector3 currentTargetPosition = _targetHero.transform.position;

        if (Vector3.Distance(currentTargetPosition, _lastTargetPosition) > 0.1f) // 위치 변화가 있을 때만
        {
            _navAgent.SetDestination(currentTargetPosition);
            _lastTargetPosition = currentTargetPosition;
        }

        Vector3 direction = NavAgent.steeringTarget - _targetHero.transform.position;
        direction.y = 0;
        HorizontalFacing(direction);
    }
    public void RandomMoveInNavMesh()
    {
        Vector3 targetPosition;

        if (RandomPointInNavMesh(_mySpawn.transform, _moveRadius, out targetPosition))
        {
            _navAgent.SetDestination(targetPosition);
            Vector3 direction = NavAgent.steeringTarget - transform.position;
            direction.y = 0;
            HorizontalFacing(direction);
        }
    }

    // NavMesh 내 랜덤 지점 선택 함수
    bool RandomPointInNavMesh(Transform center, float radius, out Vector3 result)
    {
        int maxAttempts = 10; // 최대 시도 횟수
        for (int i = 0; i < maxAttempts; i++)
        {
            // 2D 평면에서 랜덤 포인트 생성
            Vector2 randomPoint2D = Random.insideUnitCircle * radius;
            Vector3 randomPoint = new Vector3(center.position.x + randomPoint2D.x, center.position.y, center.position.z + randomPoint2D.y);

            // NavMesh에서 유효성 확인
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        // 실패 시 기본값 반환
        result = center.position;
        return false;
    }

    public void HorizontalFacing(Vector3 dir)
    {
        if (dir.x > 0)
            _prefabs.transform.localScale = new Vector3(-1, 1, 1);
        else if (dir.x < 0)
            _prefabs.transform.localScale = new Vector3(1, 1, 1);
    }

    #endregion [Monster Move Methods]

    #region [Monster Chase Methods]

    public bool SelectTarget()
    {
        bool check = false;

        if (_atkFind.UnitList.Count == 0)
            return check;

        foreach (var unit in _atkFind.UnitList)
        {
            if (!unit.GetComponent<HeroController>().IsDeath)
            {
                _targetHero = unit;
                check = true;
                break;
            }
        }

        return check;
    }

    public virtual void DoChase()
    {
        if (Vector3.Distance(MySpawn.MTRANSFORM.position, TargetHero.transform.position) > MoveRadius && !TargetHeroController.MonsterList.Contains(gameObject))
        {
            TargetHero = null;
            ChangeAnyFromState(new mIdleState());
            return;
        }

        if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
        {
            ChangeAnyFromState(new mAttackState());
            return;
        }

        ChaseInNavMesh();
    }

    #endregion [Monster Chase Methods]

    #region [Monster Attack Methods]

    public virtual void DoAttack()
    {
        if (CurrentState == PlayerState.attack)
            return;

        if (Vector3.Distance(MySpawn.MTRANSFORM.position, TargetHero.transform.position) <= MoveRadius && !_atkFind.UnitList.Contains(TargetHero))
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
    public void SetAttackCoolTime()
    {
        _attDuration += Time.deltaTime;

        if (_attDuration >= _monStat._attSpeed)
        {
            PlayStateAnimation(PlayerState.attack);
            _attDuration = 0f;
        }
    }

    public void EnterAttack()
    {
        _attDuration = _monStat._attSpeed;
    }

    public void ExitAttack()
    {
        _attDuration = 0f;
    }

    public override void AnimEvent_Attack()
    {
        var UnitList = _atkFind.UnitList;

        foreach (GameObject obj in UnitList)
        {
            var hero = obj.GetComponent<HeroController>();
            if (hero != null)
                hero.SetDamage(_monStat);
        }
    }

    public override void AnimEvent_AttackFinished()
    {
        PlayStateAnimation(PlayerState.idle);
    }

    public virtual void SetDamage(HeroController hero)
    {
        CardInfo attacker = hero.HeroStat;
        if (IsDeath)
            return;

        // 크리티컬 여부 판별
        bool isCritical = Random.value < (attacker._critChance / 100f);

        // 최종 데미지 계산
        float damage = DamageCalculator.CalculateDamage(attacker, _monStat, isCritical);
        _monStat._hp -= Mathf.CeilToInt(damage);
        UIManager.Instance.CreateDemageFont((int)damage, isCritical, _HitPos.position);

        float lifeStealAmount = damage * (attacker._LifeSteal / 100f); // 퍼센트 적용
        hero.Heal(lifeStealAmount); // HeroController에서 체력 회복 적용

        if (_monStat._hp <= 0)
        {
            _monStat._hp = 0;
            PlayStateAnimation(PlayerState.death);
            SetDeSpawn();
            DataManager.Instance.IncreaseAchieveByMonsterKill();
            _navAgent.ResetPath();
        }
    }

    public virtual void HeroGetGold()
    {
        var gold = _monStat._gold;
        DataManager.Instance.Gold += gold;
        UIManager.Instance.CreateGoldFont(gold, _HitPos.position);
    }

    public void UnitListClear()
    {
        _atkFind.UnitListClear();
    }

    #endregion [Monster Attack Methods]

    #region [OnTrigger Methods]

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }
    #endregion [OnTrigger Methods]
}
