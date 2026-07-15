using System;
using Azulon.Domain.Market;
using Azulon.Presentation.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI.Views
{
    public sealed class MarketOfferView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _powerText;
        [SerializeField] private TMP_Text _rarityText;
        [SerializeField] private TMP_Text _categoryText;
        [SerializeField] private TMP_Text _tagsText;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TMP_Text _purchaseButtonText;
        [SerializeField] private GameObject _purchasedState;

        private MarketOfferId _offerId;
        private Action<MarketOfferId> _purchaseRequested;

        public void Bind(
            MarketOfferViewData viewData,
            Sprite icon,
            Action<MarketOfferId> purchaseRequested)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            _offerId = viewData.OfferId;
            _purchaseRequested = purchaseRequested ??
                throw new ArgumentNullException(nameof(purchaseRequested));

            _icon.sprite = icon;
            _icon.enabled = icon != null;
            _nameText.text = viewData.Item.DisplayName;
            _descriptionText.text = viewData.Item.Description;
            _priceText.text = $"{viewData.Item.Price} coins";
            _powerText.text = $"Power {viewData.Item.Power}";
            _rarityText.text = viewData.Item.Rarity.ToString();
            _categoryText.text = viewData.Item.Category.ToString();
            _tagsText.text = string.Join(" | ", viewData.Item.TagNames);
            _purchasedState.SetActive(viewData.IsPurchased);
            _purchaseButton.interactable = viewData.CanPurchase;
            _purchaseButtonText.text = viewData.IsPurchased
                ? "Sold"
                : viewData.CanPurchase
                    ? "Buy"
                    : "Not enough";

            _purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);
            _purchaseButton.onClick.AddListener(HandlePurchaseClicked);
        }

        public bool TryValidateReferences(out string error)
        {
            if (_icon == null ||
                _nameText == null ||
                _descriptionText == null ||
                _priceText == null ||
                _powerText == null ||
                _rarityText == null ||
                _categoryText == null ||
                _tagsText == null ||
                _purchaseButton == null ||
                _purchaseButtonText == null ||
                _purchasedState == null)
            {
                error = $"Market offer prefab '{name}' has missing serialized references.";
                return false;
            }

            error = null;
            return true;
        }

        private void OnDestroy()
        {
            if (_purchaseButton != null)
            {
                _purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);
            }
        }

        private void HandlePurchaseClicked()
        {
            _purchaseRequested?.Invoke(_offerId);
        }
    }
}
