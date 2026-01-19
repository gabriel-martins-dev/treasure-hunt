namespace TreasureHunt.Services
{
    /// <summary>
    /// Strcuture for random, required for testing 
    /// </summary>
    public interface IRandomService
    {
        int Range(int min, int max);
    }

    /// <summary>
    /// Simple wrapper for Unity's Random
    /// </summary>
    public class RandomService : IRandomService
    {
        public int Range(int min, int max) => UnityEngine.Random.Range(min, max);
    }
}