namespace TreasureHunt.Configs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameContext", menuName = "Configs/Game Context")]
    public class GameContextConfig : ScriptableObject, IContextConfig
    {
        [Header("Round Configs")]
        [SerializeField, Min(1)] private int numberOfChestsPerRound = 9;
        [SerializeField, Min(1)] private int maxAttemptsPerRound = 3;
        [SerializeField, Min(0.1f)] private float chestOpenDuration = 2f;

        [Header("Reward Configs")]
        [Tooltip("List of all possible rewards")]
        [SerializeField] private RewardDefinition[] rewards;
        public IRewardDefinition[] RewardDefinitions => this.rewards;

        public int NumberOfChestsPerRound => this.numberOfChestsPerRound;
        public int MaxAttemptsPerRound => this.maxAttemptsPerRound;
        public float ChestOpenDuration => this.chestOpenDuration;

        private void OnValidate()
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