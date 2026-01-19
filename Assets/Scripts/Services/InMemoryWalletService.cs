namespace TreasureHunt.Services
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IWalletService
    {
        void UpdateResource(string name, int amount);

        // TRADEOFF: using IResourceUpdateEvent would cause boxing. Not super relevant here, but still
        event Action<ResourceUpdateEvent> ResourceUpdated; 
    }

    public struct ResourceUpdateEvent
    {
        public string Name;
        public int NewTotal;
        public int Amount;
    }

    public class InMemoryWalletService : IWalletService
    {
        private readonly Dictionary<string, int> resource = new();
        public event Action<ResourceUpdateEvent> ResourceUpdated;

        public void UpdateResource(string name, int amount)
        {
            if (amount < 0) {
                Debug.LogError($"Can't update ${name} reward with {amount}");
                return;
            }

            if (!resource.ContainsKey(name))
            {
                this.resource[name] = 0;
            }
            this.resource[name] += amount;
            this.ResourceUpdated?.Invoke(new ResourceUpdateEvent { Name = name, Amount = amount, NewTotal = this.resource[name] });
        }
    }
}