using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Unity.UI
{
    public enum GameSidePanel
    {
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

        public GameSidePanel CurrentPanel { get; private set; }

        private void Awake()
        {
            if (!TryValidateReferences(out var error))
            {
                Debug.LogError(error, this);
                enabled = false;
                return;
            }

            _collectionToggle.onValueChanged.AddListener(HandleCollectionChanged);
            _inventoryToggle.onValueChanged.AddListener(HandleInventoryChanged);
            _questsToggle.onValueChanged.AddListener(HandleQuestsChanged);
            Show(_initialPanel);
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
            CurrentPanel = panel;
            _collectionPanel.SetActive(panel == GameSidePanel.Collection);
            _inventoryPanel.SetActive(panel == GameSidePanel.Inventory);
            _questsPanel.SetActive(panel == GameSidePanel.Quests);
            _collectionToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Collection);
            _inventoryToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Inventory);
            _questsToggle.SetIsOnWithoutNotify(panel == GameSidePanel.Quests);
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

            error = null;
            return true;
        }

        private void HandleCollectionChanged(bool isOn)
        {
            if (isOn)
            {
                Show(GameSidePanel.Collection);
            }
        }

        private void HandleInventoryChanged(bool isOn)
        {
            if (isOn)
            {
                Show(GameSidePanel.Inventory);
            }
        }

        private void HandleQuestsChanged(bool isOn)
        {
            if (isOn)
            {
                Show(GameSidePanel.Quests);
            }
        }
    }
}
