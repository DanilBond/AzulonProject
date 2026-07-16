using Azulon.Configuration.Items;
using Azulon.Configuration.Items.Validation;
using Azulon.Configuration.Validation;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    [CustomEditor(typeof(ItemCatalogAsset))]
    public sealed class ItemCatalogAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(8f);
            if (GUILayout.Button("Open Content Authoring"))
            {
                GuildContentAuthoringWindow.Open();
            }

            EditorGUILayout.Space(8f);
            var result = ItemCatalogValidator.Validate((ItemCatalogAsset)target);
            if (result.IsValid)
            {
                EditorGUILayout.HelpBox("Catalog configuration is valid.", MessageType.Info);
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
