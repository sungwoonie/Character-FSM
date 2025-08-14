using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : ChaseState
{
    public bool rangeAttack;

    private PlayableCharacter playableCharacter;

    public override IEnumerator ChaseCoroutine()
    {
        if(playableCharacter == null)
        {
            playableCharacter = PlayableCharacter.instance;
        }

        return base.ChaseCoroutine();
    }

    public override float ChaseDistance() => rangeAttack ? 3f : 1f;

    public override float ChaseSpeed() => characterFiniteStateMachine.CanMove ? 0.3f : 0.0f;

    public override bool CanChase()
    {
        return characterFiniteStateMachine.ChaseTarget != null && characterFiniteStateMachine.ChaseTarget.gameObject.activeSelf && playableCharacter.IsAlive();
    }
}