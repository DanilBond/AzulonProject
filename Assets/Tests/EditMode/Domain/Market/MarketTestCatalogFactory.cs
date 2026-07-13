using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Tests.EditMode.Domain.Market
{
    internal static class MarketTestCatalogFactory
    {
        public static ItemCatalog CreateCatalog(params ItemRarity[] rarities)
        {
            if (rarities == null)
            {
                throw new ArgumentNullException(nameof(rarities));
            }

            var tag = new ItemTagData(new ItemTagId("market_test"), "Market Test");
            var items = new List<ItemData>(rarities.Length);
            for (var index = 0; index < rarities.Length; index++)
            {
                items.Add(CreateItem($"item_{index}", rarities[index]));
            }

            return new ItemCatalog(new[] { tag }, items);
        }

        public static ItemData CreateItem(string id, ItemRarity rarity = ItemRarity.Common)
        {
            return new ItemData(
                new ItemId(id),
                $"Test Item {id}",
                "An item used by the market generation tests.",
                3,
                5,
                rarity,
                ItemCategory.Relic,
                new[] { new ItemTagId("market_test") });
        }
    }
}
