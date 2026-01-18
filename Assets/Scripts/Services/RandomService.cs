namespace TreasureHunt.Services
{
    public interface IRandomService
    {
        int Range(int min, int max);
    }

    public class RandomService : IRandomService
    {
        public int Range(int min, int max) => UnityEngine.Random.Range(min, max);
    }
}