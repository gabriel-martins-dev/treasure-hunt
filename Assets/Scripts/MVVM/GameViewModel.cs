namespace TreasureHunt.Presentation
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using TreasureHunt.Configs;
    using TreasureHunt.GameMode;
    using TreasureHunt.Services;

    public class GameViewModel : IDisposable
    {
        public event Action<int> AttemptsChanged;
        public event Action<bool> GameFinished;
        public event Action<ResourceUpdateEvent> ResourceUpdate;
        public IReadOnlyCollection<ChestViewModel> TargetsViewModels => this.targetViewModels;
        public int MaxAttemptsPerRound => this.roundConfig.MaxAttemptsPerRound;

        readonly IGameMode gameMode;
        readonly IRoundConfig roundConfig;
        readonly IWalletService walletService;
        readonly ChestViewModel[] targetViewModels;

        CancellationTokenSource cancellationTokenSource;

        public GameViewModel(IRoundConfig roundConfig, IGameMode gameMode, IWalletService walletService)
        {
            this.gameMode = gameMode ?? throw new ArgumentNullException(nameof(gameMode));
            this.roundConfig = roundConfig ?? throw new ArgumentNullException(nameof(roundConfig));
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));

            this.targetViewModels = new ChestViewModel[this.roundConfig.NumberOfChestsPerRound];
            for (int i = 0; i < this.targetViewModels.Length; i++)
            {
                var chestViewModel = new ChestViewModel(i);
                chestViewModel.Clicked += () => { this.OnOpenClicked(chestViewModel.Index).Forget(); };
                this.targetViewModels[i] = chestViewModel;
            }

            this.gameMode.AttemptsUpdated += (val) => this.AttemptsChanged?.Invoke(val);
            this.gameMode.GameCompleted += (win) => this.OnGameFinished(win);
            this.walletService.ResourceUpdated += (resourceUptEvent) =>
            {
                this.ResourceUpdate.Invoke(resourceUptEvent);
            };
        }

        public void Start()
        {
            for (int i = 0; i < this.targetViewModels.Length; i++)
            {
                this.targetViewModels[i].SetState(ChestState.Idle);
            }
            this.gameMode.StartGame();
        }

        public async UniTaskVoid OnOpenClicked(int index)
        {
            var target = this.targetViewModels[index];

            // ignore if already opened or opening
            if (target.State == ChestState.Opened || target.State == ChestState.Opening) return;

            // cancel existing opening process
            this.CancelCurrentOpening();

            // start the new opening process
            this.cancellationTokenSource = new CancellationTokenSource();
            await DoOpenSequence(target, this.cancellationTokenSource.Token);
        }

        async UniTask DoOpenSequence(ChestViewModel target, CancellationToken token)
        {
            target.SetState(ChestState.Opening);

            var canceled = await UniTask.Delay(
                TimeSpan.FromSeconds(this.roundConfig.ChestOpenDuration),
                cancellationToken: token
            ).SuppressCancellationThrow();

            if (canceled)
            {
                UnityEngine.Debug.Log("CANCELED!");
                if (target.State == ChestState.Opening)
                {
                    target.SetState(ChestState.Idle);
                }
                return;
            }

            target.SetState(ChestState.Opened);
            gameMode.OpenAction(target.Index);
        }

        void CancelCurrentOpening()
        {
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = null;
        }

        void OnGameFinished(bool win)
        {
            for (int i = 0; i < this.targetViewModels.Length; i++)
            {
                this.targetViewModels[i].SetState(ChestState.Locked);
            }

            this.GameFinished?.Invoke(win);
        }

        public void Dispose()
        {
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource?.Dispose();
        }
    }
}
