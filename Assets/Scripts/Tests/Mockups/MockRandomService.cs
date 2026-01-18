using TreasureHunt.Services;

namespace TreasureHunt.Tests.Mocks
{
    public class MockRandomService : IRandomService
    {
        readonly int mockValue;
        public MockRandomService(int mockValue) { this.mockValue = mockValue; }
        public int Range(int min, int max) => this.mockValue;
    }
}