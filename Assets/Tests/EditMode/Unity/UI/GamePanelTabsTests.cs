using Azulon.Unity.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Azulon.Tests.EditMode.Unity.UI
{
    public sealed class GamePanelTabsTests
    {
        private GameObject _root;
        private ToggleGroup _toggleGroup;
        private Toggle _collectionToggle;
        private Toggle _inventoryToggle;
        private Toggle _questsToggle;
        private GameObject _collectionPanel;
        private GameObject _inventoryPanel;
        private GameObject _questsPanel;
        private GamePanelTabs _tabs;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("GamePanelTabs_Test");
            _root.SetActive(false);
            _toggleGroup = _root.AddComponent<ToggleGroup>();
            _toggleGroup.allowSwitchOff = false;
            _collectionToggle = CreateToggle("CollectionToggle");
            _inventoryToggle = CreateToggle("InventoryToggle");
            _questsToggle = CreateToggle("QuestsToggle");
            _collectionPanel = CreateChild("CollectionPanel");
            _inventoryPanel = CreateChild("InventoryPanel");
            _questsPanel = CreateChild("QuestsPanel");
            _tabs = _root.AddComponent<GamePanelTabs>();

            var serializedTabs = new SerializedObject(_tabs);
            serializedTabs.FindProperty("_initialPanel").intValue =
                (int)GameSidePanel.Inventory;
            serializedTabs.FindProperty("_collectionToggle").objectReferenceValue =
                _collectionToggle;
            serializedTabs.FindProperty("_inventoryToggle").objectReferenceValue =
                _inventoryToggle;
            serializedTabs.FindProperty("_questsToggle").objectReferenceValue =
                _questsToggle;
            serializedTabs.FindProperty("_collectionPanel").objectReferenceValue =
                _collectionPanel;
            serializedTabs.FindProperty("_inventoryPanel").objectReferenceValue =
                _inventoryPanel;
            serializedTabs.FindProperty("_questsPanel").objectReferenceValue =
                _questsPanel;
            serializedTabs.ApplyModifiedPropertiesWithoutUndo();

            _collectionToggle.SetIsOnWithoutNotify(false);
            _inventoryToggle.SetIsOnWithoutNotify(false);
            _questsToggle.SetIsOnWithoutNotify(false);
            _root.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void Awake_OpensInitialPanelAndAllowsLastToggleToSwitchOff()
        {
            Assert.That(_toggleGroup.allowSwitchOff, Is.True);
            Assert.That(_tabs.CurrentPanel, Is.EqualTo(GameSidePanel.Inventory));
            Assert.That(_tabs.IsAnyPanelOpen, Is.True);
            Assert.That(_inventoryToggle.isOn, Is.True);
            Assert.That(_inventoryPanel.activeSelf, Is.True);
            Assert.That(_collectionPanel.activeSelf, Is.False);
            Assert.That(_questsPanel.activeSelf, Is.False);
        }

        [Test]
        public void TurningOffActiveToggle_ClosesEveryPanel()
        {
            _inventoryToggle.isOn = false;

            Assert.That(_tabs.CurrentPanel, Is.EqualTo(GameSidePanel.None));
            Assert.That(_tabs.IsAnyPanelOpen, Is.False);
            Assert.That(_collectionPanel.activeSelf, Is.False);
            Assert.That(_inventoryPanel.activeSelf, Is.False);
            Assert.That(_questsPanel.activeSelf, Is.False);
        }

        [Test]
        public void TurningOnDifferentToggle_SwitchesToRequestedPanel()
        {
            _collectionToggle.isOn = true;

            Assert.That(_tabs.CurrentPanel, Is.EqualTo(GameSidePanel.Collection));
            Assert.That(_collectionToggle.isOn, Is.True);
            Assert.That(_inventoryToggle.isOn, Is.False);
            Assert.That(_collectionPanel.activeSelf, Is.True);
            Assert.That(_inventoryPanel.activeSelf, Is.False);
            Assert.That(_questsPanel.activeSelf, Is.False);
        }

        [Test]
        public void ShowNone_ClosesCurrentPanel()
        {
            _tabs.Show(GameSidePanel.None);

            Assert.That(_tabs.CurrentPanel, Is.EqualTo(GameSidePanel.None));
            Assert.That(_collectionToggle.isOn, Is.False);
            Assert.That(_inventoryToggle.isOn, Is.False);
            Assert.That(_questsToggle.isOn, Is.False);
        }

        private Toggle CreateToggle(string name)
        {
            var toggleObject = CreateChild(name);
            var toggle = toggleObject.AddComponent<Toggle>();
            toggle.group = _toggleGroup;
            return toggle;
        }

        private GameObject CreateChild(string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(_root.transform);
            return child;
        }
    }
}
