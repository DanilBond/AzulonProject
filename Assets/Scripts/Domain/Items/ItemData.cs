using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Azulon.Domain.Items
{
    public sealed class ItemData
    {
        private readonly ReadOnlyCollection<ItemTagId> _tags;

        public ItemData(
            ItemId id,
            string displayName,
            string description,
            int price,
            int power,
            ItemRarity rarity,
            ItemCategory category,
            IEnumerable<ItemTagId> tags)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Item ID cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Item display name cannot be empty.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Item description cannot be empty.", nameof(description));
            }

            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "Item price must be greater than zero.");
            }

            if (power < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(power), "Item power cannot be negative.");
            }

            if (!Enum.IsDefined(typeof(ItemRarity), rarity))
            {
                throw new ArgumentOutOfRangeException(nameof(rarity));
            }

            if (!Enum.IsDefined(typeof(ItemCategory), category))
            {
                throw new ArgumentOutOfRangeException(nameof(category));
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            var tagList = new List<ItemTagId>();
            var uniqueTags = new HashSet<ItemTagId>();
            foreach (var tag in tags)
            {
                if (tag.IsEmpty)
                {
                    throw new ArgumentException("Item tags cannot contain an empty ID.", nameof(tags));
                }

                if (!uniqueTags.Add(tag))
                {
                    throw new ArgumentException($"Item contains duplicate tag '{tag}'.", nameof(tags));
                }

                tagList.Add(tag);
            }

            if (tagList.Count == 0)
            {
                throw new ArgumentException("Item must have at least one tag.", nameof(tags));
            }

            Id = id;
            DisplayName = displayName;
            Description = description;
            Price = price;
            Power = power;
            Rarity = rarity;
            Category = category;
            _tags = tagList.AsReadOnly();
        }

        public ItemId Id { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int Price { get; }

        public int Power { get; }

        public ItemRarity Rarity { get; }

        public ItemCategory Category { get; }

        public IReadOnlyList<ItemTagId> Tags => _tags;

        public bool HasTag(ItemTagId tagId)
        {
            return _tags.Contains(tagId);
        }
    }
}
