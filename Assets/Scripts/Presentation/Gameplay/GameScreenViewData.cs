using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Presentation.Gameplay
{
    public sealed class GameScreenViewData
    {
        private readonly IReadOnlyList<InventoryItemViewData> _inventoryItems;
        private readonly IReadOnlyList<CollectionItemViewData> _collectionItems;
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
            int claimedQuestCount,
            bool isCompleted,
            MarketOfferViewData offer,
            IEnumerable<InventoryItemViewData> inventoryItems,
            IEnumerable<CollectionItemViewData> collectionItems,
            IEnumerable<QuestViewData> quests)
        {
            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer));
            }

            if (inventoryItems == null)
            {
                throw new ArgumentNullException(nameof(inventoryItems));
            }

            if (quests == null)
            {
                throw new ArgumentNullException(nameof(quests));
            }

            if (collectionItems == null)
            {
                throw new ArgumentNullException(nameof(collectionItems));
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
            ClaimedQuestCount = claimedQuestCount;
            IsCompleted = isCompleted;
            Offer = offer;
            _inventoryItems = new List<InventoryItemViewData>(inventoryItems).AsReadOnly();
            _collectionItems = new List<CollectionItemViewData>(collectionItems).AsReadOnly();
            _quests = new List<QuestViewData>(quests).AsReadOnly();
            AvailableItemCount = _collectionItems.Count;
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

        public MarketOfferViewData Offer { get; }

        public IReadOnlyList<InventoryItemViewData> InventoryItems => _inventoryItems;

        public IReadOnlyList<CollectionItemViewData> CollectionItems => _collectionItems;

        public IReadOnlyList<QuestViewData> Quests => _quests;
    }
}
