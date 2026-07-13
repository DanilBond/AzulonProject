using System;
using Azulon.Domain.Market;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class SequentialMarketOfferIdSourceTests
    {
        [Test]
        public void NextId_ReturnsIncreasingIdsFromConfiguredValue()
        {
            var source = new SequentialMarketOfferIdSource(40);

            Assert.That(source.NextId().Value, Is.EqualTo(40));
            Assert.That(source.NextId().Value, Is.EqualTo(41));
            Assert.That(source.NextId().Value, Is.EqualTo(42));
        }

        [TestCase(0L)]
        [TestCase(-1L)]
        public void Constructor_WithNonPositiveFirstValue_Throws(long firstValue)
        {
            Assert.That(
                () => new SequentialMarketOfferIdSource(firstValue),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void NextId_AfterMaximumValue_ThrowsWithoutWrapping()
        {
            var source = new SequentialMarketOfferIdSource(long.MaxValue);

            Assert.That(source.NextId().Value, Is.EqualTo(long.MaxValue));
            Assert.That(() => source.NextId(), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
