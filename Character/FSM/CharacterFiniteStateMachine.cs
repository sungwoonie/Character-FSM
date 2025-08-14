using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AnimatorController))]
public class CharacterFiniteStateMachine : MonoBehaviour
{
    private StateContext<CharacterFiniteStateMachine> stateContext;
    private Dictionary<CharacterStateType, IState<CharacterFiniteStateMachine>> states;

    public AnimatorController AnimatorController { get { return animator; } }
    private AnimatorController animator;

    private UnityAction onStopAction;
    private UnityAction onPlayingAction;

    public bool CanAttack { get; set; }
    public bool CanMove { get; set; }
    public Transform ChaseTarget { get; set; }

    private void Awake()
    {
        InitializeState();
        InitializeComponent();
    }

    private void OnEnable()
    {
        InitializeEventBus();
    }

    private void OnDisable()
    {
        if(stateContext != null)
        {
            stateContext.Stop();
        }

        DeinitializeEventBus();
    }

    private void InitializeComponent()
    {
        animator = GetComponent<AnimatorController>();
    }

    #region "EventBus"

    private void InitializeEventBus()
    {
        onStopAction = () =>
        {
            SetState(CharacterStateType.IdleState);
            stateContext.Stop();
        };

        onPlayingAction = () =>
        {
            SetState(CharacterStateType.IdleState);
        };

        EventBus.SubscribeEvent(GameState.Stop, onStopAction);
        EventBus.SubscribeEvent(GameState.Playing, onPlayingAction);
    }

    private void DeinitializeEventBus()
    {
        EventBus.UnSubscribeEvent(GameState.Stop, onStopAction);
        EventBus.UnSubscribeEvent(GameState.Playing, onPlayingAction);
    }

    #endregion

    private void InitializeState()
    {
        states = new Dictionary<CharacterStateType, IState<CharacterFiniteStateMachine>>();

        if(TryGetComponent<IdleState>(out var existIdle))
        {
            states.Add(CharacterStateType.IdleState, existIdle);
        }
        if(TryGetComponent<ChaseState>(out var existChase))
        {
            states.Add(CharacterStateType.ChaseState, existChase);
        }
        if(TryGetComponent<CombatState>(out var existCombat))
        {
            states.Add(CharacterStateType.CombatState, existCombat);
        }
        if(TryGetComponent<DieState>(out var existDie))
        {
            states.Add(CharacterStateType.DieState, existDie);
        }

        stateContext = new StateContext<CharacterFiniteStateMachine>(this);
    }

    public void SetState(bool canAttack, bool canMove)
    {
        CanAttack = canAttack;
        CanMove = canMove;
    }

    public void SetState(CharacterStateType type)
    {
        if(EventBus.GetCurrentState() == GameState.Stop) return;

        if(states.TryGetValue(type, out var state))
        {
            stateContext.Transition(state);
        }
    }

    public IState<CharacterFiniteStateMachine> GetState(CharacterStateType type)
    {
        if(states.TryGetValue(type, out var state))
        {
            return state;
        }

        return null;
    }
}

public enum CharacterStateType
{
    IdleState, ChaseState, CombatState, DieState
}