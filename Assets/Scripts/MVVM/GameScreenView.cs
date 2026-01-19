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
        [SerializeField] Button startGameButton;

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
            this.viewModel.AttemptsChanged += UpdateAttempts;
            this.viewModel.GameFinished += OnGameFinish;
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

        void UpdateAttempts(int count) => this.attemptsText.text = $"Remaining Attempts: {count} / {this.viewModel.MaxAttemptsPerRound}";

        void OnGameFinish(bool won)
        {
            Debug.Log("Won: " + won);
            this.startGameFooter.SetActive(true);
        }
    }
}