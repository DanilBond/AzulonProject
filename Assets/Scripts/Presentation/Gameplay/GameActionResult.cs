using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Quests;

namespace Azulon.Presentation.Gameplay
{
    public sealed class GameActionResult
    {
        internal GameActionResult(
            GameActionOutcome outcome,
            MarketOfferId offerId = default,
            ItemId itemId = default,
            QuestId questId = default,
            int coinDelta = 0,
            int reputationDelta = 0,
            bool sessionCompleted = false)
        {
            Outcome = outcome;
            OfferId = offerId;
            ItemId = itemId;
            QuestId = questId;
            CoinDelta = coinDelta;
            ReputationDelta = reputationDelta;
            SessionCompleted = sessionCompleted;
        }

        public GameActionOutcome Outcome { get; }

        public MarketOfferId OfferId { get; }

        public ItemId ItemId { get; }

        public QuestId QuestId { get; }

        public int CoinDelta { get; }

        public int ReputationDelta { get; }

        public bool SessionCompleted { get; }
    }
}
