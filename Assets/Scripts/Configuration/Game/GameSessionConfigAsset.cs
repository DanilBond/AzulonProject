using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Domain.Items;
using UnityEngine;

namespace Azulon.Configuration.Game
{
    [CreateAssetMenu(
        fileName = "GameSessionConfig",
        menuName = "Guild Relic Market/Game/Session Configuration",
        order = 200)]
    public sealed class GameSessionConfigAsset : ScriptableObject
    {
        [Header("Content")]
        [SerializeField] private ItemCatalogAsset _itemCatalog;
        [SerializeField] private GuildQuestCatalogAsset _questCatalog;

        [Header("Economy")]
        [SerializeField, Min(0)] private int _startingCoins = 8;
        [SerializeField, Min(1)] private int _dailyCoinStipend = 4;

        [Header("Market Loop")]
        [SerializeField, Min(1)] private int _visitorsPerDay = 3;
        [SerializeField, Min(1)] private int _offersPerVisitor = 3;

        [Header("Reputation Progression")]
        [SerializeField] private List<RarityUnlockThresholdConfig> _rarityThresholds =
            new List<RarityUnlockThresholdConfig>
            {
                new RarityUnlockThresholdConfig(ItemRarity.Common, 0),
                new RarityUnlockThresholdConfig(ItemRarity.Uncommon, 1),
                new RarityUnlockThresholdConfig(ItemRarity.Rare, 3),
                new RarityUnlockThresholdConfig(ItemRarity.Epic, 6),
                new RarityUnlockThresholdConfig(ItemRarity.Legendary, 10)
            };

        public ItemCatalogAsset ItemCatalog => _itemCatalog;

        public GuildQuestCatalogAsset QuestCatalog => _questCatalog;

        public int StartingCoins => _startingCoins;

        public int DailyCoinStipend => _dailyCoinStipend;

        public int VisitorsPerDay => _visitorsPerDay;

        public int OffersPerVisitor => _offersPerVisitor;

        public IReadOnlyList<RarityUnlockThresholdConfig> RarityThresholds => _rarityThresholds;
    }
}
