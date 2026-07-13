using System;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Inventory
{
    public sealed class PlayerInventoryTests
    {
        [Test]
        public void NewInventory_IsEmpty()
        {
            var inventory = new PlayerInventory();

            Assert.That(inventory.TotalItemCount, Is.Zero);
            Assert.That(inventory.UniqueItemCount, Is.Zero);
            Assert.That(inventory.CreateSnapshot(), Is.Empty);
        }

        [Test]
        public void Add_WithRepeatedItem_StacksQuantity()
        {
            var inventory = new PlayerInventory();
            var emberBlade = new ItemId("ember_blade");

            inventory.Add(emberBlade);
            inventory.Add(emberBlade, 2);

            Assert.That(inventory.Contains(emberBlade), Is.True);
            Assert.That(inventory.GetQuantity(emberBlade), Is.EqualTo(3));
            Assert.That(inventory.TotalItemCount, Is.EqualTo(3));
            Assert.That(inventory.UniqueItemCount, Is.EqualTo(1));
        }

        [Test]
        public void CreateSnapshot_PreservesFirstAcquisitionOrder()
        {
            var inventory = new PlayerInventory();
            var emberBlade = new ItemId("ember_blade");
            var sunCharm = new ItemId("sun_charm");

            inventory.Add(emberBlade);
            inventory.Add(sunCharm);
            inventory.Add(emberBlade);

            var snapshot = inventory.CreateSnapshot();

            Assert.That(snapshot.Count, Is.EqualTo(2));
            Assert.That(snapshot[0].ItemId, Is.EqualTo(emberBlade));
            Assert.That(snapshot[0].Quantity, Is.EqualTo(2));
            Assert.That(snapshot[1].ItemId, Is.EqualTo(sunCharm));
            Assert.That(snapshot[1].Quantity, Is.EqualTo(1));
        }

        [Test]
        public void CreateSnapshot_DoesNotChangeAfterLaterInventoryUpdates()
        {
            var inventory = new PlayerInventory();
            var emberBlade = new ItemId("ember_blade");
            inventory.Add(emberBlade);
            var snapshot = inventory.CreateSnapshot();

            inventory.Add(emberBlade);

            Assert.That(snapshot[0].Quantity, Is.EqualTo(1));
            Assert.That(inventory.GetQuantity(emberBlade), Is.EqualTo(2));
        }

        [Test]
        public void GetQuantity_WithUnknownItem_ReturnsZero()
        {
            var inventory = new PlayerInventory();

            Assert.That(inventory.GetQuantity(new ItemId("unknown_item")), Is.Zero);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Add_WithNonPositiveQuantity_ThrowsWithoutMutation(int quantity)
        {
            var inventory = new PlayerInventory();

            Assert.That(
                () => inventory.Add(new ItemId("ember_blade"), quantity),
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(inventory.TotalItemCount, Is.Zero);
        }

        [Test]
        public void Add_WhenCountWouldOverflow_ThrowsWithoutMutation()
        {
            var inventory = new PlayerInventory();
            var emberBlade = new ItemId("ember_blade");
            inventory.Add(emberBlade, int.MaxValue);

            Assert.That(() => inventory.Add(emberBlade), Throws.TypeOf<OverflowException>());
            Assert.That(inventory.GetQuantity(emberBlade), Is.EqualTo(int.MaxValue));
            Assert.That(inventory.TotalItemCount, Is.EqualTo(int.MaxValue));
        }
    }
}
