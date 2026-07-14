using System;
using Azulon.Domain.Items;

namespace Azulon.Domain.Progression
{
    public sealed class RarityUnlockThreshold
    {
        public RarityUnlockThreshold(ItemRarity rarity, int requiredReputation)
        {
            if (!Enum.IsDefined(typeof(ItemRarity), rarity))
            {
                throw new ArgumentOutOfRangeException(nameof(rarity));
            }

            if (requiredReputation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredReputation));
            }

            Rarity = rarity;
            RequiredReputation = requiredReputation;
        }

        public ItemRarity Rarity { get; }

        public int RequiredReputation { get; }
    }
}
