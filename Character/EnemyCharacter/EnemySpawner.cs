using StarCloudgamesLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemySpawner : SingleTon<EnemySpawner>
{
    [Header("Assignments")]
    [SerializeField] private EnemyCharacter enemyCharacterPrefab;
    [SerializeField] private Vector2 spawnArea;

    [Header("Animator")]
    [SerializeField] private List<EnemyAnimator> enemyAnimators;
    [SerializeField] private List<EnemyAnimator> dungeonEnemyAnimators;
    [SerializeField] private List<EnemyAnimator> advancementAnimators;
    [SerializeField] private List<EnemyAnimator> limitBreakAnimators;
    [SerializeField] private List<EnemyAnimator> towerOfPandaAnimators;

    private ObjectPooling<EnemyCharacter> enemyCharacterPool;
    private EnemyCharacter spawnedBoss;

    #region "Unity"

    protected override void Awake()
    {
        base.Awake();

        Initialize();
    }

    #endregion

    #region "Initialize"

    private void Initialize()
    {
        enemyCharacterPool = new ObjectPooling<EnemyCharacter>(enemyCharacterPrefab, 10, transform);
    }

    #endregion

    #region "Spawn"

    public EnemyCharacter SpawnNewEnemy(EnemyStat enemyStat, bool needHealthBar = false, bool randomPosition = true)
    {
        var spawnedEnemy = enemyCharacterPool.GetPool();

        if(needHealthBar && GameManager.instance.CurrentSpawnType() == SpawnType.Normal)
        {
            spawnedBoss = spawnedEnemy;
        }
        else
        {
            spawnedBoss = null;
        }

        spawnedEnemy.SetEnemy(enemyStat, GetEnemyAnimator(), GetExtraDamageStatType());
        spawnedEnemy.transform.position = randomPosition ? GetRandomSpawnArea() : Vector3.zero;

        if(!needHealthBar)
        {
            spawnedEnemy.Spawn();
        }

        return spawnedEnemy;
    }

    #endregion

    #region "Common Method"

    public void ClearAllEnemy()
    {
        enemyCharacterPool.DeactiveAllPool();
        DebugManager.DebugInGameMessage("All enemies cleared.");
    }

    #endregion

    #region "Get Values"

    public List<EnemyCharacter> ActivatingEnemies() => enemyCharacterPool.GetActivatingPools();

    #endregion

    #region "Animator"

    private RuntimeAnimatorController GetEnemyAnimator()
    {
        var currentSpawnType = GameManager.instance.CurrentSpawnType();

        switch(currentSpawnType)
        {
            case SpawnType.Normal:
                return GetRandomEnemyAnimator(UserDatabaseController.instance.GetStage(), spawnedBoss != null);
            case SpawnType.Dungeon:
                return GetRandomDungeonEnemyAnimator((int)DungeonManager.instance.CurrentDungeonType());
            case SpawnType.Advancement:
                return GetAdvancementEnemyAnimator();
            case SpawnType.LimitBreak:
                return GetLimitBreakEnemyAnimator();
            case SpawnType.TowerOfPanda:
                return GetTowerOfPandaEnemyAnimators();
            default:
                return null;
        }
    }

    private RuntimeAnimatorController GetAdvancementEnemyAnimator()
    {
        var currentTier = (int)AdvancementManager.instance.GetCurrentTier() + 1;
        foreach(var animator in advancementAnimators)
        {
            if(animator.stage == currentTier)
            {
                return animator.enemyAnimators[0];
            }
        }

        Debug.LogWarning($"No animator found for advancement {currentTier}, returning null.");
        return null;
    }

    private RuntimeAnimatorController GetRandomEnemyAnimator(int stage, bool isBoss)
    {
        foreach(var animator in enemyAnimators)
        {
            if(animator.stage == stage)
            {
                var targetAnimators = isBoss ? animator.bossAnimators : animator.enemyAnimators;
                int randomIndex = Random.Range(0, targetAnimators.Count);
                return targetAnimators[randomIndex];
            }
        }
        Debug.LogWarning($"No animator found for stage {stage}, returning null.");
        return null;
    }

    private RuntimeAnimatorController GetRandomDungeonEnemyAnimator(int dungeonIndex)
    {
        foreach(var animator in dungeonEnemyAnimators)
        {
            if(animator.stage == dungeonIndex)
            {
                int randomIndex = Random.Range(0, animator.enemyAnimators.Count);
                return animator.enemyAnimators[randomIndex];
            }
        }

        Debug.LogWarning($"No animator found for stage {dungeonIndex}, returning null.");
        return null;
    }

    private RuntimeAnimatorController GetLimitBreakEnemyAnimator()
    {
        return limitBreakAnimators[0].enemyAnimators[0];
    }

    private RuntimeAnimatorController GetTowerOfPandaEnemyAnimators()
    {
        var currentInfo = UserDatabaseController.instance.GetTowerOfPandaData();
        foreach(var animator in towerOfPandaAnimators)
        {
            if(animator.stage == currentInfo[0])
            {
                int randomIndex = Random.Range(0, animator.enemyAnimators.Count);
                return animator.enemyAnimators[randomIndex];
            }
        }

        Debug.LogWarning($"No animator found for TowerOfPanda {currentInfo}, returning null.");
        return null;
    }

    #endregion

    #region "Get"

    private StatType GetExtraDamageStatType()
    {
        var currentSpawnType = GameManager.instance.CurrentSpawnType();

        switch(currentSpawnType)
        {
            case SpawnType.Normal:
                return spawnedBoss == null ? StatType.normalMonsterExtraDamage : StatType.bossMonsterExtraDamage;
            case SpawnType.Dungeon:
                return StatType.dungeonExtraDamage;
            case SpawnType.TowerOfPanda:
                return StatType.towerOfPandaExtraDamage;
            default:
                return StatType.none;
        }
    }

    private Vector2 GetRandomSpawnArea()
    {
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float y = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        return new Vector2(x, y);
    }

    #endregion
}

[System.Serializable]
public struct EnemyAnimator
{
    public int stage;
    public List<RuntimeAnimatorController> enemyAnimators;
    public List<RuntimeAnimatorController> bossAnimators;
}