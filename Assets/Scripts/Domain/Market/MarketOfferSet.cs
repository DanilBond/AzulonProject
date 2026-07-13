using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Domain.Market
{
    public sealed class MarketOfferSet
    {
        private readonly IReadOnlyList<MarketOffer> _offers;
        private readonly Dictionary<MarketOfferId, MarketOffer> _offersById;

        public MarketOfferSet(IEnumerable<MarketOffer> offers)
        {
            if (offers == null)
            {
                throw new ArgumentNullException(nameof(offers));
            }

            var offerList = new List<MarketOffer>();
            var itemIds = new HashSet<ItemId>();
            _offersById = new Dictionary<MarketOfferId, MarketOffer>();

            foreach (var offer in offers)
            {
                if (offer == null)
                {
                    throw new ArgumentException("Market offer set cannot contain null.", nameof(offers));
                }

                if (_offersById.ContainsKey(offer.Id))
                {
                    throw new ArgumentException(
                        $"Market offer set contains duplicate offer ID '{offer.Id}'.",
                        nameof(offers));
                }

                _offersById.Add(offer.Id, offer);
                if (!itemIds.Add(offer.Item.Id))
                {
                    throw new ArgumentException(
                        $"Market offer set contains item '{offer.Item.Id}' more than once.",
                        nameof(offers));
                }

                offerList.Add(offer);
            }

            if (offerList.Count == 0)
            {
                throw new ArgumentException("Market offer set must contain at least one offer.", nameof(offers));
            }

            _offers = offerList.AsReadOnly();
        }

        public IReadOnlyList<MarketOffer> Offers => _offers;

        public int AvailableOfferCount
        {
            get
            {
                var count = 0;
                foreach (var offer in _offers)
                {
                    if (!offer.IsPurchased)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsFullyPurchased => AvailableOfferCount == 0;

        public bool TryGetOffer(MarketOfferId offerId, out MarketOffer offer)
        {
            return _offersById.TryGetValue(offerId, out offer);
        }
    }
}
