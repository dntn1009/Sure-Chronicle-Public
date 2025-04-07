using DefineHelper;
using DesignPattern;
using UnityEngine;

namespace fsmStatePattern
{
    public class FSM<T>
    {
        private T _owner;
        private IState<T> _currentState;


        public IState<T> CurrentState { get { return _currentState; } }

        public FSM(T owner)
        {
            _owner = owner;
        }

        public void ChangeState(IState<T> newState)
        {
            _currentState?.ExitState(_owner);
            _currentState = newState;
            _currentState?.EnterState(_owner);
        }

        public void Update() => _currentState?.UpdateState(_owner);
    }

    #region [Monster FSM State Pattern]

    public class mIdleState : IState<MonsterController>
    {

        public void EnterState(MonsterController monster)
        {
            monster.EnterIdle(2f, 4f);
        }


        public void ExitState(MonsterController monster)
        {
            monster.ExitIdle();
        }

        public void UpdateState(MonsterController monster)
        {
            monster.DoIdle();
        }
    }
    public class mMoveState : IState<MonsterController>
    {
        public void EnterState(MonsterController monster)
        {
            monster.EnterMove(5f, 8f);
        }

        public void ExitState(MonsterController monster)
        {
            monster.ExitMove();
        }

        public void UpdateState(MonsterController monster)
        {
            monster.DoMove();
        }
    }
    public class mChaseState : IState<MonsterController>
    {

        public void EnterState(MonsterController monster)
        {
            monster.PlayStateAnimation(PlayerState.run);
        }

        public void ExitState(MonsterController monster)
        {
            monster.NavAgent.ResetPath();
        }

        public void UpdateState(MonsterController monster)
        {
            monster.DoChase();
        }
    }
    public class mAttackState : IState<MonsterController>
    {

        public void EnterState(MonsterController monster)
        {
            monster.EnterAttack();
        }

        public void ExitState(MonsterController monster)
        {
            monster.ExitAttack();
        }

        public void UpdateState(MonsterController monster)
        {
            monster.DoAttack();
        }
    }

    #endregion [Monster FSM State Pattern]

    #region [Hero FSM State Pattern]

    public class hIdleState : IState<HeroController>
    {
        public void EnterState(HeroController obj)
        {
            obj.NavAgent.ResetPath();
            obj.PlayStateAnimation(PlayerState.idle);
        }

        public void ExitState(HeroController obj)
        {

        }

        public void UpdateState(HeroController obj)
        {
            obj.DoIdle();
        }
    }


    public class hSearchState : IState<HeroController>
    {
        public void EnterState(HeroController obj)
        {
            obj.startSearchCoolTime(1f);
        }

        public void ExitState(HeroController obj)
        {
        }

        public void UpdateState(HeroController obj)
        {
            obj.DoSearch();
        }
        public void FixedUpdateState(HeroController obj)
        {

        }
    }

    public class hMoveState : IState<HeroController>
    {

        public void EnterState(HeroController obj)
        {
            obj.PlayStateAnimation(PlayerState.run);
        }

        public void ExitState(HeroController obj)
        {
            obj.NavAgent.ResetPath();
        }

        public void UpdateState(HeroController obj)
        {
            if (DataManager.Instance.IsAuto)
                obj.DoAutoMove();
            else
                obj.DoMove();
            obj.DoHorizontal();
        }

    }

    public class hAttackState : IState<HeroController>
    {
        public void EnterState(HeroController obj)
        {
            obj.NavAgent.ResetPath();
            obj.EnterAttack();
        }

        public void ExitState(HeroController obj)
        {
            obj.ExitAttack();
        }

        public void UpdateState(HeroController obj)
        {
            obj.DoAttack();
        }
    }
    #endregion [Hero FSM State Pattern]

}
