using DefineHelper;
using DesignPattern;
using fsmStatePattern;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
public class HeroController : PlayerObj
{

    [Header("Hero Var")]

    [Header("Index & Follow")]
    [SerializeField] int _slotIndex;
    [SerializeField] HeroController _followHero;
    float _xzfollowPos;
    int _horizontalValue;


    [Header("Stat & AttackFind & Move")]
    [SerializeField] Transform _hitPos;
    [SerializeField] CardInfo _heroStat;
    [SerializeField] AttackAreUnitFind _atkFind;
    [SerializeField] float _movSpeed = 2f;
    [SerializeField] int _hp;
    [SerializeField] MonsterController _target;
    NavMeshAgent _navAgent;

    [Header("Sprite & Sorting Layer")]
    [SerializeField] SortingGroup _sorting;


    FSM<HeroController> _fsm;
    float _attDuration;
    Joystick joy; // JOYSTICK
    Vector3 _moveDir; //Move 방향벡터
    Vector3 _moveVec; //SLOT BY POSITION

    public Vector3 FollowDir { get { return _followHero._moveDir; } } // Slot 0 방향벡터
    public NavMeshAgent NavAgent { get { return _navAgent; } }
    public MonsterController TargetMonster { get { return _target; } set { _target = value; } }

    public CardInfo HeroStat { get { return _heroStat; } }

    public HashSet<GameObject> MonsterList { get { return _atkFind.UnitList; } }

    public bool IsDeath { get { if (CurrentState == PlayerState.death) return true; else return false; } }
    public bool _isSearch;

    bool _isWaiting; // BossSpawnWaiting
    Coroutine _respawnCoroutine;
    protected override void OnAwake()
    {
        base.OnAwake();
        Initialize();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _hp = _heroStat._hp;
        _objAttackType = _heroStat._attackType;
    }

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

