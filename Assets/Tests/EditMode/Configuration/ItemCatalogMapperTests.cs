using System;
using Azulon.Configuration.Items;
using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class ItemCatalogMapperTests
    {
        private ScriptableObjectTestFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new ScriptableObjectTestFactory();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public void ToDomain_WithValidConfiguration_CreatesIndependentCatalog()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var weapon = _factory.CreateTag("weapon", "Weapon");
            var item = _factory.CreateItem(
                "ember_blade",
                "Ember Blade",
                "A blade holding a steady magical flame.",
                _factory.CreateIcon(),
                4,
                7,
                ItemRarity.Uncommon,
                ItemCategory.Weapon,
                fire,
                weapon);
            var catalogAsset = _factory.CreateCatalog(
                new[] { fire, weapon },
                new[] { item });

            var catalog = ItemCatalogMapper.ToDomain(catalogAsset);

            Assert.That(catalog.TryGetItem(new ItemId("ember_blade"), out var mappedItem), Is.True);
            Assert.That(mappedItem.DisplayName, Is.EqualTo("Ember Blade"));
            Assert.That(mappedItem.HasTag(new ItemTagId("fire")), Is.True);
            Assert.That(mappedItem.HasTag(new ItemTagId("weapon")), Is.True);
        }

        [Test]
        public void ToDomain_WithInvalidConfiguration_ThrowsDescriptiveException()
        {
            var emptyCatalog = _factory.CreateCatalog(
                new ItemTagDefinition[0],
                new ItemDefinition[0]);

            Assert.That(
                () => ItemCatalogMapper.ToDomain(emptyCatalog),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Catalog must contain at least one tag definition"));
        }
    }
}
