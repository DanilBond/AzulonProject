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

        public MarketOffer Generate(
            ItemCatalog catalog,
            ItemRarity maximumUnlockedRarity)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (!Enum.IsDefined(typeof(ItemRarity), maximumUnlockedRarity))
            {
                throw new ArgumentOutOfRangeException(nameof(maximumUnlockedRarity));
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

            var selectedIndex = _randomSource.NextIndex(eligibleItems.Count);
            if (selectedIndex < 0 || selectedIndex >= eligibleItems.Count)
            {
                throw new InvalidOperationException(
                    $"Random source returned index {selectedIndex} for a pool of {eligibleItems.Count} items.");
            }

            return new MarketOffer(_offerIdSource.NextId(), eligibleItems[selectedIndex]);
        }
    }
}
