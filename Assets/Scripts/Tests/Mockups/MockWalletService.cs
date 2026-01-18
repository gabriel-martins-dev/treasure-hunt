using System;
using TreasureHunt.Services;

namespace TreasureHunt.Tests.Mocks
{
    public class MockWalletService : IWalletService
    {
        public event Action<ResourceUpdateEvent> ResourceUpdated;

        public void UpdateResource(string name, int amount) {
            this.ResourceUpdated?.Invoke(new ResourceUpdateEvent { Name = name, Amount = amount, NewTotal = amount });
        }
    }
}