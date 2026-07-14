using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;

namespace Azulon.Tests.EditMode.Domain.Quests
{
    internal sealed class QuestTestContext
    {
        public QuestTestContext()
        {
            FireTagId = new ItemTagId("fire");
            WeaponTagId = new ItemTagId("weapon");
            WaterTagId = new ItemTagId("water");

            EmberBlade = CreateItem(
                "ember_blade",
                7,
                ItemCategory.Weapon,
                FireTagId,
                WeaponTagId);
            FlameOrb = CreateItem(
                "flame_orb",
                5,
                ItemCategory.Relic,
                FireTagId);
            TideCharm = CreateItem(
                "tide_charm",
                4,
                ItemCategory.Accessory,
                WaterTagId);

            Catalog = new ItemCatalog(
                new[]
                {
                    new ItemTagData(FireTagId, "Fire"),
                    new ItemTagData(WeaponTagId, "Weapon"),
                    new ItemTagData(WaterTagId, "Water")
                },
                new[] { EmberBlade, FlameOrb, TideCharm });
        }

        public ItemTagId FireTagId { get; }

        public ItemTagId WeaponTagId { get; }

        public ItemTagId WaterTagId { get; }

        public ItemData EmberBlade { get; }

        public ItemData FlameOrb { get; }

        public ItemData TideCharm { get; }

        public ItemCatalog Catalog { get; }

        public GuildQuest CreateQuest(params IQuestRequirement[] requirements)
        {
            return CreateQuest(6, 1, requirements);
        }

        public GuildQuest CreateQuest(
            int rewardCoins,
            int rewardReputation,
            params IQuestRequirement[] requirements)
        {
            return new GuildQuest(
                new QuestId("flame_arsenal"),
                "Flame Arsenal",
                "Build an arsenal worthy of the guild forge.",
                rewardCoins,
                rewardReputation,
                requirements);
        }

        public GuildProgression CreateProgression(int initialReputation = 0)
        {
            return new GuildProgression(
                new[]
                {
                    new RarityUnlockThreshold(ItemRarity.Common, 0),
                    new RarityUnlockThreshold(ItemRarity.Uncommon, 1),
                    new RarityUnlockThreshold(ItemRarity.Rare, 3),
                    new RarityUnlockThreshold(ItemRarity.Epic, 6),
                    new RarityUnlockThreshold(ItemRarity.Legendary, 10)
                },
                initialReputation);
        }

        private static ItemData CreateItem(
            string id,
            int power,
            ItemCategory category,
            params ItemTagId[] tags)
        {
            return new ItemData(
                new ItemId(id),
                id,
                "An item used by the quest domain tests.",
                3,
                power,
                ItemRarity.Common,
                category,
                tags);
        }
    }
}
