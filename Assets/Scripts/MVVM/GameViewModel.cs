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
        public event Action GameStarted;
        public event Action TargetsUpdated;
        public event Action<int> AttemptsUpdated;
        public event Action<bool> GameFinished;
        public event Action<ResourceUpdateEvent> ResourceUpdate;
        public IReadOnlyCollection<ChestViewModel> TargetsViewModels => this.targetViewModels;
        public int MaxAttemptsPerRound => this.roundConfig.MaxAttemptsPerRound;

        readonly IGameMode gameMode;
        readonly IRoundConfig roundConfig;
        readonly IWalletService walletService;
        ChestViewModel[] targetViewModels;
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

            this.gameMode.AttemptsUpdated += this.HandleAttemptsUpdated;
            this.gameMode.GameCompleted += this.HandleGameFinished;
            this.walletService.ResourceUpdated += this.HandleResourcesUpdated;
        }

        public void Start()
        {
            this.gameMode.StartGame();
            // recreate view models if config is changed
            if (this.targetViewModels.Length != this.roundConfig.NumberOfChestsPerRound)
            {
                this.targetViewModels = new ChestViewModel[this.roundConfig.NumberOfChestsPerRound];
                for (int i = 0; i < this.targetViewModels.Length; i++)
                {
                    var chestViewModel = new ChestViewModel(i);
                    chestViewModel.Clicked += () => { this.OnOpenClicked(chestViewModel.Index).Forget(); };
                    this.targetViewModels[i] = chestViewModel;
                    this.targetViewModels[i].SetResult(this.gameMode.VictoryTargetIndex == this.targetViewModels[i].Index);
                }

                this.TargetsUpdated?.Invoke();
            }
            else // if config didn't change, just reset
            {
                for (int i = 0; i < this.targetViewModels.Length; i++)
                {
                    this.targetViewModels[i].SetState(ChestState.Idle);
                    this.targetViewModels[i].SetResult(this.gameMode.VictoryTargetIndex == this.targetViewModels[i].Index);
                }
            }

            this.GameStarted?.Invoke();
        }

        public async UniTaskVoid OnOpenClicked(int index)
        {
            var target = this.targetViewModels[index];

            // ignore if already opened or opening
            if (target.State == ChestState.Opened) return;

            // should cancel already opening process
            var cancelOpening = target.State == ChestState.Opening;
            // cancel existing opening process
            this.CancelCurrentOpening();

            if(!cancelOpening)
            {
                // start the new opening process
                this.cancellationTokenSource = new CancellationTokenSource();
                await DoOpenSequence(target, this.cancellationTokenSource.Token);
            }
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

        void HandleAttemptsUpdated(int val)
        {
            this.AttemptsUpdated?.Invoke(val);
        }

        void HandleGameFinished(bool win)
        {
            for (int i = 0; i < this.targetViewModels.Length; i++)
            {
                this.targetViewModels[i].SetState(ChestState.Locked);
            }

            this.GameFinished?.Invoke(win);
        }

        void HandleResourcesUpdated(ResourceUpdateEvent resourceUptEvent)
        {
            this.ResourceUpdate?.Invoke(resourceUptEvent);
        }

        public void Dispose()
        {
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource?.Dispose();
            this.gameMode.AttemptsUpdated -= this.HandleAttemptsUpdated;
            this.gameMode.GameCompleted -= this.HandleGameFinished;
            this.walletService.ResourceUpdated -= this.HandleResourcesUpdated;
        }
    }
}
