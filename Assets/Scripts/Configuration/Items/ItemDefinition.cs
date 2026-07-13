using System.Collections.Generic;
using Azulon.Domain.Items;
using UnityEngine;

namespace Azulon.Configuration.Items
{
    [CreateAssetMenu(
        fileName = "Item_New",
        menuName = "Guild Relic Market/Items/Item Definition",
        order = 20)]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 4)] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField, Min(1)] private int _price = 1;
        [SerializeField, Min(0)] private int _power;
        [SerializeField] private ItemRarity _rarity;
        [SerializeField] private ItemCategory _category = ItemCategory.Relic;
        [SerializeField] private List<ItemTagDefinition> _tags = new List<ItemTagDefinition>();

        public string Id => _id;

        public string DisplayName => _displayName;

        public string Description => _description;

        public Sprite Icon => _icon;

        public int Price => _price;

        public int Power => _power;

        public ItemRarity Rarity => _rarity;

        public ItemCategory Category => _category;

        public IReadOnlyList<ItemTagDefinition> Tags => _tags;
    }
}
