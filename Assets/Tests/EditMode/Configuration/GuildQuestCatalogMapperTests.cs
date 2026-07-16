using System;
using Azulon.Configuration.Quests;
using Azulon.Domain.Items;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class GuildQuestCatalogMapperTests
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
        public void ToDomain_WithValidConfiguration_MapsEveryRequirementStrategy()
        {
            var fire = _factory.CreateTag("fire", "Fire");
            var item = _factory.CreateItem(
                "ember_blade",
                "Ember Blade",
                "A blade holding a steady magical flame.",
                _factory.CreateIcon(),
                4,
                7,
                ItemRarity.Uncommon,
                ItemCategory.Weapon,
                fire);
            var itemCatalog = _factory.CreateCatalog(new[] { fire }, new[] { item });
            var exact = _factory.CreateExactItemRequirement("Exact", item, 1);
            var tagged = _factory.CreateTagCountRequirement("Tagged", fire, 2);
            var power = _factory.CreateTotalPowerRequirement("Power", 10);
            var uniqueItems = _factory.CreateUniqueItemCountRequirement("Collection", 1);
            var questDefinition = _factory.CreateQuest(
                "flame_arsenal",
                "Flame Arsenal",
                "Build an arsenal worthy of the guild forge.",
                6,
                1,
                exact,
                tagged,
                power,
                uniqueItems);
            var catalogAsset = _factory.CreateQuestCatalog(itemCatalog, questDefinition);

            var catalog = GuildQuestCatalogMapper.ToDomain(catalogAsset);

            Assert.That(catalog.TryGetQuest(new QuestId("flame_arsenal"), out var quest), Is.True);
            Assert.That(quest.Requirements[0], Is.TypeOf<ExactItemRequirement>());
            Assert.That(quest.Requirements[1], Is.TypeOf<TagCountRequirement>());
            Assert.That(quest.Requirements[2], Is.TypeOf<TotalPowerRequirement>());
            Assert.That(quest.Requirements[3], Is.TypeOf<UniqueItemCountRequirement>());
            Assert.That(quest.RewardCoins, Is.EqualTo(6));
            Assert.That(quest.RewardReputation, Is.EqualTo(1));
        }

        [Test]
        public void ToDomain_WithInvalidConfiguration_ThrowsDescriptiveException()
        {
            var emptyCatalog = _factory.CreateQuestCatalog(null);

            Assert.That(
                () => GuildQuestCatalogMapper.ToDomain(emptyCatalog),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("has no item catalog assigned"));
        }
    }
}
