using TreasureHunt.Configs;

namespace TreasureHunt.Tests.Mocks
{
    public class MockRoundConfig : IRoundConfig
    {
        public int NumberOfChestsPerRound { get; set; } = 9;
        public int MaxAttemptsPerRound { get; set; } = 3;
        public float ChestOpenDuration { get; set; } = 0f;
    }
}