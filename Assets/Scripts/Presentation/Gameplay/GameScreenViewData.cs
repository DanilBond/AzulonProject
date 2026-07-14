using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Presentation.Gameplay
{
    public sealed class GameScreenViewData
    {
        private readonly IReadOnlyList<MarketOfferViewData> _offers;
        private readonly IReadOnlyList<InventoryItemViewData> _inventoryItems;
        private readonly IReadOnlyList<QuestViewData> _quests;

        public GameScreenViewData(
            int dayNumber,
            int visitorNumber,
            int visitorsPerDay,
            int coins,
            int reputation,
            ItemRarity maximumUnlockedRarity,
            ItemRarity? nextRarity,
            int? nextRarityRequiredReputation,
            int totalOwnedItemCount,
            int uniqueOwnedItemCount,
            int availableItemCount,
            int claimedQuestCount,
            bool isCompleted,
            IEnumerable<MarketOfferViewData> offers,
            IEnumerable<InventoryItemViewData> inventoryItems,
            IEnumerable<QuestViewData> quests)
        {
            if (offers == null)
            {
                throw new ArgumentNullException(nameof(offers));
            }

            if (inventoryItems == null)
            {
                throw new ArgumentNullException(nameof(inventoryItems));
            }

            if (quests == null)
            {
                throw new ArgumentNullException(nameof(quests));
            }

            DayNumber = dayNumber;
            VisitorNumber = visitorNumber;
            VisitorsPerDay = visitorsPerDay;
            Coins = coins;
            Reputation = reputation;
            MaximumUnlockedRarity = maximumUnlockedRarity;
            NextRarity = nextRarity;
            NextRarityRequiredReputation = nextRarityRequiredReputation;
            TotalOwnedItemCount = totalOwnedItemCount;
            UniqueOwnedItemCount = uniqueOwnedItemCount;
            AvailableItemCount = availableItemCount;
            ClaimedQuestCount = claimedQuestCount;
            IsCompleted = isCompleted;
            _offers = new List<MarketOfferViewData>(offers).AsReadOnly();
            _inventoryItems = new List<InventoryItemViewData>(inventoryItems).AsReadOnly();
            _quests = new List<QuestViewData>(quests).AsReadOnly();
        }

        public int DayNumber { get; }

        public int VisitorNumber { get; }

        public int VisitorsPerDay { get; }

        public int Coins { get; }

        public int Reputation { get; }

        public ItemRarity MaximumUnlockedRarity { get; }

        public ItemRarity? NextRarity { get; }

        public int? NextRarityRequiredReputation { get; }

        public int TotalOwnedItemCount { get; }

        public int UniqueOwnedItemCount { get; }

        public int AvailableItemCount { get; }

        public int ClaimedQuestCount { get; }

        public int QuestCount => _quests.Count;

        public bool IsCompleted { get; }

        public bool CanAdvanceVisitor => !IsCompleted;

        public IReadOnlyList<MarketOfferViewData> Offers => _offers;

        public IReadOnlyList<InventoryItemViewData> InventoryItems => _inventoryItems;

        public IReadOnlyList<QuestViewData> Quests => _quests;
    }
}
