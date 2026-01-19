namespace TreasureHunt.Configs
{
    /// <summary>
    /// Joins all game configs for flexibility, should configs come from different places.
    /// Like server or local
    /// </summary>
    public interface IContextConfig: IRewardConfig, IRoundConfig { }

    public interface IRoundConfig
    {
        int NumberOfChestsPerRound { get; }
        int MaxAttemptsPerRound { get; }
        float ChestOpenDuration { get; }
    }

    public interface IRewardConfig
    {
        IRewardDefinition[] RewardDefinitions { get; }
    }

    /// <summary>
    /// Represents a single reward loot data structure
    /// </summary>
    public interface IRewardDefinition
    {
        string Name { get; }
        int MinAmount { get; }
        int MaxAmount { get; }
    }
}