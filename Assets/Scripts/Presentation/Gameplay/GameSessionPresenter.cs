using System;
using System.Collections.Generic;
using Azulon.Application.Gameplay;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;

namespace Azulon.Presentation.Gameplay
{
    public sealed class GameSessionPresenter
    {
        private readonly GameSession _session;

        public GameSessionPresenter(GameSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public GameScreenViewData CreateViewData()
        {
            var offers = CreateOfferViews();
            var inventoryItems = CreateInventoryViews();
            var quests = CreateQuestViews(out var claimedQuestCount);
            ResolveNextRarity(out var nextRarity, out var nextRequiredReputation);

            return new GameScreenViewData(
                _session.DayNumber,
                _session.VisitorNumber,
                _session.Settings.VisitorsPerDay,
                _session.Coins,
                _session.Reputation,
                _session.MaximumUnlockedRarity,
                nextRarity,
                nextRequiredReputation,
                _session.TotalOwnedItemCount,
                _session.UniqueOwnedItemCount,
                _session.ItemCatalog.Items.Count,
                claimedQuestCount,
                _session.IsCompleted,
                offers,
                inventoryItems,
                quests);
        }

        public GameActionResult PurchaseOffer(MarketOfferId offerId)
        {
            if (_session.IsCompleted)
            {
                return new GameActionResult(
                    GameActionOutcome.SessionCompleted,
                    offerId,
                    sessionCompleted: true);
            }

            if (offerId.IsEmpty ||
                !_session.CurrentOffers.TryGetOffer(offerId, out var offer))
            {
                return new GameActionResult(GameActionOutcome.OfferUnavailable, offerId);
            }

            var purchase = _session.PurchaseOffer(offerId);
            switch (purchase.Status)
            {
                case PurchaseStatus.Success:
                    return new GameActionResult(
                        GameActionOutcome.PurchaseSucceeded,
                        offerId,
                        offer.Item.Id,
                        coinDelta: -offer.Price);

                case PurchaseStatus.InsufficientFunds:
                    return new GameActionResult(
                        GameActionOutcome.InsufficientFunds,
                        offerId,
                        offer.Item.Id);

                case PurchaseStatus.AlreadyPurchased:
                    return new GameActionResult(
                        GameActionOutcome.OfferAlreadyPurchased,
                        offerId,
                        offer.Item.Id);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported purchase status '{purchase.Status}'.");
            }
        }

        public GameActionResult ClaimQuest(QuestId questId)
        {
            if (_session.IsCompleted)
            {
                return new GameActionResult(
                    GameActionOutcome.SessionCompleted,
                    questId: questId,
                    sessionCompleted: true);
            }

            if (questId.IsEmpty || !_session.TryGetQuestState(questId, out _))
            {
                return new GameActionResult(
                    GameActionOutcome.QuestUnavailable,
                    questId: questId);
            }

            var claim = _session.ClaimQuest(questId);
            switch (claim.Status)
            {
                case QuestClaimStatus.Success:
                    return new GameActionResult(
                        GameActionOutcome.QuestClaimed,
                        questId: questId,
                        coinDelta: claim.AwardedCoins,
                        reputationDelta: claim.AwardedReputation,
                        sessionCompleted: _session.IsCompleted);

                case QuestClaimStatus.RequirementsNotMet:
                    return new GameActionResult(
                        GameActionOutcome.QuestRequirementsNotMet,
                        questId: questId);

                case QuestClaimStatus.AlreadyClaimed:
                    return new GameActionResult(
                        GameActionOutcome.QuestAlreadyClaimed,
                        questId: questId);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported quest claim status '{claim.Status}'.");
            }
        }

        public GameActionResult AdvanceToNextVisitor()
        {
            if (_session.IsCompleted)
            {
                return new GameActionResult(
                    GameActionOutcome.SessionCompleted,
                    sessionCompleted: true);
            }

            var advance = _session.AdvanceToNextVisitor();
            return new GameActionResult(
                advance.StartedNewDay
                    ? GameActionOutcome.NewDayStarted
                    : GameActionOutcome.VisitorAdvanced,
                coinDelta: advance.CreditedCoins);
        }

        private IReadOnlyList<MarketOfferViewData> CreateOfferViews()
        {
            var offers = new List<MarketOfferViewData>(_session.CurrentOffers.Offers.Count);
            foreach (var offer in _session.CurrentOffers.Offers)
            {
                offers.Add(new MarketOfferViewData(
                    offer.Id,
                    CreateItemView(offer.Item),
                    offer.IsPurchased,
                    !_session.IsCompleted &&
                    !offer.IsPurchased &&
                    _session.Coins >= offer.Price));
            }

            return offers.AsReadOnly();
        }

        private IReadOnlyList<InventoryItemViewData> CreateInventoryViews()
        {
            var snapshot = _session.CreateInventorySnapshot();
            var inventoryItems = new List<InventoryItemViewData>(snapshot.Count);
            foreach (var entry in snapshot)
            {
                if (!_session.ItemCatalog.TryGetItem(entry.ItemId, out var item))
                {
                    throw new InvalidOperationException(
                        $"Inventory references item '{entry.ItemId}' that is missing from the catalog.");
                }

                inventoryItems.Add(new InventoryItemViewData(
                    CreateItemView(item),
                    entry.Quantity));
            }

            return inventoryItems.AsReadOnly();
        }

        private IReadOnlyList<QuestViewData> CreateQuestViews(out int claimedQuestCount)
        {
            var evaluations = _session.EvaluateQuests();
            var quests = new List<QuestViewData>(_session.QuestStates.Count);
            claimedQuestCount = 0;
            for (var questIndex = 0; questIndex < _session.QuestStates.Count; questIndex++)
            {
                var questState = _session.QuestStates[questIndex];
                var evaluation = evaluations[questIndex];
                if (questState.IsClaimed)
                {
                    claimedQuestCount++;
                }

                var requirements = new List<QuestRequirementViewData>(
                    evaluation.RequirementProgress.Count);
                for (var requirementIndex = 0;
                     requirementIndex < evaluation.RequirementProgress.Count;
                     requirementIndex++)
                {
                    var progress = evaluation.RequirementProgress[requirementIndex];
                    requirements.Add(new QuestRequirementViewData(
                        requirementIndex,
                        progress.Current,
                        progress.Required));
                }

                var quest = questState.Quest;
                quests.Add(new QuestViewData(
                    quest.Id,
                    quest.DisplayName,
                    quest.Description,
                    quest.RewardCoins,
                    quest.RewardReputation,
                    questState.IsClaimed,
                    evaluation.IsCompleted,
                    requirements));
            }

            return quests.AsReadOnly();
        }

        private ItemViewData CreateItemView(ItemData item)
        {
            var tagNames = new List<string>(item.Tags.Count);
            foreach (var tagId in item.Tags)
            {
                if (!_session.ItemCatalog.TryGetTag(tagId, out var tag))
                {
                    throw new InvalidOperationException(
                        $"Item '{item.Id}' references tag '{tagId}' that is missing from the catalog.");
                }

                tagNames.Add(tag.DisplayName);
            }

            return new ItemViewData(
                item.Id,
                item.DisplayName,
                item.Description,
                item.Price,
                item.Power,
                item.Rarity,
                item.Category,
                tagNames);
        }

        private void ResolveNextRarity(
            out ItemRarity? nextRarity,
            out int? nextRequiredReputation)
        {
            nextRarity = null;
            nextRequiredReputation = null;
            foreach (RarityUnlockThreshold threshold in _session.Settings.RarityThresholds)
            {
                if (_session.Reputation < threshold.RequiredReputation)
                {
                    nextRarity = threshold.Rarity;
                    nextRequiredReputation = threshold.RequiredReputation;
                    return;
                }
            }
        }
    }
}
