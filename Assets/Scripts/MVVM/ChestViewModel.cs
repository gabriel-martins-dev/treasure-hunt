namespace TreasureHunt.Presentation
{
    using System;

    public enum ChestState { Idle, Opening, Opened, Locked }

    public class ChestViewModel
    {
        public int Index { get; }
        public bool IsWinner { get; private set; }
        public ChestState State { get; private set; } = ChestState.Idle;

        public event Action<ChestState> StateChanged;
        public event Action Clicked;

        public ChestViewModel(int index) => Index = index;

        public void SetResult(bool isWinner) => IsWinner = isWinner;

        public void SetState(ChestState newState)
        {
            State = newState;
            StateChanged?.Invoke(State);
        }

        public void OnClicked()
        {
            if (State != ChestState.Locked)
            {
                this.Clicked?.Invoke();
            }
        }
    }
}