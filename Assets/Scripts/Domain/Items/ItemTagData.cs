using System;

namespace Azulon.Domain.Items
{
    public sealed class ItemTagData
    {
        public ItemTagData(ItemTagId id, string displayName)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Item tag ID cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Item tag display name cannot be empty.", nameof(displayName));
            }

            Id = id;
            DisplayName = displayName;
        }

        public ItemTagId Id { get; }

        public string DisplayName { get; }
    }
}
