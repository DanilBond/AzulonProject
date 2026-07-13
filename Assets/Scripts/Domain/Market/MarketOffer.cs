using System;
using Azulon.Domain.Items;

namespace Azulon.Domain.Market
{
    public sealed class MarketOffer
    {
        public MarketOffer(MarketOfferId id, ItemData item)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Market offer ID cannot be empty.", nameof(id));
            }

            Item = item ?? throw new ArgumentNullException(nameof(item));
            Id = id;
        }

        public MarketOfferId Id { get; }

        public ItemData Item { get; }

        public int Price => Item.Price;

        public bool IsPurchased { get; private set; }

        internal void MarkPurchased()
        {
            if (IsPurchased)
            {
                throw new InvalidOperationException($"Market offer '{Id}' has already been purchased.");
            }

            IsPurchased = true;
        }
    }
}
