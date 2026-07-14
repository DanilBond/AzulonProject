using Azulon.Configuration.Quests;
using Azulon.Configuration.Quests.Validation;
using Azulon.Configuration.Validation;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    [CustomEditor(typeof(GuildQuestCatalogAsset))]
    public sealed class GuildQuestCatalogAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(8f);
            var result = GuildQuestCatalogValidator.Validate((GuildQuestCatalogAsset)target);
            if (result.IsValid)
            {
                EditorGUILayout.HelpBox("Quest catalog configuration is valid.", MessageType.Info);
                return;
            }

            foreach (var issue in result.Issues)
            {
                var messageType = issue.Severity == CatalogValidationSeverity.Error
                    ? MessageType.Error
                    : MessageType.Warning;
                EditorGUILayout.HelpBox(issue.Message, messageType);
            }
        }
    }
}
