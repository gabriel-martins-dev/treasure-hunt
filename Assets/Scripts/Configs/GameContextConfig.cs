namespace TreasureHunt.Configs
{
    using UnityEngine;

    /// <summary>
    /// Concrete values of all game configs for balance. 
    /// Uses validation logic to ensure data integrity during editor editing.
    /// </summary>
    [CreateAssetMenu(fileName = "GameContext", menuName = "Configs/Game Context")]
    public class GameContextConfig : ScriptableObject, IContextConfig
    {
        [Header("Round Configs")]
        [SerializeField, Min(1)] int numberOfChestsPerRound = 9;
        [SerializeField, Min(1)] int maxAttemptsPerRound = 3;
        [SerializeField, Min(0.1f)] float chestOpenDuration = 2f;

        [Header("Reward Configs")]
        [Tooltip("List of all possible rewards")]
        [SerializeField] RewardDefinition[] rewards;
        public IRewardDefinition[] RewardDefinitions => this.rewards;

        public int NumberOfChestsPerRound => this.numberOfChestsPerRound;
        public int MaxAttemptsPerRound => this.maxAttemptsPerRound;
        public float ChestOpenDuration => this.chestOpenDuration;

        void OnValidate()
        {
            if (this.rewards != null) {
                for (int i = 0; i < this.rewards.Length; i++)
                {
                    this.rewards[i].OnValidate(); // must hava manual check, since rewards aren't Mono
                }
            }
        }
    }
}