using Azulon.Configuration.Items;
using Azulon.Configuration.Items.Validation;
using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class ItemCatalogValidatorTests
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
        public void Validate_WithCompleteCatalog_ReturnsValidResult()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var item = CreateValidItem("ember_blade", fire);
            var catalog = _factory.CreateCatalog(new[] { fire }, new[] { item });

            var result = ItemCatalogValidator.Validate(catalog);

            Assert.That(result.IsValid, Is.True, result.FormatErrors());
            Assert.That(result.Issues, Is.Empty);
        }

        [Test]
        public void Validate_WithDuplicateItemIds_ReportsError()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var first = CreateValidItem("ember_blade", fire);
            var second = CreateValidItem("ember_blade", fire);
            var catalog = _factory.CreateCatalog(new[] { fire }, new[] { first, second });

            var result = ItemCatalogValidator.Validate(catalog);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ContainsIssue(result, "Duplicate item ID 'ember_blade'"), Is.True);
        }

        [Test]
        public void Validate_WithUnregisteredTag_ReportsError()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var weapon = _factory.CreateTag("weapon", "Weapon");
            var item = CreateValidItem("ember_blade", weapon);
            var catalog = _factory.CreateCatalog(new[] { fire }, new[] { item });

            var result = ItemCatalogValidator.Validate(catalog);

            Assert.That(result.IsValid, Is.False);
            Assert.That(ContainsIssue(result, "is not registered in the catalog"), Is.True);
        }

        [Test]
        public void Validate_WithMissingPresentationData_ReportsEachError()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var item = _factory.CreateItem(
                "ember_blade",
                string.Empty,
                string.Empty,
                null,
                3,
                5,
                ItemRarity.Common,
                ItemCategory.Weapon,
                fire);
            var catalog = _factory.CreateCatalog(new[] { fire }, new[] { item });

            var result = ItemCatalogValidator.Validate(catalog);

            Assert.That(ContainsIssue(result, "has no display name"), Is.True);
            Assert.That(ContainsIssue(result, "has no description"), Is.True);
            Assert.That(ContainsIssue(result, "has no icon"), Is.True);
        }

        private ItemDefinition CreateValidItem(string id, ItemTagDefinition tag)
        {
            return _factory.CreateItem(
                id,
                "Ember Blade",
                "A blade holding a steady magical flame.",
                _factory.CreateIcon(),
                4,
                7,
                ItemRarity.Uncommon,
                ItemCategory.Weapon,
                tag);
        }

        private static bool ContainsIssue(ItemCatalogValidationResult result, string text)
        {
            foreach (var issue in result.Issues)
            {
                if (issue.Message.Contains(text))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
