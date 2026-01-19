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
        [SerializeField] GameContextConfig gameContextConfig;
        [SerializeField] GameScreenView gameScreenView;
        [SerializeField] ResourcesHUDView resourcesHUDView;

        GameViewModel gameViewModel;

        async void Awake()
        {
            IRandomService randomService = new RandomService();
            IWalletService walletService = new InMemoryWalletService();
            IRewardService rewardService = new RandomRewardService(this.gameContextConfig, randomService);
            IGameMode gameMode = new TreasureHuntGameMode(this.gameContextConfig, walletService, rewardService, randomService);
            this.gameViewModel = new (this.gameContextConfig, gameMode, walletService);

            this.gameScreenView.Bind(this.gameViewModel);
            this.resourcesHUDView.Bind(this.gameViewModel);

            await this.gameScreenView.InitializeAsync();
        }

        void OnDestroy()
        {
            this.gameViewModel?.Dispose();
        }
    }
}