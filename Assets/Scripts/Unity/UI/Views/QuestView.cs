using System;
using System.Collections.Generic;
using Azulon.Domain.Quests;
using Azulon.Presentation.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI.Views
{
    public sealed class QuestView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private Transform _requirementsContent;
        [SerializeField] private QuestRequirementView _requirementPrefab;
        [SerializeField] private Button _claimButton;
        [SerializeField] private TMP_Text _claimButtonText;
        [SerializeField] private GameObject _readyState;
        [SerializeField] private GameObject _claimedState;

        private readonly List<QuestRequirementView> _requirementViews =
            new List<QuestRequirementView>();
        private QuestId _questId;
        private Action<QuestId> _claimRequested;

        public void Bind(
            QuestViewData viewData,
            GameContentViewCatalog contentCatalog,
            Action<QuestId> claimRequested)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (contentCatalog == null)
            {
                throw new ArgumentNullException(nameof(contentCatalog));
            }

            _questId = viewData.Id;
            _claimRequested = claimRequested ?? throw new ArgumentNullException(nameof(claimRequested));
            _nameText.text = viewData.DisplayName;
            _descriptionText.text = viewData.Description;
            _rewardText.text = FormatRewards(viewData);
            _readyState.SetActive(viewData.CanClaim);
            _claimedState.SetActive(viewData.IsClaimed);
            _claimButton.interactable = viewData.CanClaim;
            _claimButtonText.text = viewData.IsClaimed
                ? "Claimed"
                : viewData.CanClaim
                    ? "Claim"
                    : "In progress";

            EnsureRequirementViewCount(viewData.Requirements.Count);
            for (var index = 0; index < _requirementViews.Count; index++)
            {
                var requirementView = _requirementViews[index];
                var isUsed = index < viewData.Requirements.Count;
                requirementView.gameObject.SetActive(isUsed);
                if (!isUsed)
                {
                    continue;
                }

                var requirement = viewData.Requirements[index];
                requirementView.Bind(
                    contentCatalog.GetRequirementDisplayName(viewData.Id, requirement.Index),
                    requirement);
            }

            _claimButton.onClick.RemoveListener(HandleClaimClicked);
            _claimButton.onClick.AddListener(HandleClaimClicked);
        }

        public bool TryValidateReferences(out string error)
        {
            if (_nameText == null ||
                _descriptionText == null ||
                _rewardText == null ||
                _requirementsContent == null ||
                _requirementPrefab == null ||
                _claimButton == null ||
                _claimButtonText == null ||
                _readyState == null ||
                _claimedState == null)
            {
                error = $"Quest prefab '{name}' has missing serialized references.";
                return false;
            }

            return _requirementPrefab.TryValidateReferences(out error);
        }

        private void OnDestroy()
        {
            if (_claimButton != null)
            {
                _claimButton.onClick.RemoveListener(HandleClaimClicked);
            }
        }

        private void EnsureRequirementViewCount(int requiredCount)
        {
            while (_requirementViews.Count < requiredCount)
            {
                _requirementViews.Add(Instantiate(
                    _requirementPrefab,
                    _requirementsContent));
            }
        }

        private void HandleClaimClicked()
        {
            _claimRequested?.Invoke(_questId);
        }

        private static string FormatRewards(QuestViewData viewData)
        {
            if (viewData.RewardCoins > 0 && viewData.RewardReputation > 0)
            {
                return $"Reward: {viewData.RewardCoins} coins | {viewData.RewardReputation} reputation";
            }

            if (viewData.RewardCoins > 0)
            {
                return $"Reward: {viewData.RewardCoins} coins";
            }

            return $"Reward: {viewData.RewardReputation} reputation";
        }
    }
}
