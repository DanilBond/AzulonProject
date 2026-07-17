using System.Linq;
using Azulon.Configuration.Game.Validation;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using NUnit.Framework;
using UnityEngine;

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
                marketContext.VisitorSprites,
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

        [Test]
        public void Validate_WithFewerThanTwoVisitorSprites_ReportsVisualError()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var config = _factory.CreateGameSessionConfig(
                context.ItemCatalog,
                context.QuestCatalog,
                4,
                3,
                2,
                new Sprite[0],
                new RarityUnlockThreshold(ItemRarity.Common, 0));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Issues.Any(issue =>
                    issue.Message.Contains("At least two visitor sprites")),
                Is.True);
        }

        [Test]
        public void Validate_WithDuplicateVisitorSprite_ReportsVisualError()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var sprite = _factory.CreateIcon();
            var config = _factory.CreateGameSessionConfig(
                context.ItemCatalog,
                context.QuestCatalog,
                4,
                3,
                2,
                new[] { sprite, sprite },
                new RarityUnlockThreshold(ItemRarity.Common, 0));

            var result = GameSessionConfigValidator.Validate(config);

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                result.Issues.Any(issue => issue.Message.Contains("duplicated")),
                Is.True);
        }
    }
}
