using System;
using System.Collections.Generic;
using Azulon.Domain.Items;
using Azulon.Domain.Quests;

namespace Azulon.Presentation.Gameplay
{
    public static class GameFeedbackFormatter
    {
        public static GameFeedback Format(
            GameActionResult result,
            GameScreenViewData viewData)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            switch (result.Outcome)
            {
                case GameActionOutcome.PurchaseSucceeded:
                    return new GameFeedback(
                        $"Purchased {FindItemName(viewData, result.ItemId)} for {Math.Abs(result.CoinDelta)} coins.",
                        GameFeedbackTone.Positive);

                case GameActionOutcome.InsufficientFunds:
                    return new GameFeedback(
                        $"Not enough coins to buy {FindItemName(viewData, result.ItemId)}.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.OfferAlreadyPurchased:
                    return new GameFeedback(
                        $"{FindItemName(viewData, result.ItemId)} has already been purchased.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.OfferUnavailable:
                    return new GameFeedback(
                        "That merchant offer is no longer available.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.QuestClaimed:
                    return FormatQuestClaim(result, viewData);

                case GameActionOutcome.QuestRequirementsNotMet:
                    return new GameFeedback(
                        $"{FindQuestName(viewData, result.QuestId)} requirements are not complete yet.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.QuestAlreadyClaimed:
                    return new GameFeedback(
                        $"{FindQuestName(viewData, result.QuestId)} reward has already been claimed.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.QuestUnavailable:
                    return new GameFeedback(
                        "That guild commission is no longer available.",
                        GameFeedbackTone.Warning);

                case GameActionOutcome.VisitorAdvanced:
                    return new GameFeedback(
                        "A new merchant has arrived.",
                        GameFeedbackTone.Neutral);

                case GameActionOutcome.NewDayStarted:
                    return new GameFeedback(
                        $"Day {viewData.DayNumber} begins. Guild stipend: +{result.CoinDelta} coins.",
                        GameFeedbackTone.Positive);

                case GameActionOutcome.SessionCompleted:
                    return new GameFeedback(
                        "All guild commissions are complete.",
                        GameFeedbackTone.Positive);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported game action outcome '{result.Outcome}'.");
            }
        }

        private static GameFeedback FormatQuestClaim(
            GameActionResult result,
            GameScreenViewData viewData)
        {
            var rewards = new List<string>(2);
            if (result.CoinDelta > 0)
            {
                rewards.Add($"+{result.CoinDelta} coins");
            }

            if (result.ReputationDelta > 0)
            {
                rewards.Add($"+{result.ReputationDelta} reputation");
            }

            var rewardText = rewards.Count > 0
                ? $" {string.Join(", ", rewards)}."
                : string.Empty;
            return new GameFeedback(
                $"Commission complete: {FindQuestName(viewData, result.QuestId)}.{rewardText}",
                GameFeedbackTone.Positive);
        }

        private static string FindItemName(GameScreenViewData viewData, ItemId itemId)
        {
            foreach (var offer in viewData.Offers)
            {
                if (offer.Item.Id == itemId)
                {
                    return offer.Item.DisplayName;
                }
            }

            foreach (var inventoryItem in viewData.InventoryItems)
            {
                if (inventoryItem.Item.Id == itemId)
                {
                    return inventoryItem.Item.DisplayName;
                }
            }

            return "item";
        }

        private static string FindQuestName(GameScreenViewData viewData, QuestId questId)
        {
            foreach (var quest in viewData.Quests)
            {
                if (quest.Id == questId)
                {
                    return quest.DisplayName;
                }
            }

            return "Commission";
        }
    }
}
