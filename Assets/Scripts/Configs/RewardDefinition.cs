namespace TreasureHunt.Configs
{
    using System;
    using UnityEngine;

    [Serializable]
    public class RewardDefinition : IRewardDefinition
    {
        [Tooltip("The Name of the reward (e.g. 'Coins', 'Gems')")]
        [SerializeField] private string name; // TRADEOFF: using weak reference for simplicity. But could shared structure with server, for example. Like an enum
        [SerializeField] private int minAmount;
        [SerializeField] private int maxAmount;

        public string Name => this.name;
        public int MinAmount => this.minAmount;
        public int MaxAmount => this.maxAmount;

        public void OnValidate()
        {
            if (maxAmount < minAmount)
            {
                maxAmount = minAmount;
                Debug.LogError($"{nameof(this.MaxAmount)} can't be smaller then {nameof(this.MinAmount)}!");
            }
        }
    }
}