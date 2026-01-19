namespace TreasureHunt.Services
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Structure for player's resource balances
    /// </summary>
    public interface IWalletService
    {
        void UpdateResource(string name, int amount);

        // TRADEOFF: not using IResourceUpdateEvent, would cause boxing. Not super relevant here, but still
        event Action<ResourceUpdateEvent> ResourceUpdated; 
    }

    public struct ResourceUpdateEvent
    {
        public string Name;
        public int NewTotal;
        public int Amount;
    }

    /// <summary>
    /// Manages the player's persistent resource balances during a session
    /// </summary>
    public class InMemoryWalletService : IWalletService
    {
        readonly Dictionary<string, int> resource = new();
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