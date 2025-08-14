using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarCloudgamesLibrary;
using System;

public class EnemyDieState : DieState
{
    private readonly float fadeSpeed = 3.0f;

    private EnemyCharacter enemy;
    private Dictionary<SpawnType, Action> deadActions;
    private Dictionary<SpawnType, Action> deadQuestActions;

    public override void Die()
    {
        if(enemy == null)
        {
            enemy = characterFiniteStateMachine.GetComponent<EnemyCharacter>();

            deadActions = new()
            {
                { SpawnType.Normal, () => StageManager.instance.EnemyDead(enemy.stat) },
                { SpawnType.Dungeon, () => DungeonManager.instance.DungeonEnemyDead() },
                { SpawnType.Advancement, () => AdvancementManager.instance.AdvancementEnemyDead() },
                { SpawnType.TowerOfPanda, () => TowerOfPandaManager.instance.TowerOfPandaEnemyDead() }
            };
        }

        StartCoroutine(DieAnimation());

        if(deadActions.TryGetValue(GameManager.instance.CurrentSpawnType(), out var action))
        {
            action.Invoke();
        }
    }

    private IEnumerator DieAnimation()
    {
        characterFiniteStateMachine.AnimatorController.SetBool(AnimatorHash.Die, true);

        Color fadeColor = spriteRenderer.color;
        float currentFadeA = spriteRenderer.color.a;

        while(currentFadeA > 0.0f)
        {
            currentFadeA -= Time.deltaTime * fadeSpeed;
            fadeColor.a = currentFadeA;

            spriteRenderer.color = fadeColor;

            transform.Translate(Vector2.up * Time.deltaTime * 1.3f);

            yield return null;
        }

        gameObject.SetActive(false);

        spriteRenderer.color = Color.white;
    }
}