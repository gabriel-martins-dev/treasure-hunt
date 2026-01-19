namespace TreasureHunt.View
{
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;

    public class ChestPool
    {
        readonly ChestView prefab;
        readonly Transform container;
        readonly ObjectPool<ChestView> pool;

        public ChestPool(ChestView prefab, Transform container)
        {
            this.prefab = prefab;
            this.container = container;

            this.pool = new ObjectPool<ChestView>(
                createFunc: () => {
                    return Object.Instantiate(this.prefab, this.container);
                },
                actionOnGet: (chest) => chest.gameObject.SetActive(true),
                actionOnRelease: (chest) => chest.gameObject.SetActive(false),
                actionOnDestroy: (chest) => Object.Destroy(chest.gameObject),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 20
            );
        }

        async UniTask<ChestView> CreateNewAsync()
        {
            var op = await Object.InstantiateAsync(this.prefab, this.container);
            return op[0];
        }

        public async UniTask CreateNewByBatchAsync(int count)
        {
            var tasks = new List<UniTask<ChestView>>();
            for (int i = 0; i < count; i++)
            {
                tasks.Add(CreateNewAsync());
            }

            var results = await UniTask.WhenAll(tasks);
            foreach (var chest in results)
            {
                this.pool.Release(chest);
            }
        }

        public async UniTask<ChestView> GetAsync()
        {
            if (this.pool.CountInactive > 0)
            {
                return this.pool.Get();
            }

            var newChest = await this.CreateNewAsync();
            return newChest;
        }

        public void Return(ChestView chest) => this.pool.Release(chest);
    }
}