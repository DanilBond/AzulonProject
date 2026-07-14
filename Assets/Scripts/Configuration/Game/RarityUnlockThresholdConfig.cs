using System;
using Azulon.Domain.Items;
using UnityEngine;

namespace Azulon.Configuration.Game
{
    [Serializable]
    public sealed class RarityUnlockThresholdConfig
    {
        [SerializeField] private ItemRarity _rarity;
        [SerializeField, Min(0)] private int _requiredReputation;

        public RarityUnlockThresholdConfig()
        {
        }

        public RarityUnlockThresholdConfig(ItemRarity rarity, int requiredReputation)
        {
            _rarity = rarity;
            _requiredReputation = requiredReputation;
        }

        public ItemRarity Rarity => _rarity;

        public int RequiredReputation => _requiredReputation;
    }
}
