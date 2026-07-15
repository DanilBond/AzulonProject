using System;
using Azulon.Presentation.Gameplay;
using TMPro;
using UnityEngine;

namespace Azulon.Unity.UI.Views
{
    public sealed class QuestRequirementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _displayNameText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private GameObject _completedState;

        public void Bind(string displayName, QuestRequirementViewData viewData)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Requirement display name cannot be empty.", nameof(displayName));
            }

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            _displayNameText.text = displayName;
            _progressText.text = $"{viewData.Current} / {viewData.Required}";
            _completedState.SetActive(viewData.IsSatisfied);
        }

        public bool TryValidateReferences(out string error)
        {
            if (_displayNameText == null || _progressText == null || _completedState == null)
            {
                error = $"Quest requirement prefab '{name}' has missing serialized references.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
