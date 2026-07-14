using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Presentation.Gameplay
{
    public sealed class ItemViewData
    {
        private readonly IReadOnlyList<string> _tagNames;

        public ItemViewData(
            ItemId id,
            string displayName,
            string description,
            int price,
            int power,
            ItemRarity rarity,
            ItemCategory category,
            IEnumerable<string> tagNames)
        {
            if (tagNames == null)
            {
                throw new ArgumentNullException(nameof(tagNames));
            }

            Id = id;
            DisplayName = displayName;
            Description = description;
            Price = price;
            Power = power;
            Rarity = rarity;
            Category = category;
            _tagNames = new List<string>(tagNames).AsReadOnly();
        }

        public ItemId Id { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int Price { get; }

        public int Power { get; }

        public ItemRarity Rarity { get; }

        public ItemCategory Category { get; }

        public IReadOnlyList<string> TagNames => _tagNames;
    }
}
