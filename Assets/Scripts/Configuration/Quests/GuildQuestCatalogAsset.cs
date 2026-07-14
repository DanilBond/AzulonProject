using System.Collections.Generic;
using Azulon.Configuration.Items;
using UnityEngine;

namespace Azulon.Configuration.Quests
{
    [CreateAssetMenu(
        fileName = "GuildQuestCatalog",
        menuName = "Guild Relic Market/Quests/Quest Catalog",
        order = 150)]
    public sealed class GuildQuestCatalogAsset : ScriptableObject
    {
        [SerializeField] private ItemCatalogAsset _itemCatalog;
        [SerializeField] private List<GuildQuestDefinition> _questDefinitions =
            new List<GuildQuestDefinition>();

        public ItemCatalogAsset ItemCatalog => _itemCatalog;

        public IReadOnlyList<GuildQuestDefinition> QuestDefinitions => _questDefinitions;
    }
}
