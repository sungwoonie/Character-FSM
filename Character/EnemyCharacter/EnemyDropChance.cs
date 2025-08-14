using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

[System.Serializable]
public struct EnemyDropChance
{
    public float growthBudgetDropChance;
    public Tier growthBudgetDropTier;

    public float equipmentDropChance;
    public Tier equipmentDropTier;

    public EnemyGrowthBudgetDropTable dropTable;

    public List<SCReward> GetGrowthBudgetDropRewards(int tier)
    {
        return dropTable.GetRewards(tier);
    }

    public bool CanGetGrowthBudget()
    {
        return GachaTool.CanGet(growthBudgetDropChance);
    }

    public bool CanGetEquipment()
    {
        return GachaTool.CanGet(equipmentDropChance);
    }

    public EnemyDropChance(Dictionary<string, string> parsedData)
    {
        dropTable = new EnemyGrowthBudgetDropTable();

        growthBudgetDropChance = StringParser.ParseFloat(parsedData, "GrowthBudgetDropChance");
        growthBudgetDropTier = StringParser.ParseEnum(parsedData, "GrowthBudgetDropTier", Tier.None);

        equipmentDropChance = StringParser.ParseFloat(parsedData, "EquipmentDropChance");
        equipmentDropTier = StringParser.ParseEnum(parsedData, "EquipmentDropTier", Tier.None);
    }
}