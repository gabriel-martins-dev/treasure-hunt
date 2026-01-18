namespace TreasureHunt.Services
{
    using TreasureHunt.Configs;
    using UnityEngine;

    public struct RewardResult
    {
        public string Name;
        public int Amount;
    }

    public interface IRewardService
    {
        RewardResult? GenerateReward();
    }

    public class RandomRewardService : IRewardService
    {
        readonly IRewardConfig config;

        public RandomRewardService(IRewardConfig rewardConfig) 
        {
            this.config = rewardConfig;
        }
        public RewardResult? GenerateReward()
        {
            var rewards = this.config.RewardDefinitions;
            if (rewards == null || rewards.Length == 0)
            {
                Debug.LogWarning("[RewardService] No rewards defined in Config!");
                return null;
            }

            var rewardDef = rewards[Random.Range(0, rewards.Length)];
            int amount = Random.Range(rewardDef.MinAmount, rewardDef.MaxAmount + 1);
            return new RewardResult { Name = rewardDef.Name, Amount = amount };
        }
    }
}