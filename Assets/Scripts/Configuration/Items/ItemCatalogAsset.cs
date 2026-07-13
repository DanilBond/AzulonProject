using System.Collections.Generic;
using UnityEngine;

namespace Azulon.Configuration.Items
{
    [CreateAssetMenu(
        fileName = "ItemCatalog",
        menuName = "Guild Relic Market/Items/Item Catalog",
        order = 30)]
    public sealed class ItemCatalogAsset : ScriptableObject
    {
        [SerializeField] private List<ItemTagDefinition> _tagDefinitions = new List<ItemTagDefinition>();
        [SerializeField] private List<ItemDefinition> _itemDefinitions = new List<ItemDefinition>();

        public IReadOnlyList<ItemTagDefinition> TagDefinitions => _tagDefinitions;

        public IReadOnlyList<ItemDefinition> ItemDefinitions => _itemDefinitions;
    }
}
