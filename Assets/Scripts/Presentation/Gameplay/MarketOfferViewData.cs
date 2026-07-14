using System;
using Azulon.Domain.Market;

namespace Azulon.Presentation.Gameplay
{
    public sealed class MarketOfferViewData
    {
        public MarketOfferViewData(
            MarketOfferId offerId,
            ItemViewData item,
            bool isPurchased,
            bool canPurchase)
        {
            OfferId = offerId;
            Item = item ?? throw new ArgumentNullException(nameof(item));
            IsPurchased = isPurchased;
            CanPurchase = canPurchase;
        }

        public MarketOfferId OfferId { get; }

        public ItemViewData Item { get; }

        public bool IsPurchased { get; }

        public bool CanPurchase { get; }
    }
}
