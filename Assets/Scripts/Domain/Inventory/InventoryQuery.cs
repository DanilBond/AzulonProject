using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Domain.Inventory
{
    public sealed class InventoryQuery
    {
        private readonly ItemCatalog _catalog;
        private readonly IReadOnlyList<InventoryEntry> _entries;

        public InventoryQuery(PlayerInventory inventory, ItemCatalog catalog)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _entries = inventory.CreateSnapshot();
        }

        public int GetItemQuantity(ItemId itemId)
        {
            EnsureItemExists(itemId);
            foreach (var entry in _entries)
            {
                if (entry.ItemId == itemId)
                {
                    return entry.Quantity;
                }
            }

            return 0;
        }

        public int CountItemsWithTag(ItemTagId tagId)
        {
            if (tagId.IsEmpty)
            {
                throw new ArgumentException("Queried tag ID cannot be empty.", nameof(tagId));
            }

            if (!_catalog.TryGetTag(tagId, out _))
            {
                throw new InvalidOperationException($"Item catalog does not contain tag '{tagId}'.");
            }

            var count = 0;
            foreach (var entry in _entries)
            {
                var item = GetCatalogItem(entry.ItemId);
                if (item.HasTag(tagId))
                {
                    count = checked(count + entry.Quantity);
                }
            }

            return count;
        }

        public int CountUniqueItems()
        {
            foreach (var entry in _entries)
            {
                GetCatalogItem(entry.ItemId);
            }

            return _entries.Count;
        }

        public int CalculateTotalPower()
        {
            var totalPower = 0;
            foreach (var entry in _entries)
            {
                var item = GetCatalogItem(entry.ItemId);
                totalPower = checked(totalPower + checked(item.Power * entry.Quantity));
            }

            return totalPower;
        }

        private void EnsureItemExists(ItemId itemId)
        {
            if (itemId.IsEmpty)
            {
                throw new ArgumentException("Queried item ID cannot be empty.", nameof(itemId));
            }

            GetCatalogItem(itemId);
        }

        private ItemData GetCatalogItem(ItemId itemId)
        {
            if (!_catalog.TryGetItem(itemId, out var item))
            {
                throw new InvalidOperationException($"Item catalog does not contain item '{itemId}'.");
            }

            return item;
        }
    }
}
