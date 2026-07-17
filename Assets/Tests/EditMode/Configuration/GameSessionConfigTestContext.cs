using System.Collections.Generic;
using Azulon.Configuration.Game;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using UnityEngine;

namespace Azulon.Tests.EditMode.Configuration
{
    internal sealed class GameSessionConfigTestContext
    {
        private readonly ScriptableObjectTestFactory _factory;

        public GameSessionConfigTestContext(
            ScriptableObjectTestFactory factory,
            ItemRarity itemRarity = ItemRarity.Common,
            string idSuffix = "main")
        {
            _factory = factory;
            var fire = factory.CreateTag($"fire_{idSuffix}", "Fire");
            Item = factory.CreateItem(
                $"ember_blade_{idSuffix}",
                "Ember Blade",
                "A blade holding a steady magical flame.",
                factory.CreateIcon(),
                4,
                7,
                itemRarity,
                ItemCategory.Weapon,
                fire);
            ItemCatalog = factory.CreateCatalog(new[] { fire }, new[] { Item });
            var requirement = factory.CreateExactItemRequirement("Ember Blade", Item, 1);
            Quest = factory.CreateQuest(
                $"ember_commission_{idSuffix}",
                "Ember Commission",
                "Acquire an Ember Blade for the guild armory.",
                4,
                1,
                requirement);
            QuestCatalog = factory.CreateQuestCatalog(ItemCatalog, Quest);
            VisitorSprites = new[] { factory.CreateIcon(), factory.CreateIcon() };
        }

        public ItemDefinition Item { get; }

        public ItemCatalogAsset ItemCatalog { get; }

        public GuildQuestDefinition Quest { get; }

        public GuildQuestCatalogAsset QuestCatalog { get; }

        public IReadOnlyList<Sprite> VisitorSprites { get; }

        public GameSessionConfigAsset CreateConfig(params RarityUnlockThreshold[] thresholds)
        {
            return _factory.CreateGameSessionConfig(
                ItemCatalog,
                QuestCatalog,
                4,
                3,
                2,
                VisitorSprites,
                thresholds);
        }
    }
}
