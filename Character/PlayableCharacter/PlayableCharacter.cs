using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;
using System;

#region "RequireComponent"
[RequireComponent(typeof(PlayableCharacterIdleState))]
[RequireComponent(typeof(PlayableCharacterChaseState))]
[RequireComponent(typeof(PlayableCharacterCombatState))]
[RequireComponent(typeof(PlayableCharacterDieState))]
[RequireComponent(typeof(CharacterFiniteStateMachine))]
#endregion
public class PlayableCharacter : SingleTon<PlayableCharacter>
{
    private double currentStageHP;
    private double currentMaxHP;

    private CharacterFiniteStateMachine fsm;

    private Coroutine cureHealthCoroutine;
    private Coroutine checkMaxHealthCoroutine;

    #region "Unity"

    protected override void Awake()
    {
        base.Awake();

        InitializeComponent();
    }

    private void Start()
    {
        SetUpCharacter();
    }

    #endregion

    #region "Initialize"

    private void InitializeComponent()
    {
        fsm = GetComponent<CharacterFiniteStateMachine>();
    }

    #endregion

    #region "Common API"

    public bool IsAlive()
    {
        return currentStageHP > 0;
    }

    #endregion

    #region "Set Up"

    public void SetUpCharacter()
    {
        SetUpHealth();

        fsm.SetState(CharacterStateType.IdleState);
    }

    private void SetUpHealth()
    {
        currentMaxHP = StatManager.instance.GetStat(ScriptableStatType.TrainingStat, StatType.maxHealthPoint);
        SetCurrentStageHP(AntiCheatManager.instance.Get("CurrentStageHP", currentMaxHP));

        if (cureHealthCoroutine != null)
        {
            StopCoroutine(cureHealthCoroutine);
            cureHealthCoroutine = null;
        }

        if (checkMaxHealthCoroutine != null)
        {
            StopCoroutine(checkMaxHealthCoroutine);
            checkMaxHealthCoroutine = null;
        }

        checkMaxHealthCoroutine = StartCoroutine(CheckMaxHealthPoint());
        cureHealthCoroutine = StartCoroutine(CureHealthCoroutine());
    }

    #endregion

    #region "Health"

    private void SetCurrentStageHP(double hp)
    {
        currentStageHP = hp;
        AntiCheatManager.instance.Set("CurrentStageHP", currentStageHP);
    }

    private IEnumerator CheckMaxHealthPoint()
    {
        while (true)
        {
            yield return Yielder.WaitForSeconds(1.0f);

            var maxHealthPoint = StatManager.instance.GetStat(ScriptableStatType.TrainingStat, StatType.maxHealthPoint);
            if (currentMaxHP != maxHealthPoint)
            {
                currentMaxHP = maxHealthPoint;
            }
        }
    }

    private IEnumerator CureHealthCoroutine()
    {
        while (true)
        {
            yield return Yielder.WaitForSeconds(1.0f);

            var cureInterval = StatManager.instance.GetStat(ScriptableStatType.TrainingStat, StatType.healthCure);
            var maxHealthPoint = StatManager.instance.GetStat(ScriptableStatType.TrainingStat, StatType.maxHealthPoint);
            var hpAfterCure = currentStageHP + (currentMaxHP * (cureInterval / 100));
            hpAfterCure = Math.Min(hpAfterCure, maxHealthPoint);

            if(hpAfterCure != currentStageHP)
            {
                SetCurrentStageHP(hpAfterCure);
            }
        }
    }

    public void GetDamage(double damage)
    {
        var afterHP = currentStageHP - damage;

        if (afterHP <= 0)
        {
            fsm.SetState(CharacterStateType.DieState);
            afterHP = 0;
        }
        else
        {
            var combatState = (PlayableCharacterCombatState)fsm.GetState(CharacterStateType.CombatState);
            combatState.GetDamageEffect();
        }

        SetCurrentStageHP(afterHP);
    }

    #endregion
}