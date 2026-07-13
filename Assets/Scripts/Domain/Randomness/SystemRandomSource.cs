using System;

namespace Azulon.Domain.Randomness
{
    public sealed class SystemRandomSource : IRandomSource
    {
        private readonly Random _random;

        public SystemRandomSource()
            : this(new Random())
        {
        }

        public SystemRandomSource(int seed)
            : this(new Random(seed))
        {
        }

        private SystemRandomSource(Random random)
        {
            _random = random;
        }

        public int NextIndex(int exclusiveUpperBound)
        {
            if (exclusiveUpperBound <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(exclusiveUpperBound),
                    "Random upper bound must be greater than zero.");
            }

            return _random.Next(exclusiveUpperBound);
        }
    }
}
