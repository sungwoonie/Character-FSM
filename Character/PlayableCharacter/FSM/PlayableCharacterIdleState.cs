using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayableCharacterIdleState : IdleState
{
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    public override bool ExistTarget()
    {
        enemies = EnemySpawner.instance.ActivatingEnemies();
        return enemies.Count > 0 && enemies.Find(x => x.gameObject.activeSelf);
    }

    public override void SetChaseTarget()
    {
        characterFiniteStateMachine.ChaseTarget = NearestEnemy();
    }

    private Transform NearestEnemy()
    {
        Transform nearest = null;
        float minDistanceSqr = Mathf.Infinity;

        foreach(var monster in enemies)
        {
            if(monster == null || !monster.gameObject.activeInHierarchy || !monster.IsAlive()) continue;

            float distanceSqr = (monster.transform.position - transform.position).sqrMagnitude;
            if(distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                nearest = monster.transform;
            }
        }

        return nearest;
    }
}