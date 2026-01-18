namespace TreasureHunt.Root
{
    using TreasureHunt.Configs;
    using TreasureHunt.Services;
    using UnityEngine;

    /// <summary>
    /// This is the game root, the application's entry point
    /// Responsibilities: Initialization, Injection and Binding
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameContextConfig gameContextConfig;

        private void Awake()
        {
            Debug.Log($"[EntryPoint] context config: {JsonUtility.ToJson(this.gameContextConfig, true)}");

            IWalletService walletService = new InMemoryWalletService();
            IRewardService rewardService = new RandomRewardService(gameContextConfig);

            walletService.ResourceUpdated += (evt) =>
            {
                Debug.Log($"[Event] Currency Update: {evt.Name} changed by {evt.Amount}. Total: {evt.NewTotal}");
            };

            walletService.UpdateResource("Coins", 100);

            var rewardResult = rewardService.GenerateReward();
            walletService.UpdateResource(rewardResult!.Value.Name, rewardResult!.Value.Amount);

        }
    }

}