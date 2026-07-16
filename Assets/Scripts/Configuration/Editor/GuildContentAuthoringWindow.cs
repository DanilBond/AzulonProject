using System;
using Azulon.Configuration.Game;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed partial class GuildContentAuthoringWindow : EditorWindow
    {
        private enum AuthoringTab
        {
            Tags = 0,
            Items = 1,
            Requirements = 2,
            Quests = 3,
            Validation = 4
        }

        private static readonly string[] TabLabels =
        {
            "Tags",
            "Items",
            "Requirements",
            "Quests",
            "Validation"
        };

        [SerializeField] private GameSessionConfigAsset _configuration;
        [SerializeField] private AuthoringTab _activeTab;
        [SerializeField] private string _statusMessage;
        [SerializeField] private MessageType _statusType = MessageType.Info;

        private ContentAssetCreationService _creationService;
        private ContentCatalogSynchronizer _synchronizer;
        private ContentValidationReport _validationReport;
        private Vector2 _scrollPosition;

        private ItemCatalogAsset ItemCatalog =>
            _configuration == null ? null : _configuration.ItemCatalog;

        private GuildQuestCatalogAsset QuestCatalog =>
            _configuration == null ? null : _configuration.QuestCatalog;

        [MenuItem("Tools/Guild Relic Market/Content Authoring", priority = 10)]
        public static void Open()
        {
            var window = GetWindow<GuildContentAuthoringWindow>();
            window.titleContent = new GUIContent("Guild Content");
            window.minSize = new Vector2(540f, 620f);
            window.Show();
        }

        [MenuItem("Tools/Guild Relic Market/Validate All Content", priority = 20)]
        private static void ValidateAllContent()
        {
            ContentValidationService.ValidateAllProjectConfigurations();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Guild Content");
            minSize = new Vector2(540f, 620f);
            _creationService = new ContentAssetCreationService();
            _synchronizer = new ContentCatalogSynchronizer();
            TryFindConfiguration();
            RefreshValidation(false);
            EditorApplication.projectChanged += HandleProjectChanged;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= HandleProjectChanged;
        }

        private void OnGUI()
        {
            EnsureServices();
            DrawConfigurationHeader();

            _activeTab = (AuthoringTab)GUILayout.Toolbar(
                (int)_activeTab,
                TabLabels,
                GUILayout.Height(28f));

            if (!string.IsNullOrWhiteSpace(_statusMessage))
            {
                EditorGUILayout.HelpBox(_statusMessage, _statusType);
            }

            if (_configuration == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a Game Session Configuration before authoring content.",
                    MessageType.Error);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.Space(8f);
            switch (_activeTab)
            {
                case AuthoringTab.Tags:
                    DrawTagTool();
                    break;
                case AuthoringTab.Items:
                    DrawItemTool();
                    break;
                case AuthoringTab.Requirements:
                    DrawRequirementTool();
                    break;
                case AuthoringTab.Quests:
                    DrawQuestTool();
                    break;
                case AuthoringTab.Validation:
                    DrawValidationTool();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawConfigurationHeader()
        {
            EditorGUILayout.LabelField("Content Configuration", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var configuration = (GameSessionConfigAsset)EditorGUILayout.ObjectField(
                "Session Config",
                _configuration,
                typeof(GameSessionConfigAsset),
                false);
            if (EditorGUI.EndChangeCheck())
            {
                _configuration = configuration;
                _statusMessage = null;
                RefreshValidation(false);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(_configuration == null))
                {
                    if (GUILayout.Button("Select Config"))
                    {
                        Selection.activeObject = _configuration;
                        EditorGUIUtility.PingObject(_configuration);
                    }

                    if (GUILayout.Button("Sync Catalogs"))
                    {
                        SynchronizeCatalogs();
                    }

                    if (GUILayout.Button("Validate Now"))
                    {
                        RefreshValidation(true);
                        SetStatus(
                            _validationReport.IsValid
                                ? "All configured content is valid."
                                : "Content validation found errors. See the Validation tab and Console.",
                            _validationReport.IsValid ? MessageType.Info : MessageType.Error);
                    }
                }
            }

            EditorGUILayout.Space(6f);
        }

        private T TryCreate<T>(Func<T> create) where T : UnityEngine.Object
        {
            try
            {
                var asset = create();
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
                SetStatus($"Created '{asset.name}'.", MessageType.Info);
                RefreshValidation(true);
                return asset;
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, MessageType.Error);
                return null;
            }
        }

        private void SynchronizeCatalogs()
        {
            if (ItemCatalog == null || QuestCatalog == null)
            {
                SetStatus(
                    "Session configuration must reference both item and quest catalogs.",
                    MessageType.Error);
                return;
            }

            try
            {
                var result = _synchronizer.Synchronize(ItemCatalog, QuestCatalog);
                var message =
                    $"Catalog sync added {result.AddedTags} tag(s), " +
                    $"{result.AddedItems} item(s), and {result.AddedQuests} quest(s).";
                if (result.HasIssues)
                {
                    message += $"\n{string.Join("\n", result.Issues)}";
                }

                SetStatus(message, result.HasIssues ? MessageType.Warning : MessageType.Info);
                RefreshValidation(true);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, MessageType.Error);
            }
        }

        private void RefreshValidation(bool logResult)
        {
            _validationReport = logResult
                ? ContentValidationService.ValidateAndLog(_configuration)
                : ContentValidationService.Validate(_configuration);
            Repaint();
        }

        private void HandleProjectChanged()
        {
            RefreshValidation(false);
        }

        private void TryFindConfiguration()
        {
            if (_configuration != null)
            {
                return;
            }

            var configurations = ContentValidationService.FindProjectConfigurations();
            if (configurations.Count == 1)
            {
                _configuration = configurations[0];
            }
        }

        private void EnsureServices()
        {
            if (_creationService == null)
            {
                _creationService = new ContentAssetCreationService();
            }

            if (_synchronizer == null)
            {
                _synchronizer = new ContentCatalogSynchronizer();
            }
        }

        private void SetStatus(string message, MessageType type)
        {
            _statusMessage = message;
            _statusType = type;
            Repaint();
        }

        private static void DrawStableIdField(
            string label,
            string displayName,
            ref string id)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                id = EditorGUILayout.TextField(label, id);
                if (GUILayout.Button(
                        new GUIContent("Generate", "Generate a lowercase stable ID from the display name."),
                        GUILayout.Width(82f)))
                {
                    id = ContentAuthoringNameUtility.ToStableId(displayName);
                }
            }
        }
    }
}
