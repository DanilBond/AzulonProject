using System;

namespace Azulon.Presentation.Gameplay
{
    public sealed class CollectionItemViewData
    {
        public CollectionItemViewData(
            ItemViewData item,
            int ownedQuantity,
            bool isUnlocked)
        {
            if (ownedQuantity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ownedQuantity));
            }

            Item = item ?? throw new ArgumentNullException(nameof(item));
            OwnedQuantity = ownedQuantity;
            IsUnlocked = isUnlocked;
        }

        public ItemViewData Item { get; }

        public int OwnedQuantity { get; }

        public bool IsOwned => OwnedQuantity > 0;

        public bool IsUnlocked { get; }
    }
}
