namespace TreasureHunt.View
{
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using TMPro;
    using TreasureHunt.Presentation;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages the chests and the global HUD (attempts and win/loss messages), reacting to events from view model
    /// </summary>
    public class GameScreenView : MonoBehaviour
    {
        [Header("Grid Setup")]
        [SerializeField] Transform targetContainer;
        [SerializeField] ChestView targetPrefab;

        [Header("HUD")]
        [SerializeField] TextMeshProUGUI attemptsText;
        [SerializeField] GameObject startGameFooter;
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] Button startGameButton;

        const string WinMessage = "You Found the Treasure! You Win!";
        const string LossMessage = "Game Over! Out of Attempts!";

        ChestPool chestPool;
        GameViewModel viewModel;
        readonly List<ChestView> activeChests = new();

        public void Bind(GameViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public async UniTask InitializeAsync()
        {
            this.chestPool = new (this.targetPrefab, this.targetContainer);
            // allocate based on the initial config
            await this.chestPool.CreateNewByBatchAsync(this.viewModel.TargetsViewModels.Count);

            // bind HUD
            this.viewModel.AttemptsUpdated += HandleAttemptsUpdate;
            this.viewModel.GameStarted += HandleGameStarted;
            this.viewModel.GameFinished += HandleGameFinished;
            this.viewModel.TargetsUpdated += HandleTargetsUpdated;
            this.startGameButton.onClick.AddListener(this.HandleStartRound);
            this.startGameFooter.SetActive(true);

            this.HandleTargetsUpdated();
        }

        void HandleTargetsUpdated()
        {
            this.BuildChests().Forget(); // dynamically updates chests in case of config change
        }

        async UniTask BuildChests()
        {
            if (this.activeChests.Count != this.viewModel.TargetsViewModels.Count)
            {
                // return current active views to pool
                foreach (var view in this.activeChests)
                {
                    this.chestPool.Return(view);
                }
                this.activeChests.Clear();

                // fetch from pool
                var viewTasks = new List<UniTask<ChestView>>();
                foreach (var model in this.viewModel.TargetsViewModels)
                {
                    viewTasks.Add(this.chestPool.GetAsync());
                }

                var results = await UniTask.WhenAll(viewTasks);

                // bind the models
                int i = 0;
                foreach (var model in this.viewModel.TargetsViewModels)
                {
                    results[i].Bind(model);
                    this.activeChests.Add(results[i]);
                    i++;
                }
            }
        }

        void HandleStartRound()
        {
            this.startGameFooter.SetActive(false);
            this.attemptsText.gameObject.SetActive(true);
            this.targetContainer.gameObject.SetActive(true);
            this.viewModel.Start();
        }

        void HandleAttemptsUpdate(int count) => this.attemptsText.text = $"Remaining Attempts: {count} / {this.viewModel.MaxAttemptsPerRound}";

        void HandleGameStarted()
        {
            this.resultText.gameObject.SetActive(false);
        }

        void HandleGameFinished(bool victory)
        {
            this.startGameFooter.SetActive(true);
            this.resultText.gameObject.SetActive(true);
            this.resultText.text = victory ? WinMessage : LossMessage;
        }

        void OnDestroy()
        {
            if (this.viewModel != null)
            {
                this.viewModel.AttemptsUpdated -= HandleAttemptsUpdate;
                this.viewModel.GameStarted -= HandleGameStarted;
                this.viewModel.GameFinished -= HandleGameFinished;
                this.viewModel.TargetsUpdated -= HandleTargetsUpdated;
            }

            this.chestPool?.Dispose();
            this.startGameButton.onClick.RemoveAllListeners();
        }
    }
}