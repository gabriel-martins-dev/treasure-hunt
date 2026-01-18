namespace TreasureHunt.Root
{
    using TreasureHunt.Configs;
    using UnityEngine;

    /// <summary>
    /// This is the game root, the application's entry point
    /// Responsibilities: Initialization, Injection and Binding
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameContextConfig gameContextConfig;

        private void Awake()
        {
            Debug.Log($"context config: {JsonUtility.ToJson(this.gameContextConfig, true)}");
        }
    }

}