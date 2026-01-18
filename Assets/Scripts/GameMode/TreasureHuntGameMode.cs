namespace TreasureHunt.GameMode
{
    using System;
    using TreasureHunt.Configs;
    using TreasureHunt.Services;
    using UnityEngine;

    public class TreasureHuntGameMode : IGameMode
    {
        public event Action<int> AttemptsUpdated;
        public event Action<bool> GameCompleted;

        readonly IRoundConfig roundConfig;
        readonly IWalletService walletService;
        readonly IRewardService rewardService;
        readonly IRandomService randomService;

        int victoryChestIndex;
        int currentAttempts;
        bool gameFinished;

        public TreasureHuntGameMode(    
            IRoundConfig roundConfig,
            IWalletService walletService,
            IRewardService rewardService,
            IRandomService randomService)
        {
            this.roundConfig = roundConfig ?? throw new ArgumentNullException(nameof(roundConfig));
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            this.rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
            this.randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }

        public void StartGame()
        {
            this.gameFinished = false;
            this.currentAttempts = this.roundConfig.MaxAttemptsPerRound;
            this.victoryChestIndex = this.randomService.Range(0, this.roundConfig.NumberOfChestsPerRound);
            Debug.Log($"[GameMode] Round Started. Attempts: {this.currentAttempts}. Treasure is in {this.victoryChestIndex}");
            this.AttemptsUpdated?.Invoke(currentAttempts);
        }

        public void OpenAction(int openIndex)
        {
            if(gameFinished)
            {
                Debug.LogError("[GameMode] User tried to open chest in finished game.");
                return;
            }

            // update attempts
            this.currentAttempts--;
            this.AttemptsUpdated?.Invoke(this.currentAttempts);

            bool isWinner = (openIndex == this.victoryChestIndex);
            if(!isWinner)
            {
                if (this.currentAttempts <= 0)
                {
                    this.gameFinished = true;
                    this.GameCompleted?.Invoke(false);
                }
            }
            else
            {
                RewardResult? reward = this.rewardService.GenerateReward();
                if (reward.HasValue)
                {
                    this.walletService.UpdateResource(reward.Value.Name, reward.Value.Amount);
                }

                this.gameFinished = true;
                this.GameCompleted?.Invoke(true);
            }
        }
    }
}