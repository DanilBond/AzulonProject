using System;
using System.Collections.Generic;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Randomness;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class MarketOfferGeneratorTests
    {
        [Test]
        public void Generate_WithEnoughItems_ReturnsRequestedUniqueOffers()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(0, 0, 0));

            var result = generator.Generate(catalog, ItemRarity.Common, 3);

            Assert.That(result.Offers.Count, Is.EqualTo(3));
            Assert.That(result.AvailableOfferCount, Is.EqualTo(3));
            AssertUniqueIdsAndItems(result);
        }

        [Test]
        public void Generate_WithSameSeed_ProducesSameOffers()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common);
            var firstGenerator = CreateGenerator(new SystemRandomSource(1729));
            var secondGenerator = CreateGenerator(new SystemRandomSource(1729));

            var first = firstGenerator.Generate(catalog, ItemRarity.Common, 3);
            var second = secondGenerator.Generate(catalog, ItemRarity.Common, 3);

            for (var index = 0; index < first.Offers.Count; index++)
            {
                Assert.That(first.Offers[index].Id, Is.EqualTo(second.Offers[index].Id));
                Assert.That(first.Offers[index].Item.Id, Is.EqualTo(second.Offers[index].Item.Id));
            }
        }

        [Test]
        public void Generate_FiltersLockedRaritiesAndCapsCountToEligiblePool()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Uncommon,
                ItemRarity.Rare,
                ItemRarity.Epic);
            var generator = CreateGenerator(new SequenceRandomSource(0, 0));

            var result = generator.Generate(catalog, ItemRarity.Uncommon, 10);

            Assert.That(result.Offers.Count, Is.EqualTo(2));
            foreach (var offer in result.Offers)
            {
                Assert.That((int)offer.Item.Rarity, Is.LessThanOrEqualTo((int)ItemRarity.Uncommon));
            }
        }

        [Test]
        public void Generate_ForFollowingMerchant_AllowsItemToReappearWithNewOfferId()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(0, 0));

            var firstMerchant = generator.Generate(catalog, ItemRarity.Common, 1);
            var secondMerchant = generator.Generate(catalog, ItemRarity.Common, 1);

            Assert.That(
                firstMerchant.Offers[0].Item.Id,
                Is.EqualTo(secondMerchant.Offers[0].Item.Id));
            Assert.That(firstMerchant.Offers[0].Id.Value, Is.EqualTo(1));
            Assert.That(secondMerchant.Offers[0].Id.Value, Is.EqualTo(2));
            Assert.That(secondMerchant.Offers[0].IsPurchased, Is.False);
        }

        [Test]
        public void Generate_WithoutEligibleItems_ThrowsDescriptiveError()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Rare);
            var generator = CreateGenerator(new SequenceRandomSource());

            Assert.That(
                () => generator.Generate(catalog, ItemRarity.Common, 1),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("no items available"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Generate_WithNonPositiveRequestedCount_Throws(int count)
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource());

            Assert.That(
                () => generator.Generate(catalog, ItemRarity.Common, count),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Generate_WhenRandomSourceBreaksContract_ThrowsDescriptiveError()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(1));

            Assert.That(
                () => generator.Generate(catalog, ItemRarity.Common, 1),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("returned index 1"));
        }

        [Test]
        public void Generate_WithInvalidCatalogOrRarity_Throws()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource());

            Assert.That(
                () => generator.Generate(null, ItemRarity.Common, 1),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => generator.Generate(catalog, (ItemRarity)999, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_WithMissingDependency_Throws()
        {
            var random = new SystemRandomSource(1729);
            var ids = new SequentialMarketOfferIdSource();

            Assert.That(
                () => new MarketOfferGenerator(null, ids),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => new MarketOfferGenerator(random, null),
                Throws.TypeOf<ArgumentNullException>());
        }

        private static MarketOfferGenerator CreateGenerator(IRandomSource randomSource)
        {
            return new MarketOfferGenerator(
                randomSource,
                new SequentialMarketOfferIdSource());
        }

        private static void AssertUniqueIdsAndItems(MarketOfferSet offers)
        {
            var offerIds = new HashSet<MarketOfferId>();
            var itemIds = new HashSet<ItemId>();
            foreach (var offer in offers.Offers)
            {
                Assert.That(offerIds.Add(offer.Id), Is.True);
                Assert.That(itemIds.Add(offer.Item.Id), Is.True);
            }
        }
    }
}
