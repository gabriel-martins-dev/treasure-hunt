namespace TreasureHunt.Services
{
    using System;
    using TreasureHunt.Configs;
    using UnityEngine;

    public struct RewardResult
    {
        public string Name;
        public int Amount;
    }

    /// <summary>
    /// Structure for calculating rewards
    /// </summary>
    public interface IRewardService
    {
        RewardResult? GenerateReward();
    }

    /// <summary>
    /// Randomly selects and calculating rewards
    /// </summary>
    public class RandomRewardService : IRewardService
    {
        readonly IRewardConfig config;
        readonly IRandomService randomService;

        public RandomRewardService(IRewardConfig rewardConfig, IRandomService randomService) 
        {
            this.config = rewardConfig ?? throw new ArgumentNullException(nameof(rewardConfig));
            this.randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        public RewardResult? GenerateReward()
        {
            var rewards = this.config.RewardDefinitions;
            if (rewards == null || rewards.Length == 0)
            {
                Debug.LogWarning("[RewardService] No rewards defined in Config!");
                return null;
            }

            var rewardDef = rewards[this.randomService.Range(0, rewards.Length)];
            int amount = this.randomService.Range(rewardDef.MinAmount, rewardDef.MaxAmount + 1);
            return new RewardResult { Name = rewardDef.Name, Amount = amount };
        }
    }
}