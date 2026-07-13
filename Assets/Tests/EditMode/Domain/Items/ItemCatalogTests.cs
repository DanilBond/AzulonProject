using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Items
{
    public sealed class ItemCatalogTests
    {
        [Test]
        public void Constructor_WithValidData_AllowsTypedLookup()
        {
            var fire = new ItemTagData(new ItemTagId("fire"), "Fire");
            var item = CreateItem("ember_blade", fire.Id);
            var catalog = new ItemCatalog(new[] { fire }, new[] { item });

            var itemFound = catalog.TryGetItem(item.Id, out var foundItem);
            var tagFound = catalog.TryGetTag(fire.Id, out var foundTag);

            Assert.That(itemFound, Is.True);
            Assert.That(foundItem, Is.SameAs(item));
            Assert.That(tagFound, Is.True);
            Assert.That(foundTag, Is.SameAs(fire));
        }

        [Test]
        public void Constructor_WithDuplicateItemId_Throws()
        {
            var fire = new ItemTagData(new ItemTagId("fire"), "Fire");
            var first = CreateItem("ember_blade", fire.Id);
            var second = CreateItem("ember_blade", fire.Id);

            Assert.That(
                () => new ItemCatalog(new[] { fire }, new[] { first, second }),
                Throws.ArgumentException);
        }

        [Test]
        public void Constructor_WithUnknownItemTag_Throws()
        {
            var fire = new ItemTagData(new ItemTagId("fire"), "Fire");
            var item = CreateItem("tidal_blade", new ItemTagId("water"));

            Assert.That(
                () => new ItemCatalog(new[] { fire }, new[] { item }),
                Throws.ArgumentException);
        }

        private static ItemData CreateItem(string id, ItemTagId tagId)
        {
            return new ItemData(
                new ItemId(id),
                "Test Item",
                "An item used by the automated tests.",
                3,
                5,
                ItemRarity.Common,
                ItemCategory.Relic,
                new[] { tagId });
        }
    }
}
