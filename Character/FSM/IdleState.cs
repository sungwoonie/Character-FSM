using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

public class IdleState : MonoBehaviour, IState<CharacterFiniteStateMachine>
{
    protected CharacterFiniteStateMachine characterFiniteStateMachine;

    public void Handle(CharacterFiniteStateMachine controller)
    {
        if(characterFiniteStateMachine == null)
        {
            characterFiniteStateMachine = controller;
        }

        StartCoroutine(IdleCoroutine());
    }

    public virtual IEnumerator IdleCoroutine()
    {
        if(characterFiniteStateMachine.AnimatorController.GetBool(AnimatorHash.Die))
        {
            characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Die, false);
        }

        characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Run, false);
        yield return new WaitUntil(() => WaitForReady());

        while(!ExistTarget())
        {
            yield return Yielder.WaitForSeconds(1.0f);
        }

        SetChaseTarget();

        characterFiniteStateMachine.SetState(CharacterStateType.ChaseState);
    }

    public virtual bool WaitForReady()
    {
        return characterFiniteStateMachine.AnimatorController.IsPlaying(AnimatorHash.Idle);
    }

    public virtual bool ExistTarget() => false;

    public virtual void SetChaseTarget(){}

    public void StopFunction()
    {
        StopAllCoroutines();
    }
}