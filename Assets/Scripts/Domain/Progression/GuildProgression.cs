using System;
using System.Collections.Generic;
using Azulon.Domain.Items;

namespace Azulon.Domain.Progression
{
    public sealed class GuildProgression
    {
        private readonly IReadOnlyList<RarityUnlockThreshold> _thresholds;

        public GuildProgression(
            IEnumerable<RarityUnlockThreshold> thresholds,
            int initialReputation = 0)
        {
            if (thresholds == null)
            {
                throw new ArgumentNullException(nameof(thresholds));
            }

            if (initialReputation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialReputation));
            }

            var thresholdList = new List<RarityUnlockThreshold>();
            var rarities = new HashSet<ItemRarity>();
            foreach (var threshold in thresholds)
            {
                if (threshold == null)
                {
                    throw new ArgumentException("Rarity thresholds cannot contain null.", nameof(thresholds));
                }

                if (!rarities.Add(threshold.Rarity))
                {
                    throw new ArgumentException(
                        $"Rarity '{threshold.Rarity}' has more than one threshold.",
                        nameof(thresholds));
                }

                thresholdList.Add(threshold);
            }

            thresholdList.Sort((left, right) => left.Rarity.CompareTo(right.Rarity));
            ValidateThresholdOrder(thresholdList);

            _thresholds = thresholdList.AsReadOnly();
            Reputation = initialReputation;
            MaximumUnlockedRarity = ResolveMaximumRarity();
        }

        public int Reputation { get; private set; }

        public ItemRarity MaximumUnlockedRarity { get; private set; }

        public IReadOnlyList<RarityUnlockThreshold> Thresholds => _thresholds;

        public void AddReputation(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Reputation = checked(Reputation + amount);
            MaximumUnlockedRarity = ResolveMaximumRarity();
        }

        private static void ValidateThresholdOrder(IReadOnlyList<RarityUnlockThreshold> thresholds)
        {
            if (thresholds.Count == 0)
            {
                throw new ArgumentException("At least the Common rarity threshold is required.", nameof(thresholds));
            }

            for (var index = 0; index < thresholds.Count; index++)
            {
                var threshold = thresholds[index];
                if ((int)threshold.Rarity != index)
                {
                    throw new ArgumentException(
                        "Rarity thresholds must form a contiguous sequence starting with Common.",
                        nameof(thresholds));
                }

                if (index == 0 && threshold.RequiredReputation != 0)
                {
                    throw new ArgumentException(
                        "Common rarity must unlock at zero reputation.",
                        nameof(thresholds));
                }

                if (index > 0 &&
                    threshold.RequiredReputation <= thresholds[index - 1].RequiredReputation)
                {
                    throw new ArgumentException(
                        "Rarity reputation thresholds must increase strictly.",
                        nameof(thresholds));
                }
            }
        }

        private ItemRarity ResolveMaximumRarity()
        {
            var maximum = ItemRarity.Common;
            foreach (var threshold in _thresholds)
            {
                if (Reputation < threshold.RequiredReputation)
                {
                    break;
                }

                maximum = threshold.Rarity;
            }

            return maximum;
        }
    }
}
