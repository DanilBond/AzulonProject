using Azulon.Domain.Items;

namespace Azulon.Domain.Quests
{
    public sealed class QuestClaimResult
    {
        internal QuestClaimResult(
            QuestClaimStatus status,
            QuestEvaluation evaluation,
            int awardedCoins,
            int awardedReputation,
            int walletBalance,
            int totalReputation,
            ItemRarity maximumUnlockedRarity)
        {
            Status = status;
            Evaluation = evaluation;
            AwardedCoins = awardedCoins;
            AwardedReputation = awardedReputation;
            WalletBalance = walletBalance;
            TotalReputation = totalReputation;
            MaximumUnlockedRarity = maximumUnlockedRarity;
        }

        public QuestClaimStatus Status { get; }

        public bool Succeeded => Status == QuestClaimStatus.Success;

        public QuestEvaluation Evaluation { get; }

        public QuestId QuestId => Evaluation.Quest.Id;

        public int AwardedCoins { get; }

        public int AwardedReputation { get; }

        public int WalletBalance { get; }

        public int TotalReputation { get; }

        public ItemRarity MaximumUnlockedRarity { get; }
    }
}
