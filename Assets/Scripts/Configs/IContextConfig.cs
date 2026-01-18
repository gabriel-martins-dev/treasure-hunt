namespace TreasureHunt.Configs
{
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

    public interface IRewardDefinition
    {
        string Name { get; }
        int MinAmount { get; }
        int MaxAmount { get; }
    }
}