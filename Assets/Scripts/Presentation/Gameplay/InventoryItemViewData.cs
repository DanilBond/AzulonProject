using System;

namespace Azulon.Presentation.Gameplay
{
    public sealed class InventoryItemViewData
    {
        public InventoryItemViewData(ItemViewData item, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            Item = item ?? throw new ArgumentNullException(nameof(item));
            Quantity = quantity;
        }

        public ItemViewData Item { get; }

        public int Quantity { get; }
    }
}
