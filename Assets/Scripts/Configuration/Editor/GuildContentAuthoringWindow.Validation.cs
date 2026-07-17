using System.Collections.Generic;
using Azulon.Configuration.Validation;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed partial class GuildContentAuthoringWindow
    {
        private void DrawValidationTool()
        {
            if (_validationReport == null ||
                _validationReport.Configuration != _configuration)
            {
                RefreshValidation(false);
            }

            EditorGUILayout.LabelField("Content Validation", EditorStyles.boldLabel);
            if (_validationReport.IsValid)
            {
                EditorGUILayout.HelpBox(
                    $"All content is valid. " +
                    $"{ItemCatalog.TagDefinitions.Count} tag(s), " +
                    $"{ItemCatalog.ItemDefinitions.Count} item(s), " +
                    $"{QuestCatalog.QuestDefinitions.Count} quest(s), " +
                    $"{_configuration.VisitorSprites.Count} visitor sprite(s).",
                    MessageType.Info);
            }

            DrawValidationGroup(
                "Item Catalog",
                _validationReport.ItemCatalog.IsValid,
                _validationReport.ItemCatalog.Issues,
                ItemCatalog);
            DrawValidationGroup(
                "Quest Catalog",
                _validationReport.QuestCatalog.IsValid,
                _validationReport.QuestCatalog.Issues,
                QuestCatalog);
            DrawValidationGroup(
                "Game Session",
                _validationReport.Session.IsValid,
                _validationReport.Session.Issues,
                _configuration);
        }

        private static void DrawValidationGroup(
            string title,
            bool isValid,
            IReadOnlyList<CatalogValidationIssue> issues,
            UnityEngine.Object context)
        {
            EditorGUILayout.Space(8f);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(context == null))
                {
                    if (GUILayout.Button("Select", GUILayout.Width(64f)))
                    {
                        Selection.activeObject = context;
                        EditorGUIUtility.PingObject(context);
                    }
                }
            }

            if (isValid)
            {
                EditorGUILayout.HelpBox($"{title} is valid.", MessageType.Info);
                return;
            }

            foreach (var issue in issues)
            {
                var messageType = issue.Severity == CatalogValidationSeverity.Error
                    ? MessageType.Error
                    : MessageType.Warning;
                EditorGUILayout.HelpBox(issue.Message, messageType);
            }
        }
    }
}
