using System;
using System.Collections.Generic;
using Azulon.Domain.Randomness;

namespace Azulon.Tests.EditMode.Domain.Market
{
    internal sealed class SequenceRandomSource : IRandomSource
    {
        private readonly Queue<int> _indices;

        public SequenceRandomSource(params int[] indices)
        {
            _indices = new Queue<int>(indices ?? throw new ArgumentNullException(nameof(indices)));
        }

        public int NextIndex(int exclusiveUpperBound)
        {
            if (_indices.Count == 0)
            {
                throw new InvalidOperationException("Test random sequence has no indices left.");
            }

            return _indices.Dequeue();
        }
    }
}
