using Azulon.Domain.Items;

namespace Azulon.Domain.Market
{
    public sealed class PurchaseResult
    {
        internal PurchaseResult(
            PurchaseStatus status,
            MarketOfferId offerId,
            ItemId itemId,
            int remainingCoins,
            int ownedQuantity)
        {
            Status = status;
            OfferId = offerId;
            ItemId = itemId;
            RemainingCoins = remainingCoins;
            OwnedQuantity = ownedQuantity;
        }

        public PurchaseStatus Status { get; }

        public bool Succeeded => Status == PurchaseStatus.Success;

        public MarketOfferId OfferId { get; }

        public ItemId ItemId { get; }

        public int RemainingCoins { get; }

        public int OwnedQuantity { get; }
    }
}
