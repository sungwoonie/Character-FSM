using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;
using UnityEngine.EventSystems;

public class ChaseState : MonoBehaviour, IState<CharacterFiniteStateMachine>
{
    protected CharacterFiniteStateMachine characterFiniteStateMachine;
    protected SpriteRenderer spriteRenderer;

    public virtual void Handle(CharacterFiniteStateMachine controller)
    {
        if(characterFiniteStateMachine == null)
        {
            characterFiniteStateMachine = controller;

            if(TryGetComponent<SpriteRenderer>(out var spriteRendererComponent))
            {
                spriteRenderer = spriteRendererComponent;
            }
        }

        StartCoroutine(ChaseCoroutine());
    }

    public virtual IEnumerator ChaseCoroutine()
    {
        var currentDistance = 0.0f;

        while(CanChase())
        {
            currentDistance = Vector2.Distance(transform.position, characterFiniteStateMachine.ChaseTarget.position);

            if(currentDistance > ChaseDistance())
            {
                var moveDirection = (Vector2)(characterFiniteStateMachine.ChaseTarget.position - transform.position).normalized;
                transform.Translate(moveDirection * Time.deltaTime * ChaseSpeed());

                SetSpriteFlipX(moveDirection.x < 0);

                characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Run, true);
            }
            else
            {
                characterFiniteStateMachine.SetState(CharacterStateType.CombatState);
                characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Run, false);
                yield break;
            }

            yield return null;
        }

        characterFiniteStateMachine.SetState(CharacterStateType.IdleState);
        characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Run, false);
    }

    public virtual bool CanChase()
    {
        return characterFiniteStateMachine.ChaseTarget != null && characterFiniteStateMachine.ChaseTarget.gameObject.activeSelf;
    }

    public virtual float ChaseDistance() => 1.0f;

    public virtual float ChaseSpeed() => 0.3f;

    public virtual void SetSpriteFlipX(bool isLeft)
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.flipX = isLeft;
        }
    }

    public void StopFunction()
    {
        StopAllCoroutines();
    }
}
