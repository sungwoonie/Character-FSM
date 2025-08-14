using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyStat
{
    public double maxHealthPoint;
    public double strikingPower;
    public double dropGold;
    public double dropEXP;
    public EnemyDropChance dropChance;

    public bool canMove;
    public bool canAttack;

    public bool isValid;

    public EnemyStat(AdvancementData advancementData)
    {
        isValid = true;
        maxHealthPoint = advancementData.maxHealthPoint;
        strikingPower = 0d;
        dropGold = 0d;
        dropEXP = 0d;
        dropChance = new EnemyDropChance();

        canMove = false;
        canAttack = false;
    }

    public EnemyStat(TowerOfPandaData towerOfPandaData)
    {
        isValid = true;

        maxHealthPoint = towerOfPandaData.monsterHealthPoint;
        strikingPower = 0d;
        dropGold = 0d;
        dropEXP = 0d;
        dropChance = new EnemyDropChance();

        canMove = true;
        canAttack = true;
    }

    public EnemyStat(DungeonData dungeonData)
    {
        isValid = true;

        maxHealthPoint = 0d;
        strikingPower = 0d;
        dropGold = 0d;
        dropEXP = 0d;
        dropChance = new EnemyDropChance();

        canMove = true;
        canAttack = true;

        var currentStep = dungeonData.CurrentStep;
        var currentStage = dungeonData.CurrentStage;

        if (dungeonData.monsterHealthPointOffsets.TryGetValue(currentStage, out var offset))
        {
            if (dungeonData.monsterHealthPointRatios.TryGetValue(currentStage, out var ratio))
            {
                maxHealthPoint = offset * Math.Pow(ratio, currentStep - 1);
            }
        }
    }

    public EnemyStat(StageData stageData, int floor, EnemyGrowthBudgetDropTable _dropTable, bool isBoss)
    {
        isValid = true;

        maxHealthPoint = stageData.monsterOffsetHealthPoint * Math.Pow(stageData.monsterHealthPointRatio, floor - 1);
        strikingPower = stageData.monsterStrikingPower * Math.Pow(stageData.monsterStrikingPowerRatio, floor - 1);
        dropGold = stageData.dropGoldOffset * Math.Pow(stageData.dropGoldRatio, floor - 1);
        dropEXP = stageData.dropEXPOffset * Math.Pow(stageData.dropEXPRatio, floor - 1);

        dropChance = stageData.dropChance;
        dropChance.dropTable = _dropTable;

        canMove = true;
        canAttack = true;

        if (isBoss)
        {
            maxHealthPoint *= 10.0d;
            strikingPower *= 5.0d;
            dropGold *= 6.0d;
            dropEXP *= 3.0d;

            dropChance.equipmentDropChance *= 2.0f;
            dropChance.growthBudgetDropChance *= 2.0f;
        }
    }
}