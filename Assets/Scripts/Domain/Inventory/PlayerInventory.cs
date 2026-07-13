using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Domain.Inventory
{
    public sealed class PlayerInventory
    {
        private readonly List<ItemId> _acquisitionOrder = new List<ItemId>();
        private readonly Dictionary<ItemId, int> _quantities = new Dictionary<ItemId, int>();

        public int TotalItemCount { get; private set; }

        public int UniqueItemCount => _acquisitionOrder.Count;

        public void Add(ItemId itemId, int quantity = 1)
        {
            EnsureValidItemId(itemId);
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Added quantity must be greater than zero.");
            }

            _quantities.TryGetValue(itemId, out var currentQuantity);
            var updatedQuantity = checked(currentQuantity + quantity);
            var updatedTotal = checked(TotalItemCount + quantity);

            if (currentQuantity == 0)
            {
                _acquisitionOrder.Add(itemId);
            }

            _quantities[itemId] = updatedQuantity;
            TotalItemCount = updatedTotal;
        }

        public bool Contains(ItemId itemId)
        {
            EnsureValidItemId(itemId);
            return _quantities.ContainsKey(itemId);
        }

        public int GetQuantity(ItemId itemId)
        {
            EnsureValidItemId(itemId);
            return _quantities.TryGetValue(itemId, out var quantity) ? quantity : 0;
        }

        public IReadOnlyList<InventoryEntry> CreateSnapshot()
        {
            var entries = new List<InventoryEntry>(_acquisitionOrder.Count);
            foreach (var itemId in _acquisitionOrder)
            {
                entries.Add(new InventoryEntry(itemId, _quantities[itemId]));
            }

            return entries.AsReadOnly();
        }

        private static void EnsureValidItemId(ItemId itemId)
        {
            if (itemId.IsEmpty)
            {
                throw new ArgumentException("Inventory item ID cannot be empty.", nameof(itemId));
            }
        }
    }
}
