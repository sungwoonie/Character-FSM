using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

public class EnemyCombatState : CombatState
{
    private PlayableCharacter playableCharacter;
    private EnemyCharacter enemyCharacter;

    public override IEnumerator CombatCoroutine()
    {
        if(playableCharacter == null)
        {
            Initialize();
        }

        return base.CombatCoroutine();
    }

    private void Initialize()
    {
        playableCharacter = PlayableCharacter.instance;
        enemyCharacter = GetComponent<EnemyCharacter>();
    }

    public override float AttackDelay() => 1f;

    public override void ResetTrigger()
    {
        characterFiniteStateMachine.AnimatorController.ResetTrigger(AnimatorHash.Attack1);
    }

    public override bool CanAttack()
    {
        return characterFiniteStateMachine.CanAttack && !characterFiniteStateMachine.AnimatorController.IsPlaying(AnimatorHash.Attack1);
    }

    public override void Attack()
    {
        characterFiniteStateMachine.AnimatorController.SetTrigger(AnimatorHash.Attack1);

        playableCharacter.GetDamage(enemyCharacter.stat.strikingPower);

        if(!playableCharacter.IsAlive())
        {
            playableCharacter = null;
            characterFiniteStateMachine.ChaseTarget = null;

            characterFiniteStateMachine.SetState(CharacterStateType.IdleState);
        }
    }
}