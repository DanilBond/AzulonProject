using Azulon.Application.Gameplay;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;
using Azulon.Domain.Randomness;

namespace Azulon.Tests.EditMode.Application.Gameplay
{
    internal sealed class GameSessionTestContext
    {
        public GameSessionTestContext()
        {
            FireTagId = new ItemTagId("fire");
            WeaponTagId = new ItemTagId("weapon");

            EmberBlade = CreateItem(
                "ember_blade",
                ItemRarity.Common,
                4,
                7,
                FireTagId,
                WeaponTagId);
            FlameOrb = CreateItem(
                "flame_orb",
                ItemRarity.Uncommon,
                4,
                7,
                FireTagId);
            PhoenixSeal = CreateItem(
                "phoenix_seal",
                ItemRarity.Rare,
                7,
                10,
                FireTagId);

            ItemCatalog = new ItemCatalog(
                new[]
                {
                    new ItemTagData(FireTagId, "Fire"),
                    new ItemTagData(WeaponTagId, "Weapon")
                },
                new[] { EmberBlade, FlameOrb, PhoenixSeal });

            EmberCommission = new GuildQuest(
                new QuestId("ember_commission"),
                "Ember Commission",
                "Acquire an Ember Blade for the guild armory.",
                4,
                1,
                new IQuestRequirement[]
                {
                    new ExactItemRequirement(EmberBlade.Id, 1)
                });
            FlameArsenal = new GuildQuest(
                new QuestId("flame_arsenal"),
                "Flame Arsenal",
                "Assemble a powerful collection of fire weapons.",
                8,
                2,
                new IQuestRequirement[]
                {
                    new TagCountRequirement(FireTagId, 2),
                    new TagCountRequirement(WeaponTagId, 1),
                    new TotalPowerRequirement(14)
                });

            QuestCatalog = new GuildQuestCatalog(new[] { EmberCommission, FlameArsenal });
            Settings = new GameSessionSettings(
                4,
                4,
                2,
                3,
                new[]
                {
                    new RarityUnlockThreshold(ItemRarity.Common, 0),
                    new RarityUnlockThreshold(ItemRarity.Uncommon, 1),
                    new RarityUnlockThreshold(ItemRarity.Rare, 3)
                });
        }

        public ItemTagId FireTagId { get; }

        public ItemTagId WeaponTagId { get; }

        public ItemData EmberBlade { get; }

        public ItemData FlameOrb { get; }

        public ItemData PhoenixSeal { get; }

        public ItemCatalog ItemCatalog { get; }

        public GuildQuest EmberCommission { get; }

        public GuildQuest FlameArsenal { get; }

        public GuildQuestCatalog QuestCatalog { get; }

        public GameSessionSettings Settings { get; }

        public GameSession CreateSession()
        {
            return new GameSession(
                Settings,
                ItemCatalog,
                QuestCatalog,
                new FirstItemRandomSource(),
                new SequentialMarketOfferIdSource());
        }

        public static MarketOffer FindOffer(GameSession session, ItemId itemId)
        {
            foreach (var offer in session.CurrentOffers.Offers)
            {
                if (offer.Item.Id == itemId)
                {
                    return offer;
                }
            }

            return null;
        }

        private static ItemData CreateItem(
            string id,
            ItemRarity rarity,
            int price,
            int power,
            params ItemTagId[] tags)
        {
            return new ItemData(
                new ItemId(id),
                id,
                "An item used by the game session tests.",
                price,
                power,
                rarity,
                ItemCategory.Relic,
                tags);
        }

        private sealed class FirstItemRandomSource : IRandomSource
        {
            public int NextIndex(int exclusiveUpperBound)
            {
                return 0;
            }
        }
    }
}
