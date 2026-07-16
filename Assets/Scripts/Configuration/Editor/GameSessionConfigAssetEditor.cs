using Azulon.Configuration.Game;
using Azulon.Configuration.Game.Validation;
using Azulon.Configuration.Validation;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    [CustomEditor(typeof(GameSessionConfigAsset))]
    public sealed class GameSessionConfigAssetEditor : UnityEditor.Editor
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
            var result = GameSessionConfigValidator.Validate((GameSessionConfigAsset)target);
            if (result.IsValid)
            {
                EditorGUILayout.HelpBox(
                    "Game session configuration is valid.",
                    MessageType.Info);
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
