using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Configuration.Quests.Validation;
using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class GuildQuestCatalogValidatorTests
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
        public void Validate_WithCompleteQuestCatalog_ReturnsValidResult()
        {
            var setup = CreateValidSetup();

            var result = GuildQuestCatalogValidator.Validate(setup.QuestCatalog);

            Assert.That(result.IsValid, Is.True, result.FormatErrors());
            Assert.That(result.Issues, Is.Empty);
        }

        [Test]
        public void Validate_WithUnregisteredItemOrTag_ReportsEachError()
        {
            var setup = CreateValidSetup();
            var externalTag = _factory.CreateTag("external", "External");
            var externalItem = _factory.CreateItem(
                "external_item",
                "External Item",
                "An item outside the configured catalog.",
                _factory.CreateIcon(),
                2,
                2,
                ItemRarity.Common,
                ItemCategory.Relic,
                setup.FireTag);
            var exact = _factory.CreateExactItemRequirement("External item", externalItem, 1);
            var tagged = _factory.CreateTagCountRequirement("External tag", externalTag, 1);
            var quest = _factory.CreateQuest(
                "invalid_references",
                "Invalid References",
                "A deliberately invalid test quest.",
                3,
                1,
                exact,
                tagged);
            var catalog = _factory.CreateQuestCatalog(setup.ItemCatalog, quest);

            var result = GuildQuestCatalogValidator.Validate(catalog);

            Assert.That(ContainsIssue(result, "not registered in the item catalog"), Is.True);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Validate_WithDuplicateQuestId_ReportsError()
        {
            var setup = CreateValidSetup();
            var requirement = _factory.CreateTotalPowerRequirement("Power", 1);
            var duplicate = _factory.CreateQuest(
                setup.Quest.Id,
                "Duplicate",
                "A duplicate quest used by the test.",
                2,
                1,
                requirement);
            var catalog = _factory.CreateQuestCatalog(setup.ItemCatalog, setup.Quest, duplicate);

            var result = GuildQuestCatalogValidator.Validate(catalog);

            Assert.That(ContainsIssue(result, "Duplicate quest ID"), Is.True);
        }

        [Test]
        public void Validate_WithInvalidRequirementAndRewards_ReportsErrors()
        {
            var setup = CreateValidSetup();
            var requirement = _factory.CreateExactItemRequirement(string.Empty, null, 0);
            var quest = _factory.CreateQuest(
                "invalid_quest",
                "Invalid Quest",
                "A deliberately invalid test quest.",
                0,
                0,
                requirement);
            var catalog = _factory.CreateQuestCatalog(setup.ItemCatalog, quest);

            var result = GuildQuestCatalogValidator.Validate(catalog);

            Assert.That(ContainsIssue(result, "must have a coin or reputation reward"), Is.True);
            Assert.That(ContainsIssue(result, "has no display name"), Is.True);
            Assert.That(ContainsIssue(result, "has no item assigned"), Is.True);
            Assert.That(ContainsIssue(result, "must require at least one item"), Is.True);
        }

        private ValidQuestSetup CreateValidSetup()
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
            var itemCatalog = _factory.CreateCatalog(
                new[] { fire, weapon },
                new[] { item });
            var exact = _factory.CreateExactItemRequirement("Collect Ember Blade", item, 1);
            var fireCount = _factory.CreateTagCountRequirement("Fire Affinity", fire, 2);
            var power = _factory.CreateTotalPowerRequirement("Total Power", 10);
            var quest = _factory.CreateQuest(
                "flame_arsenal",
                "Flame Arsenal",
                "Build an arsenal worthy of the guild forge.",
                6,
                1,
                exact,
                fireCount,
                power);
            var questCatalog = _factory.CreateQuestCatalog(itemCatalog, quest);
            return new ValidQuestSetup(fire, itemCatalog, quest, questCatalog);
        }

        private static bool ContainsIssue(GuildQuestCatalogValidationResult result, string text)
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

        private sealed class ValidQuestSetup
        {
            public ValidQuestSetup(
                ItemTagDefinition fireTag,
                ItemCatalogAsset itemCatalog,
                GuildQuestDefinition quest,
                GuildQuestCatalogAsset questCatalog)
            {
                FireTag = fireTag;
                ItemCatalog = itemCatalog;
                Quest = quest;
                QuestCatalog = questCatalog;
            }

            public ItemTagDefinition FireTag { get; }

            public ItemCatalogAsset ItemCatalog { get; }

            public GuildQuestDefinition Quest { get; }

            public GuildQuestCatalogAsset QuestCatalog { get; }
        }
    }
}
