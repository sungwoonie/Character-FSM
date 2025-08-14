using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IdleState
{
    private PlayableCharacter playableCharacter;

    public override bool ExistTarget()
    {
        if(playableCharacter == null)
        {
            playableCharacter = PlayableCharacter.instance;
        }

        return playableCharacter != null && playableCharacter.gameObject.activeSelf && playableCharacter.IsAlive();
    }

    public override void SetChaseTarget()
    {
        characterFiniteStateMachine.ChaseTarget = playableCharacter.transform;
    }
}