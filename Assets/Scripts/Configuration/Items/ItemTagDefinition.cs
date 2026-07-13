using UnityEngine;

namespace Azulon.Configuration.Items
{
    [CreateAssetMenu(
        fileName = "Tag_New",
        menuName = "Guild Relic Market/Items/Tag Definition",
        order = 10)]
    public sealed class ItemTagDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Color _displayColor = Color.white;

        public string Id => _id;

        public string DisplayName => _displayName;

        public Color DisplayColor => _displayColor;
    }
}
