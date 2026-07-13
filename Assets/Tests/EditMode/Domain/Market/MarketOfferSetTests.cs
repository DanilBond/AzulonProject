using System;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;
using Azulon.Domain.Market;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class MarketOfferSetTests
    {
        [Test]
        public void Constructor_WithDuplicateOfferId_Throws()
        {
            var first = CreateOffer(1, "first_item");
            var second = CreateOffer(1, "second_item");

            Assert.That(
                () => new MarketOfferSet(new[] { first, second }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Constructor_WithDuplicateItem_Throws()
        {
            var item = MarketTestCatalogFactory.CreateItem("shared_item");
            var first = new MarketOffer(new MarketOfferId(1), item);
            var second = new MarketOffer(new MarketOfferId(2), item);

            Assert.That(
                () => new MarketOfferSet(new[] { first, second }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Availability_TracksPurchasedOffers()
        {
            var first = CreateOffer(1, "first_item");
            var second = CreateOffer(2, "second_item");
            var offers = new MarketOfferSet(new[] { first, second });
            var purchaseService = new PurchaseService();
            var wallet = new Wallet(10);
            var inventory = new PlayerInventory();

            Assert.That(offers.AvailableOfferCount, Is.EqualTo(2));
            Assert.That(offers.TryGetOffer(first.Id, out var found), Is.True);
            Assert.That(found, Is.SameAs(first));

            purchaseService.Purchase(first, wallet, inventory);

            Assert.That(offers.AvailableOfferCount, Is.EqualTo(1));
            Assert.That(offers.IsFullyPurchased, Is.False);

            purchaseService.Purchase(second, wallet, inventory);

            Assert.That(offers.AvailableOfferCount, Is.Zero);
            Assert.That(offers.IsFullyPurchased, Is.True);
        }

        [Test]
        public void Constructor_WithoutOffers_Throws()
        {
            Assert.That(
                () => new MarketOfferSet(new MarketOffer[0]),
                Throws.TypeOf<ArgumentException>());
        }

        private static MarketOffer CreateOffer(long id, string itemId)
        {
            return new MarketOffer(
                new MarketOfferId(id),
                MarketTestCatalogFactory.CreateItem(itemId));
        }
    }
}
