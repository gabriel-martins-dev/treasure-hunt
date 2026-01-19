namespace TreasureHunt.Presentation
{
    using System;

    public enum ChestState { Idle, Opening, Opened, Locked }
    /// <summary>
    /// State for single chest
    /// </summary>
    public class ChestViewModel
    {
        public int Index { get; }
        public bool IsWinner { get; private set; }
        public ChestState State { get; private set; } = ChestState.Idle;

        public event Action<ChestState> StateUpdated;
        public event Action Clicked;

        public ChestViewModel(int index) => Index = index;

        public void SetResult(bool isWinner) => IsWinner = isWinner;

        public void SetState(ChestState newState)
        {
            State = newState;
            StateUpdated?.Invoke(State);
        }

        public void OnClicked()
        {
            this.Clicked?.Invoke();
        }
    }
}