using System;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Market
{
    public sealed class PurchaseServiceTests
    {
        private PurchaseService _service;
        private PlayerInventory _inventory;

        [SetUp]
        public void SetUp()
        {
            _service = new PurchaseService();
            _inventory = new PlayerInventory();
        }

        [Test]
        public void Purchase_WithAffordableOffer_CompletesTransaction()
        {
            var wallet = new Wallet(10);
            var offer = CreateOffer(1, CreateItem("ember_blade", 4));

            var result = _service.Purchase(offer, wallet, _inventory);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Status, Is.EqualTo(PurchaseStatus.Success));
            Assert.That(result.OfferId, Is.EqualTo(offer.Id));
            Assert.That(result.ItemId, Is.EqualTo(offer.Item.Id));
            Assert.That(result.RemainingCoins, Is.EqualTo(6));
            Assert.That(result.OwnedQuantity, Is.EqualTo(1));
            Assert.That(wallet.Balance, Is.EqualTo(6));
            Assert.That(_inventory.GetQuantity(offer.Item.Id), Is.EqualTo(1));
            Assert.That(offer.IsPurchased, Is.True);
        }

        [Test]
        public void Purchase_WithInsufficientFunds_LeavesAllStateUnchanged()
        {
            var wallet = new Wallet(3);
            var offer = CreateOffer(1, CreateItem("ember_blade", 4));

            var result = _service.Purchase(offer, wallet, _inventory);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Status, Is.EqualTo(PurchaseStatus.InsufficientFunds));
            Assert.That(result.RemainingCoins, Is.EqualTo(3));
            Assert.That(result.OwnedQuantity, Is.Zero);
            Assert.That(wallet.Balance, Is.EqualTo(3));
            Assert.That(_inventory.TotalItemCount, Is.Zero);
            Assert.That(offer.IsPurchased, Is.False);
        }

        [Test]
        public void Purchase_WithAlreadyPurchasedOffer_DoesNotChargeTwice()
        {
            var wallet = new Wallet(10);
            var offer = CreateOffer(1, CreateItem("ember_blade", 4));
            _service.Purchase(offer, wallet, _inventory);

            var secondResult = _service.Purchase(offer, wallet, _inventory);

            Assert.That(secondResult.Succeeded, Is.False);
            Assert.That(secondResult.Status, Is.EqualTo(PurchaseStatus.AlreadyPurchased));
            Assert.That(secondResult.RemainingCoins, Is.EqualTo(6));
            Assert.That(secondResult.OwnedQuantity, Is.EqualTo(1));
            Assert.That(wallet.Balance, Is.EqualTo(6));
            Assert.That(_inventory.GetQuantity(offer.Item.Id), Is.EqualTo(1));
        }

        [Test]
        public void Purchase_WithDistinctOffersForSameItem_StacksInventory()
        {
            var wallet = new Wallet(10);
            var item = CreateItem("ember_blade", 4);
            var firstOffer = CreateOffer(1, item);
            var secondOffer = CreateOffer(2, item);

            var firstResult = _service.Purchase(firstOffer, wallet, _inventory);
            var secondResult = _service.Purchase(secondOffer, wallet, _inventory);

            Assert.That(firstResult.OwnedQuantity, Is.EqualTo(1));
            Assert.That(secondResult.OwnedQuantity, Is.EqualTo(2));
            Assert.That(firstResult.RemainingCoins, Is.EqualTo(6));
            Assert.That(secondResult.RemainingCoins, Is.EqualTo(2));
            Assert.That(_inventory.UniqueItemCount, Is.EqualTo(1));
            Assert.That(_inventory.TotalItemCount, Is.EqualTo(2));
        }

        [Test]
        public void Purchase_WhenInventoryCannotAddItem_DoesNotSpendCoinsOrOffer()
        {
            var wallet = new Wallet(10);
            var item = CreateItem("ember_blade", 4);
            var offer = CreateOffer(1, item);
            _inventory.Add(item.Id, int.MaxValue);

            Assert.That(
                () => _service.Purchase(offer, wallet, _inventory),
                Throws.TypeOf<OverflowException>());
            Assert.That(wallet.Balance, Is.EqualTo(10));
            Assert.That(offer.IsPurchased, Is.False);
            Assert.That(_inventory.GetQuantity(item.Id), Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void Purchase_WithMissingDependency_Throws()
        {
            var wallet = new Wallet(10);
            var offer = CreateOffer(1, CreateItem("ember_blade", 4));

            Assert.That(
                () => _service.Purchase(null, wallet, _inventory),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => _service.Purchase(offer, null, _inventory),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => _service.Purchase(offer, wallet, null),
                Throws.TypeOf<ArgumentNullException>());
        }

        private static MarketOffer CreateOffer(long id, ItemData item)
        {
            return new MarketOffer(new MarketOfferId(id), item);
        }

        private static ItemData CreateItem(string id, int price)
        {
            return new ItemData(
                new ItemId(id),
                "Test Item",
                "An item used by the purchase service tests.",
                price,
                5,
                ItemRarity.Common,
                ItemCategory.Relic,
                new[] { new ItemTagId("test_tag") });
        }
    }
}
