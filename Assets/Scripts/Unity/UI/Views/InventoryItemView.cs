using System;
using Azulon.Presentation.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI.Views
{
    public sealed class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _quantityText;
        [SerializeField] private TMP_Text _powerText;
        [SerializeField] private TMP_Text _rarityText;
        [SerializeField] private TMP_Text _categoryText;
        [SerializeField] private TMP_Text _tagsText;

        public void Bind(InventoryItemViewData viewData, Sprite icon)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            _icon.sprite = icon;
            _icon.enabled = icon != null;
            _nameText.text = viewData.Item.DisplayName;
            _descriptionText.text = viewData.Item.Description;
            _quantityText.text = $"x{viewData.Quantity}";
            _powerText.text = $"Power {(long)viewData.Item.Power * viewData.Quantity}";
            _rarityText.text = viewData.Item.Rarity.ToString();
            _categoryText.text = viewData.Item.Category.ToString();
            _tagsText.text = string.Join(" | ", viewData.Item.TagNames);
        }

        public bool TryValidateReferences(out string error)
        {
            if (_icon == null ||
                _nameText == null ||
                _descriptionText == null ||
                _quantityText == null ||
                _powerText == null ||
                _rarityText == null ||
                _categoryText == null ||
                _tagsText == null)
            {
                error = $"Inventory item prefab '{name}' has missing serialized references.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
