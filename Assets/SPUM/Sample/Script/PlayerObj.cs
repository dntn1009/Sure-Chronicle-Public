using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DefineHelper;

[System.Serializable]
public class MyEvent : UnityEvent<PlayerState>
{

}

[ExecuteInEditMode]
public class PlayerObj : MonoBehaviour
{
    [Header("PlayerObj Var")]
    public SPUM_Prefabs _prefabs;
    public float _charMS;
    private PlayerState _currentState;
    public PlayerState CurrentState{
        get => _currentState;
        set {
            _stateChanged.Invoke(value);
            _currentState = value;
        }
    }

    public AttackType _objAttackType;

    private MyEvent _stateChanged = new MyEvent();

    //public Vector3 _goalPos;
    // Start is called before the first frame update

    // Update is called once per frame

    void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        OnStart();
    }
    void Update()
    {
        /*transform.position = new Vector3(transform.position.x,transform.position.y,transform.localPosition.y * 0.01f);
        switch(_currentState)
        {
            case PlayerState.idle:
            break;

            case PlayerState.run:
            DoMove();
            break;
        }*/
    }

    virtual protected void OnStart()
    {
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
        }

        _stateChanged.AddListener(PlayStateAnimation);
    }

    virtual protected void OnAwake()
    {

    }

    public void PlayStateAnimation(PlayerState state)
    {
        _currentState = state;
        string animationName = state.ToString();
        if (state == PlayerState.attack || state == PlayerState.skill)
        {
            animationName += $"_{_objAttackType}";
        }

        _prefabs.PlayAnimation(animationName);
    }

    public void HeroStateAnimation(PlayerState state)
    {
        _currentState = state;
        string animationName = state.ToString();


        _prefabs.PlayAnimation(animationName);
    }

    public virtual void AnimEvent_Attack()
    {
        AttackSoundToSkillType();
    }

    public virtual void AnimEvent_AttackFinished()
    {

    }

    public void AttackSoundToSkillType()
    {
        switch(_objAttackType)
        {
            case AttackType.Bow:
                AudioManager.Instance.LoadSFX("SFX/ArrowAttack.wav");
                break;
            case AttackType.Normal:
                AudioManager.Instance.LoadSFX("SFX/SlashAttack.wav");
                break;
            case AttackType.Magic:
                AudioManager.Instance.LoadSFX("SFX/MagicAttack.wav");
                break;
        }
    }

    /*void DoMove()
    {
        Vector3 _dirVec  = _goalPos - transform.position ;
        Vector3 _disVec = (Vector2)_goalPos - (Vector2)transform.position ;
        if( _disVec.sqrMagnitude < 0.1f )
        {
            _currentState = PlayerState.idle;
            PlayStateAnimation(_currentState);
            return;
        }
        Vector3 _dirMVec = _dirVec.normalized;
        transform.position += (_dirMVec * _charMS * Time.deltaTime );
        

        if(_dirMVec.x > 0 ) _prefabs.transform.localScale = new Vector3(-1,1,1);
        else if (_dirMVec.x < 0) _prefabs.transform.localScale = new Vector3(1,1,1);
    }*/

    /*public void SetMovePos(Vector2 pos)
    {
        _goalPos = pos;
        _currentState = PlayerState.run;
        PlayStateAnimation(_currentState);
    }*/
}
