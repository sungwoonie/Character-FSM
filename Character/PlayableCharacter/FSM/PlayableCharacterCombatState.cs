using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;

public class PlayableCharacterCombatState : CombatState
{
    private int attackAnimationIndex;

    private EnemyCharacter currentTarget;
    private FloatingTextController floatingTextController;


    public override IEnumerator CombatCoroutine()
    {
        floatingTextController ??= GetComponent<FloatingTextController>();

        attackAnimationIndex = 0;
        currentTarget = characterFiniteStateMachine.ChaseTarget.GetComponent<EnemyCharacter>();

        return base.CombatCoroutine();
    }

    public override float AttackDelay() => (float)StatManager.instance.GetStat(ScriptableStatType.TrainingStat, StatType.attackSpeed);

    public override void ResetTrigger()
    {
        characterFiniteStateMachine.AnimatorController.ResetTrigger(AnimatorHash.Attack1);
        characterFiniteStateMachine.AnimatorController.ResetTrigger(AnimatorHash.Attack2);
    }

    public override bool CanAttack()
    {
        return !characterFiniteStateMachine.AnimatorController.IsPlaying(AnimatorHash.Attack1) && !characterFiniteStateMachine.AnimatorController.IsPlaying(AnimatorHash.Attack2);
    }

    public override void Attack() 
    {
        if(attackAnimationIndex == 0)
        {
            characterFiniteStateMachine.AnimatorController.SetTrigger(AnimatorHash.Attack1);
            attackAnimationIndex++;
        }
        else
        {
            characterFiniteStateMachine.AnimatorController.SetTrigger(AnimatorHash.Attack2);
            attackAnimationIndex = 0;
        }

        var damage = StatManager.instance.GetFinalNormalAttackDamage(out bool isCritical);
        var bonusDamage = StatManager.instance.GetStat(ScriptableStatType.None, currentTarget.bonusDamageStatType);

        damage = damage * (1 + bonusDamage / 100.0d);

        currentTarget.GetDamage(damage);

        floatingTextController.SetFloatingText(damage.ToCurrencyString(), currentTarget.transform.position, isCritical);

        if (!currentTarget.IsAlive())
        {
            currentTarget = null;
            characterFiniteStateMachine.ChaseTarget = null;

            characterFiniteStateMachine.SetState(CharacterStateType.IdleState);
        }
    }
}