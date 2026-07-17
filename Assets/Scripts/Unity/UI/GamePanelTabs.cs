using System;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI
{
    public enum GameSidePanel
    {
        None = -1,
        Collection = 0,
        Inventory = 1,
        Quests = 2
    }

    [DisallowMultipleComponent]
    public sealed class GamePanelTabs : MonoBehaviour
    {
        [SerializeField] private GameSidePanel _initialPanel = GameSidePanel.Inventory;
        [SerializeField] private Toggle _collectionToggle;
        [SerializeField] private Toggle _inventoryToggle;
        [SerializeField] private Toggle _questsToggle;
        [SerializeField] private GameObject _collectionPanel;
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private GameObject _questsPanel;

        public GameSidePanel CurrentPanel { get; private set; } = GameSidePanel.None;

        public bool IsAnyPanelOpen => CurrentPanel != GameSidePanel.None;

        private void Awake()
        {
            if (!TryValidateReferences(out var error))
            {
                Debug.LogError(error, this);
                enabled = false;
                return;
            }

            _collectionToggle.group.allowSwitchOff = true;
            _collectionToggle.onValueChanged.AddListener(HandleCollectionChanged);
            _inventoryToggle.onValueChanged.AddListener(HandleInventoryChanged);
            _questsToggle.onValueChanged.AddListener(HandleQuestsChanged);
            ApplyState(_initialPanel, true);
        }

        private void OnDestroy()
        {
            if (_collectionToggle != null)
            {
                _collectionToggle.onValueChanged.RemoveListener(HandleCollectionChanged);
            }

            if (_inventoryToggle != null)
            {
                _inventoryToggle.onValueChanged.RemoveListener(HandleInventoryChanged);
            }

            if (_questsToggle != null)
            {
                _questsToggle.onValueChanged.RemoveListener(HandleQuestsChanged);
            }
        }

        public void Show(GameSidePanel panel)
        {
            if (panel == GameSidePanel.None)
            {
                Close();
                return;
            }

            if (panel != GameSidePanel.Collection &&
                panel != GameSidePanel.Inventory &&
                panel != GameSidePanel.Quests)
            {
                throw new ArgumentOutOfRangeException(nameof(panel), panel, "Unknown side panel.");
            }

            ApplyState(panel, false);
        }

        public void Close()
        {
            ApplyState(GameSidePanel.None, false);
        }

        private void ApplyState(GameSidePanel panel, bool instant)
        {
            CurrentPanel = panel;
            SetPanelVisible(
                _collectionPanel,
                panel == GameSidePanel.Collection,
                instant);
            SetPanelVisible(
                _inventoryPanel,
                panel == GameSidePanel.Inventory,
                instant);
            SetPanelVisible(
                _questsPanel,
                panel == GameSidePanel.Quests,
                instant);
            _collectionToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Collection);
            _inventoryToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Inventory);
            _questsToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Quests);
        }

        private static void SetPanelVisible(
            GameObject panel,
            bool visible,
            bool instant)
        {
            var animator = panel.GetComponent<UiVisibilityAnimator>();
            if (animator != null)
            {
                animator.SetVisible(visible, instant);
                return;
            }

            panel.SetActive(visible);
        }

        public bool TryValidateReferences(out string error)
        {
            if (_collectionToggle == null ||
                _inventoryToggle == null ||
                _questsToggle == null ||
                _collectionPanel == null ||
                _inventoryPanel == null ||
                _questsPanel == null)
            {
                error = $"Game panel tabs '{name}' have missing serialized references.";
                return false;
            }

            var toggleGroup = _collectionToggle.group;
            if (toggleGroup == null ||
                _inventoryToggle.group != toggleGroup ||
                _questsToggle.group != toggleGroup)
            {
                error = $"Game panel tabs '{name}' must use the same ToggleGroup.";
                return false;
            }

            if (!Enum.IsDefined(typeof(GameSidePanel), _initialPanel))
            {
                error = $"Game panel tabs '{name}' have an unknown initial panel.";
                return false;
            }

            error = null;
            return true;
        }

        private void HandleCollectionChanged(bool isOn)
        {
            HandleToggleChanged(GameSidePanel.Collection, isOn);
        }

        private void HandleInventoryChanged(bool isOn)
        {
            HandleToggleChanged(GameSidePanel.Inventory, isOn);
        }

        private void HandleQuestsChanged(bool isOn)
        {
            HandleToggleChanged(GameSidePanel.Quests, isOn);
        }

        private void HandleToggleChanged(GameSidePanel panel, bool isOn)
        {
            if (isOn)
            {
                Show(panel);
                return;
            }

            if (CurrentPanel == panel && !AreAnyTogglesOn())
            {
                Close();
            }
        }

        private bool AreAnyTogglesOn()
        {
            return _collectionToggle.isOn ||
                   _inventoryToggle.isOn ||
                   _questsToggle.isOn;
        }
    }
}
