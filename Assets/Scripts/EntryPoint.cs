namespace TreasureHunt.Root
{
    using TreasureHunt.Configs;
    using TreasureHunt.GameMode;
    using TreasureHunt.Presentation;
    using TreasureHunt.Services;
    using TreasureHunt.View;
    using UnityEngine;

    /// <summary>
    /// This is the game root, the application's entry point
    /// Responsibilities: Initialization, Injection and Binding
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameContextConfig gameContextConfig;
        [SerializeField] private GameScreenView gameScreenView;

        async void Awake()
        {
            Debug.Log($"[EntryPoint] context config: {JsonUtility.ToJson(this.gameContextConfig, true)}");
            IRandomService randomService = new RandomService();
            IWalletService walletService = new InMemoryWalletService();
            IRewardService rewardService = new RandomRewardService(gameContextConfig, randomService);
            IGameMode gameMode = new TreasureHuntGameMode(gameContextConfig, walletService, rewardService, randomService);
            GameViewModel gameViewModel = new (gameMode, gameContextConfig);

            walletService.ResourceUpdated += (evt) =>
            {
                Debug.Log($"[Event] Currency Update: {evt.Name} changed by {evt.Amount}. Total: {evt.NewTotal}");
            };

            gameMode.GameCompleted += (victory) =>
            {
                var msg = victory ? "Victory!" : "Defeat!";
                Debug.Log($"[Event] Game finished. {msg}");
            };

            gameScreenView.Bind(gameViewModel);
            await gameScreenView.InitializeAsync();
        }
    }
}