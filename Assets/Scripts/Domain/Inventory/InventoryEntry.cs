using System;
using Azulon.Domain.Items;

namespace Azulon.Domain.Inventory
{
    public readonly struct InventoryEntry
    {
        public InventoryEntry(ItemId itemId, int quantity)
        {
            if (itemId.IsEmpty)
            {
                throw new ArgumentException("Inventory item ID cannot be empty.", nameof(itemId));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Inventory quantity must be greater than zero.");
            }

            ItemId = itemId;
            Quantity = quantity;
        }

        public ItemId ItemId { get; }

        public int Quantity { get; }
    }
}
