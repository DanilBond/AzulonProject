using System;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;

namespace Azulon.Domain.Quests
{
    public sealed class QuestClaimService
    {
        private readonly QuestEvaluator _evaluator;

        public QuestClaimService(QuestEvaluator evaluator)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        public QuestClaimResult Claim(
            GuildQuestState questState,
            PlayerInventory inventory,
            ItemCatalog catalog,
            Wallet wallet,
            GuildProgression progression)
        {
            if (questState == null)
            {
                throw new ArgumentNullException(nameof(questState));
            }

            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            if (progression == null)
            {
                throw new ArgumentNullException(nameof(progression));
            }

            var evaluation = _evaluator.Evaluate(questState.Quest, inventory, catalog);
            if (questState.IsClaimed)
            {
                return CreateResult(QuestClaimStatus.AlreadyClaimed, evaluation, wallet, progression);
            }

            if (!evaluation.IsCompleted)
            {
                return CreateResult(QuestClaimStatus.RequirementsNotMet, evaluation, wallet, progression);
            }

            EnsureRewardDoesNotOverflow(questState.Quest, wallet, progression);
            if (questState.Quest.RewardCoins > 0)
            {
                wallet.Credit(questState.Quest.RewardCoins);
            }

            if (questState.Quest.RewardReputation > 0)
            {
                progression.AddReputation(questState.Quest.RewardReputation);
            }

            questState.MarkClaimed();
            return CreateResult(QuestClaimStatus.Success, evaluation, wallet, progression);
        }

        private static void EnsureRewardDoesNotOverflow(
            GuildQuest quest,
            Wallet wallet,
            GuildProgression progression)
        {
            if ((long)wallet.Balance + quest.RewardCoins > int.MaxValue ||
                (long)progression.Reputation + quest.RewardReputation > int.MaxValue)
            {
                throw new OverflowException("Quest reward exceeds the supported progression range.");
            }
        }

        private static QuestClaimResult CreateResult(
            QuestClaimStatus status,
            QuestEvaluation evaluation,
            Wallet wallet,
            GuildProgression progression)
        {
            var succeeded = status == QuestClaimStatus.Success;
            return new QuestClaimResult(
                status,
                evaluation,
                succeeded ? evaluation.Quest.RewardCoins : 0,
                succeeded ? evaluation.Quest.RewardReputation : 0,
                wallet.Balance,
                progression.Reputation,
                progression.MaximumUnlockedRarity);
        }
    }
}
