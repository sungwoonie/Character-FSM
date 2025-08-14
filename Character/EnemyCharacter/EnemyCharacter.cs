using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

#region "RequireComponent"
[RequireComponent(typeof(EnemyIdleState))]
[RequireComponent(typeof(EnemyChaseState))]
[RequireComponent(typeof(EnemyCombatState))]
[RequireComponent(typeof(EnemyDieState))]
[RequireComponent(typeof(CharacterFiniteStateMachine))]
#endregion
public class EnemyCharacter : MonoBehaviour
{
    public EnemyStat stat;
    public StatType bonusDamageStatType;

    private double currentHealthPoint;

    private CharacterFiniteStateMachine fsm;
    private UIHealthBar healthBar;

    public void Spawn()
    {
        currentHealthPoint = stat.maxHealthPoint;
        healthBar?.gameObject.SetActive(true);

        fsm.SetState(CharacterStateType.IdleState);
    }

    public void SetEnemy(EnemyStat targetStat, RuntimeAnimatorController targetAnimator, StatType bonusDamageStatType)
    {
        stat = targetStat;
        this.bonusDamageStatType = bonusDamageStatType;

        healthBar?.gameObject.SetActive(false);
        healthBar = null;

        if (fsm == null)
        {
            InitializeComponent();
        }

        fsm.SetState(targetStat.canAttack, targetStat.canMove);
        fsm.AnimatorController.SetUp(targetAnimator);

        gameObject.SetActive(true);
    }

    public void SetHealthBar(UIHealthBar uiHealthBar, HealthBarType healthBarType = HealthBarType.HealthBar)
    {
        healthBar = uiHealthBar;
        healthBar.SetUpHealthBar(stat.maxHealthPoint, healthBarType);
    }

    private void InitializeComponent()
    {
        fsm = GetComponent<CharacterFiniteStateMachine>();
    }

    public void GetDamage(double damage)
    {
        if(GameManager.instance.CurrentSpawnType() == SpawnType.LimitBreak)
        {
            LimitBreakManager.instance.ComulativeDamage(damage);
            healthBar?.UpdateHealthBar(damage);
            return;
        }

        currentHealthPoint -= damage;
        if(currentHealthPoint <= 0)
        {
            currentHealthPoint = 0;
            fsm.SetState(CharacterStateType.DieState);
        }
        else
        {
            var combatState = (EnemyCombatState)fsm.GetState(CharacterStateType.CombatState);
            combatState.GetDamageEffect();
        }

        healthBar?.UpdateHealthBar(currentHealthPoint);
    }

    public bool IsAlive()
    {
        return currentHealthPoint > 0;
    }
}