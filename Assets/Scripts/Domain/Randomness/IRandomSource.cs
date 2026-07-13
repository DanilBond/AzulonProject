namespace Azulon.Domain.Randomness
{
    public interface IRandomSource
    {
        int NextIndex(int exclusiveUpperBound);
    }
}
