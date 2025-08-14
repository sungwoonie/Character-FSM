using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterChaseState : ChaseState
{
    private EnemyCharacter enemyCharacter;

    public override IEnumerator ChaseCoroutine()
    {
        if(characterFiniteStateMachine.ChaseTarget != null)
        {
            enemyCharacter = characterFiniteStateMachine.ChaseTarget.GetComponent<EnemyCharacter>();
        }

        return base.ChaseCoroutine();
    }

    public override bool CanChase()
    {
        return base.CanChase() && enemyCharacter.IsAlive();
    }
    public override float ChaseDistance() => 0.5f;

    public override float ChaseSpeed() => 2.0f;
}