using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests.Requirements;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed partial class GuildContentAuthoringWindow
    {
        private enum RequirementKind
        {
            ExactItem = 0,
            TagCount = 1,
            TotalPower = 2,
            UniqueItemCount = 3
        }

        [SerializeField] private RequirementKind _requirementKind;
        [SerializeField] private string _requirementDisplayName;
        [SerializeField] private ItemDefinition _requirementItem;
        [SerializeField] private ItemTagDefinition _requirementTag;
        [SerializeField] private int _requirementValue = 1;
        [SerializeField] private bool _addRequirementToQuestDraft = true;

        [SerializeField] private string _questDisplayName;
        [SerializeField] private string _questId;
        [SerializeField] private string _questDescription;
        [SerializeField] private int _questRewardCoins = 4;
        [SerializeField] private int _questRewardReputation = 1;
        [SerializeField] private List<QuestRequirementDefinition> _questRequirements =
            new List<QuestRequirementDefinition>();

        private void DrawRequirementTool()
        {
            EditorGUILayout.LabelField("Create Quest Requirement", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Requirements are reusable strategy assets. They do not need catalog registration.",
                MessageType.Info);
            _requirementKind = (RequirementKind)EditorGUILayout.EnumPopup(
                "Requirement Type",
                _requirementKind);
            _requirementDisplayName = EditorGUILayout.TextField(
                "Display Name",
                _requirementDisplayName);

            switch (_requirementKind)
            {
                case RequirementKind.ExactItem:
                    _requirementItem = (ItemDefinition)EditorGUILayout.ObjectField(
                        "Item",
                        _requirementItem,
                        typeof(ItemDefinition),
                        false);
                    _requirementValue = EditorGUILayout.IntField(
                        "Required Quantity",
                        _requirementValue);
                    break;
                case RequirementKind.TagCount:
                    _requirementTag = (ItemTagDefinition)EditorGUILayout.ObjectField(
                        "Tag",
                        _requirementTag,
                        typeof(ItemTagDefinition),
                        false);
                    _requirementValue = EditorGUILayout.IntField(
                        "Required Quantity",
                        _requirementValue);
                    break;
                case RequirementKind.TotalPower:
                    _requirementValue = EditorGUILayout.IntField(
                        "Required Power",
                        _requirementValue);
                    break;
                case RequirementKind.UniqueItemCount:
                    _requirementValue = EditorGUILayout.IntField(
                        "Required Unique Items",
                        _requirementValue);
                    break;
            }

            _addRequirementToQuestDraft = EditorGUILayout.Toggle(
                "Add To Quest Draft",
                _addRequirementToQuestDraft);
            using (new EditorGUI.DisabledScope(ItemCatalog == null))
            {
                if (GUILayout.Button("Create Requirement", GUILayout.Height(32f)))
                {
                    var created = CreateRequirementFromDraft();
                    if (created != null)
                    {
                        if (_addRequirementToQuestDraft &&
                            !_questRequirements.Contains(created))
                        {
                            _questRequirements.Add(created);
                        }

                        _requirementDisplayName = string.Empty;
                        _requirementValue = 1;
                    }
                }
            }
        }

        private QuestRequirementDefinition CreateRequirementFromDraft()
        {
            switch (_requirementKind)
            {
                case RequirementKind.ExactItem:
                    return TryCreate(() => _creationService.CreateExactItemRequirement(
                        ItemCatalog,
                        _requirementDisplayName,
                        _requirementItem,
                        _requirementValue));
                case RequirementKind.TagCount:
                    return TryCreate(() => _creationService.CreateTagCountRequirement(
                        ItemCatalog,
                        _requirementDisplayName,
                        _requirementTag,
                        _requirementValue));
                case RequirementKind.TotalPower:
                    return TryCreate(() => _creationService.CreateTotalPowerRequirement(
                        _requirementDisplayName,
                        _requirementValue));
                case RequirementKind.UniqueItemCount:
                    return TryCreate(() => _creationService.CreateUniqueItemCountRequirement(
                        ItemCatalog,
                        _requirementDisplayName,
                        _requirementValue));
                default:
                    return null;
            }
        }

        private void DrawQuestTool()
        {
            EditorGUILayout.LabelField("Create Guild Quest", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The quest is automatically appended to the selected quest catalog.",
                MessageType.Info);
            _questDisplayName = EditorGUILayout.TextField(
                "Display Name",
                _questDisplayName);
            DrawStableIdField("Stable ID", _questDisplayName, ref _questId);
            EditorGUILayout.LabelField("Description");
            _questDescription = EditorGUILayout.TextArea(
                _questDescription ?? string.Empty,
                GUILayout.MinHeight(56f));
            _questRewardCoins = EditorGUILayout.IntField(
                "Reward Coins",
                _questRewardCoins);
            _questRewardReputation = EditorGUILayout.IntField(
                "Reward Reputation",
                _questRewardReputation);

            EditorGUILayout.Space(6f);
            DrawQuestRequirementList();
            using (new EditorGUI.DisabledScope(QuestCatalog == null))
            {
                if (GUILayout.Button("Create Quest", GUILayout.Height(32f)))
                {
                    var created = TryCreate(() => _creationService.CreateQuest(
                        QuestCatalog,
                        _questId,
                        _questDisplayName,
                        _questDescription,
                        _questRewardCoins,
                        _questRewardReputation,
                        _questRequirements));
                    if (created != null)
                    {
                        ResetQuestDraft();
                    }
                }
            }
        }

        private void DrawQuestRequirementList()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Requirements", EditorStyles.boldLabel);
                if (GUILayout.Button(
                        new GUIContent("+", "Add requirement slot"),
                        GUILayout.Width(28f)))
                {
                    _questRequirements.Add(null);
                }
            }

            if (_questRequirements.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "Add at least one requirement asset.",
                    MessageType.Warning);
                return;
            }

            for (var index = 0; index < _questRequirements.Count; index++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _questRequirements[index] =
                        (QuestRequirementDefinition)EditorGUILayout.ObjectField(
                            $"Requirement {index + 1}",
                            _questRequirements[index],
                            typeof(QuestRequirementDefinition),
                            false);
                    if (GUILayout.Button(
                            new GUIContent("-", "Remove requirement"),
                            GUILayout.Width(28f)))
                    {
                        _questRequirements.RemoveAt(index);
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }

        private void ResetQuestDraft()
        {
            _questDisplayName = string.Empty;
            _questId = string.Empty;
            _questDescription = string.Empty;
            _questRequirements.Clear();
        }
    }
}
