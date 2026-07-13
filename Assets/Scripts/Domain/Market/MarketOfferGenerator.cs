using System;
using System.Collections.Generic;
using Azulon.Domain.Items;
using Azulon.Domain.Randomness;

namespace Azulon.Domain.Market
{
    public sealed class MarketOfferGenerator
    {
        private readonly IRandomSource _randomSource;
        private readonly IMarketOfferIdSource _offerIdSource;

        public MarketOfferGenerator(
            IRandomSource randomSource,
            IMarketOfferIdSource offerIdSource)
        {
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            _offerIdSource = offerIdSource ?? throw new ArgumentNullException(nameof(offerIdSource));
        }

        public MarketOfferSet Generate(
            ItemCatalog catalog,
            ItemRarity maximumUnlockedRarity,
            int requestedOfferCount)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (!Enum.IsDefined(typeof(ItemRarity), maximumUnlockedRarity))
            {
                throw new ArgumentOutOfRangeException(nameof(maximumUnlockedRarity));
            }

            if (requestedOfferCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requestedOfferCount),
                    "Requested offer count must be greater than zero.");
            }

            var eligibleItems = new List<ItemData>();
            foreach (var item in catalog.Items)
            {
                if ((int)item.Rarity <= (int)maximumUnlockedRarity)
                {
                    eligibleItems.Add(item);
                }
            }

            if (eligibleItems.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Catalog has no items available at rarity '{maximumUnlockedRarity}' or below.");
            }

            var generatedCount = Math.Min(requestedOfferCount, eligibleItems.Count);
            var offers = new List<MarketOffer>(generatedCount);
            for (var index = 0; index < generatedCount; index++)
            {
                var selectedIndex = _randomSource.NextIndex(eligibleItems.Count);
                if (selectedIndex < 0 || selectedIndex >= eligibleItems.Count)
                {
                    throw new InvalidOperationException(
                        $"Random source returned index {selectedIndex} for a pool of {eligibleItems.Count} items.");
                }

                var selectedItem = eligibleItems[selectedIndex];
                eligibleItems.RemoveAt(selectedIndex);
                offers.Add(new MarketOffer(_offerIdSource.NextId(), selectedItem));
            }

            return new MarketOfferSet(offers);
        }
    }
}
