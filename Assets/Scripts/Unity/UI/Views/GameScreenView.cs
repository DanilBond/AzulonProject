using System;
using System.Collections.Generic;
using Azulon.Domain.Market;
using Azulon.Domain.Quests;
using Azulon.Presentation.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI.Views
{
    public sealed class GameScreenView : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private TMP_Text _reputationText;
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _visitorText;
        [SerializeField] private TMP_Text _rarityText;
        [SerializeField] private TMP_Text _nextRarityText;
        [SerializeField] private TMP_Text _collectionProgressText;
        [SerializeField] private TMP_Text _questProgressText;

        [Header("Market")]
        [SerializeField] private Transform _marketContent;
        [SerializeField] private MarketOfferView _marketOfferPrefab;
        [SerializeField] private Button _nextVisitorButton;

        [Header("Collection")]
        [SerializeField] private Transform _collectionContent;
        [SerializeField] private CollectionItemView _collectionItemPrefab;

        [Header("Inventory")]
        [SerializeField] private Transform _inventoryContent;
        [SerializeField] private InventoryItemView _inventoryItemPrefab;
        [SerializeField] private GameObject _emptyInventoryState;

        [Header("Quests")]
        [SerializeField] private Transform _questContent;
        [SerializeField] private QuestView _questPrefab;

        [Header("Feedback")]
        [SerializeField] private TMP_Text _feedbackText;
        [SerializeField] private Color _neutralFeedbackColor = Color.white;
        [SerializeField] private Color _positiveFeedbackColor = new Color(0.35f, 0.85f, 0.45f);
        [SerializeField] private Color _warningFeedbackColor = new Color(1f, 0.65f, 0.25f);
        [SerializeField] private GameObject _completionOverlay;
        [SerializeField] private GameObject _fatalErrorPanel;
        [SerializeField] private TMP_Text _fatalErrorText;

        private readonly List<MarketOfferView> _marketOfferViews =
            new List<MarketOfferView>();
        private readonly List<InventoryItemView> _inventoryItemViews =
            new List<InventoryItemView>();
        private readonly List<CollectionItemView> _collectionItemViews =
            new List<CollectionItemView>();
        private readonly List<QuestView> _questViews = new List<QuestView>();

        private Action<MarketOfferId> _purchaseRequested;
        private Action<QuestId> _claimRequested;
        private Action _nextVisitorRequested;

        public void Initialize(
            Action<MarketOfferId> purchaseRequested,
            Action<QuestId> claimRequested,
            Action nextVisitorRequested)
        {
            Release();
            _purchaseRequested = purchaseRequested ??
                throw new ArgumentNullException(nameof(purchaseRequested));
            _claimRequested = claimRequested ?? throw new ArgumentNullException(nameof(claimRequested));
            _nextVisitorRequested = nextVisitorRequested ??
                throw new ArgumentNullException(nameof(nextVisitorRequested));

            _nextVisitorButton.onClick.AddListener(HandleNextVisitorClicked);
            _feedbackText.gameObject.SetActive(false);
            _fatalErrorPanel.SetActive(false);
        }

        public void Render(
            GameScreenViewData viewData,
            GameContentViewCatalog contentCatalog)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (contentCatalog == null)
            {
                throw new ArgumentNullException(nameof(contentCatalog));
            }

            _coinsText.text = $"Coins: {viewData.Coins}";
            _reputationText.text = $"Reputation: {viewData.Reputation}";
            _dayText.text = $"Day {viewData.DayNumber}";
            _visitorText.text = $"Merchant {viewData.VisitorNumber} / {viewData.VisitorsPerDay}";
            _rarityText.text = $"Unlocked: {viewData.MaximumUnlockedRarity}";
            _nextRarityText.text = FormatNextRarity(viewData);
            _collectionProgressText.text =
                $"Collection: {viewData.UniqueOwnedItemCount} / {viewData.AvailableItemCount}";
            _questProgressText.text =
                $"Commissions: {viewData.ClaimedQuestCount} / {viewData.QuestCount}";

            RenderMarket(viewData, contentCatalog);
            RenderCollection(viewData, contentCatalog);
            RenderInventory(viewData, contentCatalog);
            RenderQuests(viewData, contentCatalog);

            _nextVisitorButton.interactable = viewData.CanAdvanceVisitor;
            _completionOverlay.SetActive(viewData.IsCompleted);
        }

        public void ShowFeedback(GameFeedback feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            _feedbackText.text = feedback.Message;
            _feedbackText.color = ResolveFeedbackColor(feedback.Tone);
            _feedbackText.gameObject.SetActive(true);
        }

        public void ShowFatalError(string message)
        {
            if (_fatalErrorPanel != null)
            {
                _fatalErrorPanel.SetActive(true);
            }

            if (_fatalErrorText != null)
            {
                _fatalErrorText.text = message;
            }

            if (_nextVisitorButton != null)
            {
                _nextVisitorButton.interactable = false;
            }
        }

        public bool TryValidateReferences(out string error)
        {
            if (_coinsText == null ||
                _reputationText == null ||
                _dayText == null ||
                _visitorText == null ||
                _rarityText == null ||
                _nextRarityText == null ||
                _collectionProgressText == null ||
                _questProgressText == null ||
                _marketContent == null ||
                _marketOfferPrefab == null ||
                _nextVisitorButton == null ||
                _collectionContent == null ||
                _collectionItemPrefab == null ||
                _inventoryContent == null ||
                _inventoryItemPrefab == null ||
                _emptyInventoryState == null ||
                _questContent == null ||
                _questPrefab == null ||
                _feedbackText == null ||
                _completionOverlay == null ||
                _fatalErrorPanel == null ||
                _fatalErrorText == null)
            {
                error = $"Game screen view '{name}' has missing serialized references.";
                return false;
            }

            if (!_marketOfferPrefab.TryValidateReferences(out error))
            {
                return false;
            }

            if (!_inventoryItemPrefab.TryValidateReferences(out error))
            {
                return false;
            }

            if (!_collectionItemPrefab.TryValidateReferences(out error))
            {
                return false;
            }

            return _questPrefab.TryValidateReferences(out error);
        }

        public void Release()
        {
            if (_nextVisitorButton != null)
            {
                _nextVisitorButton.onClick.RemoveListener(HandleNextVisitorClicked);
            }

            _purchaseRequested = null;
            _claimRequested = null;
            _nextVisitorRequested = null;
        }

        private void OnDestroy()
        {
            Release();
        }

        private void RenderMarket(
            GameScreenViewData viewData,
            GameContentViewCatalog contentCatalog)
        {
            EnsureMarketViewCount(viewData.Offers.Count);
            for (var index = 0; index < _marketOfferViews.Count; index++)
            {
                var offerView = _marketOfferViews[index];
                var isUsed = index < viewData.Offers.Count;
                offerView.gameObject.SetActive(isUsed);
                if (!isUsed)
                {
                    continue;
                }

                var offer = viewData.Offers[index];
                offerView.Bind(
                    offer,
                    contentCatalog.GetItemIcon(offer.Item.Id),
                    _purchaseRequested);
            }
        }

        private void RenderInventory(
            GameScreenViewData viewData,
            GameContentViewCatalog contentCatalog)
        {
            _emptyInventoryState.SetActive(viewData.InventoryItems.Count == 0);
            EnsureInventoryViewCount(viewData.InventoryItems.Count);
            for (var index = 0; index < _inventoryItemViews.Count; index++)
            {
                var itemView = _inventoryItemViews[index];
                var isUsed = index < viewData.InventoryItems.Count;
                itemView.gameObject.SetActive(isUsed);
                if (!isUsed)
                {
                    continue;
                }

                var inventoryItem = viewData.InventoryItems[index];
                itemView.Bind(
                    inventoryItem,
                    contentCatalog.GetItemIcon(inventoryItem.Item.Id));
            }
        }

        private void RenderCollection(
            GameScreenViewData viewData,
            GameContentViewCatalog contentCatalog)
        {
            EnsureCollectionViewCount(viewData.CollectionItems.Count);
            for (var index = 0; index < _collectionItemViews.Count; index++)
            {
                var itemView = _collectionItemViews[index];
                var isUsed = index < viewData.CollectionItems.Count;
                itemView.gameObject.SetActive(isUsed);
                if (!isUsed)
                {
                    continue;
                }

                var collectionItem = viewData.CollectionItems[index];
                itemView.Bind(
                    collectionItem,
                    contentCatalog.GetItemIcon(collectionItem.Item.Id));
            }
        }

        private void RenderQuests(
            GameScreenViewData viewData,
            GameContentViewCatalog contentCatalog)
        {
            EnsureQuestViewCount(viewData.Quests.Count);
            for (var index = 0; index < _questViews.Count; index++)
            {
                var questView = _questViews[index];
                var isUsed = index < viewData.Quests.Count;
                questView.gameObject.SetActive(isUsed);
                if (isUsed)
                {
                    questView.Bind(
                        viewData.Quests[index],
                        contentCatalog,
                        _claimRequested);
                }
            }
        }

        private void EnsureMarketViewCount(int requiredCount)
        {
            while (_marketOfferViews.Count < requiredCount)
            {
                _marketOfferViews.Add(Instantiate(_marketOfferPrefab, _marketContent));
            }
        }

        private void EnsureInventoryViewCount(int requiredCount)
        {
            while (_inventoryItemViews.Count < requiredCount)
            {
                _inventoryItemViews.Add(Instantiate(_inventoryItemPrefab, _inventoryContent));
            }
        }

        private void EnsureCollectionViewCount(int requiredCount)
        {
            while (_collectionItemViews.Count < requiredCount)
            {
                _collectionItemViews.Add(Instantiate(
                    _collectionItemPrefab,
                    _collectionContent));
            }
        }

        private void EnsureQuestViewCount(int requiredCount)
        {
            while (_questViews.Count < requiredCount)
            {
                _questViews.Add(Instantiate(_questPrefab, _questContent));
            }
        }

        private void HandleNextVisitorClicked()
        {
            _nextVisitorRequested?.Invoke();
        }

        private Color ResolveFeedbackColor(GameFeedbackTone tone)
        {
            switch (tone)
            {
                case GameFeedbackTone.Positive:
                    return _positiveFeedbackColor;
                case GameFeedbackTone.Warning:
                    return _warningFeedbackColor;
                default:
                    return _neutralFeedbackColor;
            }
        }

        private static string FormatNextRarity(GameScreenViewData viewData)
        {
            if (!viewData.NextRarity.HasValue ||
                !viewData.NextRarityRequiredReputation.HasValue)
            {
                return "All rarities unlocked";
            }

            return $"Next: {viewData.NextRarity.Value} " +
                   $"({viewData.Reputation} / {viewData.NextRarityRequiredReputation.Value})";
        }
    }
}
