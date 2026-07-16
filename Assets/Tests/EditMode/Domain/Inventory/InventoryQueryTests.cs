using System;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Tests.EditMode.Domain.Quests;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Inventory
{
    public sealed class InventoryQueryTests
    {
        [Test]
        public void Queries_CountStacksTagsAndTotalPower()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id, 2);
            inventory.Add(context.FlameOrb.Id);
            inventory.Add(context.TideCharm.Id);
            var query = new InventoryQuery(inventory, context.Catalog);

            Assert.That(query.GetItemQuantity(context.EmberBlade.Id), Is.EqualTo(2));
            Assert.That(query.CountUniqueItems(), Is.EqualTo(3));
            Assert.That(query.CountItemsWithTag(context.FireTagId), Is.EqualTo(3));
            Assert.That(query.CountItemsWithTag(context.WeaponTagId), Is.EqualTo(2));
            Assert.That(query.CalculateTotalPower(), Is.EqualTo(23));
        }

        [Test]
        public void Query_RemainsStableAfterInventoryChanges()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id);
            var query = new InventoryQuery(inventory, context.Catalog);

            inventory.Add(context.EmberBlade.Id);

            Assert.That(query.GetItemQuantity(context.EmberBlade.Id), Is.EqualTo(1));
            Assert.That(query.CountUniqueItems(), Is.EqualTo(1));
            Assert.That(query.CalculateTotalPower(), Is.EqualTo(7));
        }

        [Test]
        public void Query_WithInventoryItemOutsideCatalog_Throws()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(new ItemId("unknown_item"));
            var query = new InventoryQuery(inventory, context.Catalog);

            Assert.That(
                () => query.CalculateTotalPower(),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("unknown_item"));
            Assert.That(
                () => query.CountUniqueItems(),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("unknown_item"));
        }
    }
}
