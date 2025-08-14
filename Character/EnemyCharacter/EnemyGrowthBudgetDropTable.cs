using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

[System.Serializable]
public struct EnemyGrowthBudgetDropTable
{
    public Dictionary<int, EnemyGrowthBudgetDropReward> dropRewards;
    public Tier tier;

    public List<SCReward> GetRewards(int tier)
    {
        var rewardList = new List<SCReward>();

        foreach(var dropReward in dropRewards)
        {
            if(dropReward.Key <= tier)
            {
                var newReward = new SCReward($"DropGrowthBudgetReward{tier}", RewardType.Currency, dropReward.Value.rewardID, dropReward.Value.rewardAmount);
                rewardList.Add(newReward);
            }
        }

        return rewardList;
    }

    public EnemyGrowthBudgetDropTable(Dictionary<string, string> parsedData)
    {
        tier = StringParser.ParseEnum(parsedData, "Tier", Tier.None);

        dropRewards = new Dictionary<int, EnemyGrowthBudgetDropReward>();

        for(int i = (int)Tier.F; i <= (int)Tier.LL; i++)
        {
            if(parsedData.TryGetValue($"{(Tier)i}RewardID", out var data))
            {
                var rewardID = StringParser.ParseEnum(parsedData, $"{(Tier)i}RewardID", RewardID.None);

                if(rewardID == RewardID.None)
                {
                    continue;
                }

                var newDropReward = new EnemyGrowthBudgetDropReward
                {
                    rewardAmount = StringParser.ParseDouble(parsedData, $"{(Tier)i}RewardAmount"),
                    rewardID = rewardID
                };

                dropRewards[i] = newDropReward;
            }
        }
    }
}

public struct EnemyGrowthBudgetDropReward
{
    public RewardID rewardID;
    public double rewardAmount;
}