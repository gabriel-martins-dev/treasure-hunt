namespace TreasureHunt.GameMode
{
    using System;

    public interface IGameMode
    {
        event Action<int> AttemptsUpdated;
        event Action<bool> GameCompleted;
        void StartGame();
        void OpenAction(int openIndex);
    }
}