using System;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Randomness;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class MarketOfferGeneratorTests
    {
        [Test]
        public void Generate_WithEligibleItems_ReturnsOneSelectedOffer()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(2));

            var result = generator.Generate(catalog, ItemRarity.Common);

            Assert.That(result.Item.Id, Is.EqualTo(new ItemId("item_2")));
            Assert.That(result.Id.Value, Is.EqualTo(1));
            Assert.That(result.IsPurchased, Is.False);
        }

        [Test]
        public void Generate_WithSameSeed_ProducesSameOffer()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common,
                ItemRarity.Common);
            var firstGenerator = CreateGenerator(new SystemRandomSource(1729));
            var secondGenerator = CreateGenerator(new SystemRandomSource(1729));

            var first = firstGenerator.Generate(catalog, ItemRarity.Common);
            var second = secondGenerator.Generate(catalog, ItemRarity.Common);

            Assert.That(first.Item.Id, Is.EqualTo(second.Item.Id));
            Assert.That(first.Id, Is.EqualTo(second.Id));
        }

        [Test]
        public void Generate_FiltersLockedRarities()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(
                ItemRarity.Common,
                ItemRarity.Uncommon,
                ItemRarity.Rare,
                ItemRarity.Epic);
            var generator = CreateGenerator(new SequenceRandomSource(1));

            var result = generator.Generate(catalog, ItemRarity.Uncommon);

            Assert.That(result.Item.Id, Is.EqualTo(new ItemId("item_1")));
            Assert.That(result.Item.Rarity, Is.EqualTo(ItemRarity.Uncommon));
        }

        [Test]
        public void Generate_ForFollowingMerchant_AllowsItemWithNewOfferId()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(0, 0));

            var firstMerchant = generator.Generate(catalog, ItemRarity.Common);
            var secondMerchant = generator.Generate(catalog, ItemRarity.Common);

            Assert.That(firstMerchant.Item.Id, Is.EqualTo(secondMerchant.Item.Id));
            Assert.That(firstMerchant.Id.Value, Is.EqualTo(1));
            Assert.That(secondMerchant.Id.Value, Is.EqualTo(2));
            Assert.That(secondMerchant.IsPurchased, Is.False);
        }

        [Test]
        public void Generate_WithoutEligibleItems_ThrowsDescriptiveError()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Rare);
            var generator = CreateGenerator(new SequenceRandomSource());

            Assert.That(
                () => generator.Generate(catalog, ItemRarity.Common),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("no items available"));
        }

        [Test]
        public void Generate_WhenRandomSourceBreaksContract_ThrowsDescriptiveError()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource(1));

            Assert.That(
                () => generator.Generate(catalog, ItemRarity.Common),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("returned index 1"));
        }

        [Test]
        public void Generate_WithInvalidCatalogOrRarity_Throws()
        {
            var catalog = MarketTestCatalogFactory.CreateCatalog(ItemRarity.Common);
            var generator = CreateGenerator(new SequenceRandomSource());

            Assert.That(
                () => generator.Generate(null, ItemRarity.Common),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => generator.Generate(catalog, (ItemRarity)999),
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
    }
}