        if (_fsm.CurrentState is hMoveState)
        {
            if (DataManager.Instance.IsAuto)
            {
                AutoMove();
                return;
            }

            JoyStickMove();
        }

    }

    #region [Initialize Methods]

    public void Initialize()
    {
        _isWaiting = false;
        _isSearch = false;
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false; // 필요 시 설정
        _navAgent.updateUpAxis = false;   // 2D에서 필요 시 설정
        _navAgent.autoRepath = true;
        _xzfollowPos = 0.75f;
        _horizontalValue = 1;
        _sorting = GetComponent<SortingGroup>();
        _attDuration = 0f;
        joy = UIManager.Instance.JoyStick;
        _fsm = new FSM<HeroController>(this);

        if (DataManager.Instance.IsAuto)
            _fsm.ChangeState(new hSearchState());
        else
            _fsm.ChangeState(new hIdleState());
    }

    public void SetSpawn(int SlotIndex, CardInfo Info, HeroController followHero = null)
    {//SlotIndex 세팅 => Hero 위치 결정에 사용
        //
        _slotIndex = SlotIndex;
        _heroStat = Info;
        _hp = _heroStat._hp;
        _followHero = followHero;
        _atkFind.Initialize(this);
    }
    public void ChangeFollowHero(HeroController _herocontroller)
    {
        _followHero = _herocontroller;
    }

    public void WaitingCheck(bool check)
    {
        _isWaiting = check;
        
        if (check)
        {
            _target = null;
            if (DataManager.Instance.IsAuto)
                ChangeAnyFromState(new hSearchState());
            else
                ChangeAnyFromState(new hIdleState());

            _atkFind.UnitListClear();
        }

    }

    #endregion [Initialize Methods]

    #region [Idle & Search Methods]
    public void DoIdle()
    {
        if (_slotIndex == 0)
        {
            joyStickDir();

            if (_moveDir != Vector3.zero)
            {
                Debug.Log(_moveDir);
                ChangeAnyFromState(new hMoveState());
                return;
            }

            if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
            {
                ChangeAnyFromState(new hAttackState());
                Debug.Log("Att 체크");
            }
            return;
        }

        if (_followHero != null && _followHero._moveDir != Vector3.zero)
        {
            ChangeAnyFromState(new hMoveState());
            return;
        }
        _moveVec = CalculateSlotPosition();
        if (Vector3.Distance(transform.position, _moveVec) > 0.1f)
        {
            if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
            {
                ChangeAnyFromState(new hAttackState());
                return;
            }

            ChangeAnyFromState(new hMoveState());
        }
    } // Change From MoveState

    public void DoSearch()
    {
        if (!_isSearch)
            return;

        if (_slotIndex == 0)
        {
            if (_target != null)
            {
                if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
                {
                    ChangeAnyFromState(new hAttackState());
                    return;
                }
                ChangeAnyFromState(new hMoveState());
                return;
            }

            SelectClosestTarget();

            if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
                ChangeAnyFromState(new hAttackState());
            else
                ChangeAnyFromState(new hMoveState());

            return;
        }

        if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck() && Vector3.Distance(transform.position, _moveVec) <= 2f)
        {
            ChangeAnyFromState(new hAttackState());
            return;
        }

        if (Vector3.Distance(transform.position, CalculateSlotPosition()) <= 0.1f)
            return;

        if (_followHero != null && _followHero.TargetMonster != null)
            ChangeAnyFromState(new hMoveState());
    }

    public void startSearchCoolTime(float seconds)
    {
        StartCoroutine(SearchCoolTime(seconds));
    }

    IEnumerator SearchCoolTime(float seconds)
    {
        NavAgent.ResetPath();
        PlayStateAnimation(PlayerState.idle);
        _isSearch = false;
        yield return new WaitForSeconds(seconds);
        _isSearch = true;
    }

    #endregion [Idle & Search Methods]

    #region [Move & Horizontal Methods]
    private Vector3 CalculateSlotPosition()
    {
        if (_followHero == null)
            return this.transform.position;

        float offsetX = _horizontalValue * _xzfollowPos;
        float offsetZ = 0;

        switch (_slotIndex)
        {
            case 2: offsetZ = _xzfollowPos; break;
            case 3: offsetZ = -_xzfollowPos; break;
        }

        return new Vector3(_followHero.transform.position.x + offsetX, 0, _followHero.transform.position.z + offsetZ);
    }//Slot By Position
    public void joyStickDir()
    {
        float mz = joy.Vertical;
        float mx = joy.Horizontal;
        Vector3 dv = new Vector3(mx, 0, mz);
        _moveDir = (dv.magnitude > 1) ? dv.normalized : dv; // normalized화
    } // JoyStick Dir Vector3
    public void Move()
    {
        if (_moveDir != Vector3.zero)
            transform.Translate(_moveDir * _movSpeed * Time.fixedDeltaTime, Space.World);
    } // JoyStick value Move
    private void MoveTowardsTarget()
    {
        NavAgent.SetDestination(_moveVec);
    } // SLot 1-3 Follow Move & Change IdleState
    private void JoyStickMove()
    {
        if (_slotIndex == 0)
        {
            Move();
            return;
        }

        MoveTowardsTarget();
    }//FixedUpdate
    public void SelectClosestTarget()
    {
        // 전체 씬에서 모든 몬스터를 찾음
        MonsterController[] allMonsters = FindObjectsOfType<MonsterController>();

        MonsterController closestMonster = null;
        float closestDistance = float.MaxValue;

        // 모든 몬스터와의 거리를 계산하여 가장 가까운 몬스터를 찾음
        foreach (MonsterController monster in allMonsters)
        {
            if (monster.IsDeath)
                continue;

            float distance = Vector3.Distance(transform.position, monster.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMonster = monster;
            }
        }

        // 가장 가까운 몬스터를 _target으로 설정
        _target = closestMonster;
    }

    public void AutoMove()
    {
        if (_slotIndex == 0)
        {
            NavAgent.SetDestination(_target.transform.position);
            return;
        }
        MoveTowardsTarget();//IsAuto == True (SetDestination)
    }

    public void DoMove()
    {
        if (_slotIndex == 0)
        {
            joyStickDir();

            if (_moveDir == Vector3.zero)
                ChangeAnyFromState(new hIdleState());
            return;
        }
        // 슬롯 0이 아닌 경우 따라가기 이동 처리
        _moveVec = CalculateSlotPosition();

        if (_followHero != null && _followHero._moveDir == Vector3.zero && _atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
        {
            ChangeAnyFromState(new hIdleState());
            return;
        }

        if (Vector3.Distance(transform.position, _moveVec) <= 0.1f)
            ChangeAnyFromState(new hIdleState());

    } // Slot 0 = JoyStick Move Else Slot0 Follow

    public void DoAutoMove()
    {
        if (_slotIndex == 0)
        {
            if (_target == null || _target.IsDeath)// || _target.IsDeath == true
            {
                _target = null;
                ChangeAnyFromState(new hSearchState());
                return;
            }

            if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck())
                ChangeAnyFromState(new hSearchState());

            _moveDir = (_target.transform.position - transform.position).normalized;
            return;
        }

        _moveVec = CalculateSlotPosition();
        if (_atkFind.UnitList.Count > 0 && !_atkFind.RangeIsDeathCehck() && Vector3.Distance(transform.position, _moveVec) <= 2f)
        {
            ChangeAnyFromState(new hSearchState());
            return;
        }

        if (Vector3.Distance(transform.position, _moveVec) <= 0.1f)
        {
            ChangeAnyFromState(new hSearchState());
            return;
        }
    }
    public void HorizontalFacing(Vector3 dir)
    {
        if (dir.x > 0 && _horizontalValue == 1)
        {
            _prefabs.transform.localScale = new Vector3(-1, 1, 1);
            _horizontalValue = -1;
        }
        else if (dir.x < 0 && _horizontalValue == -1)
        {
            _prefabs.transform.localScale = new Vector3(1, 1, 1);
            _horizontalValue = 1;
        }
    } // Dir Value Scale Change Left - Right
    public void DoHorizontal()
    {
        if (_slotIndex == 0)
        {
            HorizontalFacing(_moveDir);
            return;
        }

        HorizontalFacing(_followHero.FollowDir);
    } // Slot By HorizontalFacing

    #endregion [Move & Horizontal Methods]

    #region [Attack & Damage Methods]

    public void DoAttack()
    {
        if (CurrentState == PlayerState.attack)
            return;

        if (DataManager.Instance.IsAuto)
        {
            if (_slotIndex == 0)
            {
                if (_atkFind.UnitList.Count == 0 || _atkFind.RangeIsDeathCehck())
                {
                    ChangeAnyFromState(new hSearchState());
                    return;
                }

                if (TargetMonster == null || TargetMonster.IsDeath)// TargetMonster.IsDeath == true
                {
                    TargetMonster = null;
                    ChangeAnyFromState(new hSearchState());
                    return;
                }
            }

            _moveVec = CalculateSlotPosition();

            if (_atkFind.UnitList.Count == 0 || _atkFind.RangeIsDeathCehck() || Vector3.Distance(transform.position, _moveVec) > 2f)
            {
                ChangeAnyFromState(new hSearchState());
                return;
            }
        }

        if (_slotIndex == 0)
        {
            joyStickDir();
            if (_atkFind.UnitList.Count == 0 || _atkFind.RangeIsDeathCehck() || _moveDir != Vector3.zero)
                ChangeAnyFromState(new hIdleState());

            SetAttackCoolTime();
            return;
        }

        if (_atkFind.UnitList.Count == 0 || _atkFind.RangeIsDeathCehck() || _followHero._moveDir != Vector3.zero)
            ChangeAnyFromState(new hIdleState());

        SetAttackCoolTime();
    }

    public void SetAttackCoolTime()
    {
        _attDuration += Time.deltaTime;

        if (_attDuration >= _heroStat._attSpeed)
        {
            PlayStateAnimation(PlayerState.attack);
            _attDuration = 0f;
        }
    }

    public void EnterAttack()
    {
        _attDuration = _heroStat._attSpeed;
    }

    public void ExitAttack()
    {
        _attDuration = 0f;
    }

    public void SetDamage(MonsterData attacker)
    {
        if (IsDeath)
            return;

        // 크리티컬 여부 판별
        bool isCritical = Random.value < attacker._critChance;

        // 최종 데미지 계산
        float damage = DamageCalculator.CalculateDamage(attacker, _heroStat, isCritical);

        _hp -= Mathf.CeilToInt(damage);
        UIManager.Instance.CreateDemageFont((int)damage, isCritical, _hitPos.position);

        if (_hp <= 0)
        {
            _hp = 0;
            PlayStateAnimation(PlayerState.death);
            _respawnCoroutine = StartCoroutine(RespawnAfter(5f));
        }
    }



    public override void AnimEvent_Attack()
    {
        base.AnimEvent_Attack();
        var UnitList = _atkFind.UnitList;

        foreach (GameObject obj in UnitList)
        {
            var mon = obj.GetComponent<MonsterController>();
            if (mon != null)
                mon.SetDamage(this); // 공격자의 스탯을 넘겨줌
        }
    }

    public override void AnimEvent_AttackFinished()
    {
        PlayStateAnimation(PlayerState.idle);
    }

    public void Heal(float amount)
    {
        var heal = Mathf.CeilToInt(amount); // 정수로 변환하여 체력 회복
        if (_hp > _heroStat._hp + heal)
            _hp = _heroStat._hp;
        else
            _hp += heal;

        UIManager.Instance.CreateHealFont((int)amount, _hitPos.position); // UI에 회복 이펙트 표시
                                                                          // 최대 체력 초과 방지
    }//몬스터 피격시 체력 회복

    private IEnumerator RespawnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }// Boss - 모두 안죽으면 몇 초 뒤 리스폰, Normal - 

    public void Respawn()
    {
        _hp = _heroStat._hp;
        if (DataManager.Instance.IsAuto)
            ChangeAnyFromState(new hSearchState());
        else
            ChangeAnyFromState(new hIdleState());

    }

    public void StopRespawn()
    {
        StopCoroutine(_respawnCoroutine);
        if (CurrentState != PlayerState.death)
            PlayStateAnimation(PlayerState.death);
    }//모두 죽으면 대기 ( 보스전에서만)

    public void ChapterSpawn(Transform transform = null)
    {
        if(transform == null)
            this.gameObject.transform.position = Vector3.zero;
        else
            this.gameObject.transform.position = transform.position;

        Respawn();
        WaitingCheck(true);
    }

    #endregion [Attack & Damage Methods]

    public void ChangeAnyFromState(IState<HeroController> state)
    {
        _fsm.ChangeState(state);
    }
}
