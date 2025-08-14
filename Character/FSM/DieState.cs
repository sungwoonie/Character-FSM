using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;

public class DieState : MonoBehaviour, IState<CharacterFiniteStateMachine>
{
    protected SpriteRenderer spriteRenderer;
    protected CharacterFiniteStateMachine characterFiniteStateMachine;

    public void Handle(CharacterFiniteStateMachine controller)
    {
        if(characterFiniteStateMachine == null)
        {
            characterFiniteStateMachine = controller;

            if(TryGetComponent<SpriteRenderer>(out var spriteRendererComponent))
            {
                spriteRenderer = spriteRendererComponent;
            }
        }

        Die();
    }

    public virtual void Die(){}

    public void StopFunction(){}
}
