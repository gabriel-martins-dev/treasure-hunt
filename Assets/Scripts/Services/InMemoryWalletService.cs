namespace TreasureHunt.Services
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IWalletService
    {
        void UpdateResource(string id, int amount);

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

        public void UpdateResource(string id, int amount)
        {
            if (amount < 0) {
                Debug.LogError($"Can't update ${id} reward with {amount}");
                return;
            }

            if (!resource.ContainsKey(id))
            {
                this.resource[id] = 0;
            }
            this.resource[id] += amount;
            this.ResourceUpdated.Invoke(new ResourceUpdateEvent { Name = id, Amount = amount, NewTotal = this.resource[id] });
            Debug.Log($"[InMemoryWallet] Added {amount} to {id}. New Total: {this.resource[id]}");
        }
    }
}