using TreasureHunt.Services;

namespace TreasureHunt.Tests.Mocks
{
    public class MockRewardService : IRewardService
    {
        readonly string name;
        readonly int amount;

        public MockRewardService(string name, int amount) 
        { 
            this.name = name; 
            this.amount = amount;
        }

        public RewardResult? GenerateReward()
        {
            return new RewardResult { Name = this.name, Amount = this.amount };
        }
    }
}