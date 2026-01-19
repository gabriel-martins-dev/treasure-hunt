namespace TreasureHunt.View
{
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using TMPro;
    using TreasureHunt.Presentation;
    using UnityEngine;
    using UnityEngine.UI;

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

        GameViewModel viewModel;

        public void Bind(GameViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public async UniTask InitializeAsync()
        {
            //TRADEOFF: directly instantiating targets here for simplicity. A better solution would be to use Factory and Pooling
            // to track all the instantiation tasks
            var spawnTasks = new List<UniTask<ChestView>>();

            foreach (var chestModel in this.viewModel.TargetsViewModels)
            {
                // InstantiateAsync returns an AsyncInstantiateOperation
                // We can convert this to a UniTask
                var task = InstantiateAsync(this.targetPrefab, this.targetContainer)
                    .ToUniTask()
                    .ContinueWith(op => {
                        var view = op[0];
                        view.Bind(chestModel);
                        return view;
                    });

                spawnTasks.Add(task);
            }

            // await all instantiations
            await UniTask.WhenAll(spawnTasks);

            // bind HUD
            this.viewModel.AttemptsUpdated += HandleAttemptsUpdate;
            this.viewModel.GameStarted += HandleGameStarted;
            this.viewModel.GameFinished += HandleGameFinished;
            this.startGameButton.onClick.AddListener(this.StartRound);
            this.startGameFooter.SetActive(true);
        }

        void StartRound()
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
            Debug.Log("Victory: " + victory);
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
            }
        }
    }
}