using System;
using Azulon.Domain.Market;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class MarketOfferIdTests
    {
        [TestCase(0L)]
        [TestCase(-1L)]
        public void Constructor_WithNonPositiveValue_Throws(long value)
        {
            Assert.That(
                () => new MarketOfferId(value),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Equality_UsesNumericValue()
        {
            var first = new MarketOfferId(12);
            var second = new MarketOfferId(12);
            var different = new MarketOfferId(13);

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
            Assert.That(first != different, Is.True);
        }
    }
}
