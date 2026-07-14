using System.Linq;
using Azulon.Configuration.Game.Validation;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class GameSessionConfigValidatorTests
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
        public void Validate_WithCoherentConfiguration_ReturnsValidResult()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.True, result.FormatErrors());
        }

        [Test]
        public void Validate_WithoutCommonItem_ReportsUnreachableFirstVisitor()
        {
            var context = new GameSessionConfigTestContext(
                _factory,
                ItemRarity.Uncommon);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Issues.Any(issue => issue.Message.Contains("at least one Common item")),
                Is.True);
        }

        [Test]
        public void Validate_WithDifferentItemCatalogs_ReportsCompositionError()
        {
            var marketContext = new GameSessionConfigTestContext(
                _factory,
                ItemRarity.Common,
                "market");
            var questContext = new GameSessionConfigTestContext(
                _factory,
                ItemRarity.Common,
                "quests");
            var config = _factory.CreateGameSessionConfig(
                marketContext.ItemCatalog,
                questContext.QuestCatalog,
                4,
                3,
                2,
                3,
                new RarityUnlockThreshold(ItemRarity.Common, 0));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Issues.Any(issue => issue.Message.Contains("same item catalog asset")),
                Is.True);
        }

        [Test]
        public void Validate_WithNonContiguousRarities_ReportsProgressionError()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Rare, 3));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Issues.Any(issue => issue.Message.Contains("contiguous sequence")),
                Is.True);
        }
    }
}
