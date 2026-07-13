using System;
using Azulon.Domain.Randomness;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Randomness
{
    public sealed class SystemRandomSourceTests
    {
        [Test]
        public void NextIndex_WithSameSeed_ProducesSameSequence()
        {
            var first = new SystemRandomSource(1729);
            var second = new SystemRandomSource(1729);

            for (var index = 0; index < 20; index++)
            {
                Assert.That(first.NextIndex(100), Is.EqualTo(second.NextIndex(100)));
            }
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void NextIndex_WithNonPositiveBound_Throws(int exclusiveUpperBound)
        {
            var random = new SystemRandomSource(1729);

            Assert.That(
                () => random.NextIndex(exclusiveUpperBound),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
