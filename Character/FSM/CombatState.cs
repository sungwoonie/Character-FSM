using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;

public class CombatState : MonoBehaviour, IState<CharacterFiniteStateMachine>
{
    protected CharacterFiniteStateMachine characterFiniteStateMachine;
    protected SpriteRenderer spriteRenderer;

    public virtual void Handle(CharacterFiniteStateMachine controller)
    {
        if(characterFiniteStateMachine == null)
        {
            characterFiniteStateMachine = controller;

            if(spriteRenderer == null && TryGetComponent<SpriteRenderer>(out var spriteRendererComponent))
            {
                spriteRenderer = spriteRendererComponent;
            }
        }

        StartCoroutine(CombatCoroutine());
    }

    public virtual IEnumerator CombatCoroutine()
    {
        var currentAttackDelay = 0.0f;

        while(CanCombat())
        {
            if(currentAttackDelay > 0.0f)
            {
                currentAttackDelay -= Time.deltaTime * AttackDelay();
            }
            else
            {
                if(CanAttack())
                {
                    Attack();
                    currentAttackDelay = 1f;
                }
            }

            yield return null;
        }

        ResetTrigger();
        yield return new WaitUntil(() => characterFiniteStateMachine.AnimatorController.IsPlaying(AnimatorHash.Idle));

        characterFiniteStateMachine.SetState(CharacterStateType.IdleState);
    }

    public virtual bool CanCombat()
    {
        return characterFiniteStateMachine.ChaseTarget != null && characterFiniteStateMachine.ChaseTarget.gameObject.activeSelf && CheckDistance(); 
    }

    public bool CheckDistance()
    {
        var chaseState = (ChaseState)characterFiniteStateMachine.GetState(CharacterStateType.ChaseState);
        var currentDistance = Vector2.Distance(transform.position, characterFiniteStateMachine.ChaseTarget.position);

        return chaseState != null && currentDistance < chaseState.ChaseDistance();
    }

    public virtual float AttackDelay() => 1.0f;

    public virtual void Attack() {}

    public virtual bool CanAttack() => false;

    public virtual void GetDamageEffect()
    {
        StartCoroutine(GetDamageSprite());
    }

    public virtual IEnumerator GetDamageSprite()
    {
        if (spriteRenderer == null && TryGetComponent<SpriteRenderer>(out var spriteRendererComponent))
        {
            spriteRenderer = spriteRendererComponent;
        }

        for (int i = 0; i < 3; i++)
        {
            if(i % 2 == 0)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }

            yield return Yielder.WaitForSeconds(0.1f);
        }

        spriteRenderer.color = Color.white;
    }
    public virtual void ResetTrigger() { }

    public void StopFunction()
    {
        spriteRenderer.color = Color.white;

        StopAllCoroutines();
    }
}
