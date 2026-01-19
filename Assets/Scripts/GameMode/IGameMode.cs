namespace TreasureHunt.GameMode
{
    using System;

    /// <summary>
    /// Defines the core logic for a game session.
    /// </summary>
    public interface IGameMode
    {
        event Action<int> AttemptsUpdated;
        event Action<bool> GameCompleted;

        /// <summary>
        /// For presentation layer reveal the outcome. TRADEOFF: ideally could be retrieved from result open action result, but keeping for simplicity 
        /// </summary>
        int VictoryTargetIndex { get; }

        void StartGame();
        void OpenAction(int openIndex);
    }
}