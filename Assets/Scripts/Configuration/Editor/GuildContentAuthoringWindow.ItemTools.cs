using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Domain.Items;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed partial class GuildContentAuthoringWindow
    {
        [SerializeField] private string _tagDisplayName;
        [SerializeField] private string _tagId;
        [SerializeField] private Color _tagColor = Color.white;

        [SerializeField] private string _itemDisplayName;
        [SerializeField] private string _itemId;
        [SerializeField] private string _itemDescription;
        [SerializeField] private Sprite _itemIcon;
        [SerializeField] private int _itemPrice = 4;
        [SerializeField] private int _itemPower = 5;
        [SerializeField] private ItemRarity _itemRarity;
        [SerializeField] private ItemCategory _itemCategory = ItemCategory.Relic;
        [SerializeField] private List<ItemTagDefinition> _itemTags =
            new List<ItemTagDefinition>();

        private void DrawTagTool()
        {
            EditorGUILayout.LabelField("Create Item Tag", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The new tag asset is automatically registered in the selected item catalog.",
                MessageType.Info);
            _tagDisplayName = EditorGUILayout.TextField("Display Name", _tagDisplayName);
            DrawStableIdField("Stable ID", _tagDisplayName, ref _tagId);
            _tagColor = EditorGUILayout.ColorField("Display Color", _tagColor);

            using (new EditorGUI.DisabledScope(ItemCatalog == null))
            {
                if (GUILayout.Button("Create Tag", GUILayout.Height(32f)))
                {
                    var created = TryCreate(() => _creationService.CreateTag(
                        ItemCatalog,
                        _tagId,
                        _tagDisplayName,
                        _tagColor));
                    if (created != null)
                    {
                        _tagDisplayName = string.Empty;
                        _tagId = string.Empty;
                    }
                }
            }

            if (ItemCatalog == null)
            {
                EditorGUILayout.HelpBox(
                    "The session configuration has no item catalog.",
                    MessageType.Error);
            }
        }

        private void DrawItemTool()
        {
            EditorGUILayout.LabelField("Create Item", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The item is created in Data/Items/Definitions and registered in catalog order.",
                MessageType.Info);
            _itemDisplayName = EditorGUILayout.TextField("Display Name", _itemDisplayName);
            DrawStableIdField("Stable ID", _itemDisplayName, ref _itemId);
            EditorGUILayout.LabelField("Description");
            _itemDescription = EditorGUILayout.TextArea(
                _itemDescription ?? string.Empty,
                GUILayout.MinHeight(56f));
            _itemIcon = (Sprite)EditorGUILayout.ObjectField(
                "Icon",
                _itemIcon,
                typeof(Sprite),
                false);
            _itemPrice = EditorGUILayout.IntField("Price", _itemPrice);
            _itemPower = EditorGUILayout.IntField("Power", _itemPower);
            _itemRarity = (ItemRarity)EditorGUILayout.EnumPopup("Rarity", _itemRarity);
            _itemCategory = (ItemCategory)EditorGUILayout.EnumPopup(
                "Category",
                _itemCategory);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            DrawItemTagSelection();

            using (new EditorGUI.DisabledScope(ItemCatalog == null))
            {
                if (GUILayout.Button("Create Item", GUILayout.Height(32f)))
                {
                    var created = TryCreate(() => _creationService.CreateItem(
                        ItemCatalog,
                        _itemId,
                        _itemDisplayName,
                        _itemDescription,
                        _itemIcon,
                        _itemPrice,
                        _itemPower,
                        _itemRarity,
                        _itemCategory,
                        _itemTags));
                    if (created != null)
                    {
                        ResetItemDraft();
                    }
                }
            }
        }

        private void DrawItemTagSelection()
        {
            if (ItemCatalog == null)
            {
                EditorGUILayout.HelpBox("No item catalog is assigned.", MessageType.Error);
                return;
            }

            RemoveUnregisteredItemTags();
            if (ItemCatalog.TagDefinitions.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "Create or synchronize at least one tag first.",
                    MessageType.Warning);
                return;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foreach (var tag in ItemCatalog.TagDefinitions)
                {
                    if (tag == null)
                    {
                        continue;
                    }

                    var selected = _itemTags.Contains(tag);
                    var updated = EditorGUILayout.ToggleLeft(
                        $"{tag.DisplayName} ({tag.Id})",
                        selected);
                    if (updated == selected)
                    {
                        continue;
                    }

                    if (updated)
                    {
                        _itemTags.Add(tag);
                    }
                    else
                    {
                        _itemTags.Remove(tag);
                    }
                }
            }
        }

        private void RemoveUnregisteredItemTags()
        {
            for (var index = _itemTags.Count - 1; index >= 0; index--)
            {
                var selectedTag = _itemTags[index];
                var isRegistered = false;
                foreach (var catalogTag in ItemCatalog.TagDefinitions)
                {
                    if (selectedTag == catalogTag)
                    {
                        isRegistered = true;
                        break;
                    }
                }

                if (!isRegistered)
                {
                    _itemTags.RemoveAt(index);
                }
            }
        }

        private void ResetItemDraft()
        {
            _itemDisplayName = string.Empty;
            _itemId = string.Empty;
            _itemDescription = string.Empty;
            _itemIcon = null;
            _itemTags.Clear();
        }
    }
}
