using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerObj _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<PlayerObj>();
    }

    public void AnimEvent_Attack()
    {
        _owner.AnimEvent_Attack();
    }

    public void AnimEvent_AttackFinished()
    {
        _owner.AnimEvent_AttackFinished();
    }
}
