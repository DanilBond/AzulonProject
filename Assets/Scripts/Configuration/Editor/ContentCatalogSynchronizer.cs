using System;
using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed class ContentCatalogSyncResult
    {
        private readonly List<string> _issues = new List<string>();

        public int AddedTags { get; internal set; }

        public int AddedItems { get; internal set; }

        public int AddedQuests { get; internal set; }

        public IReadOnlyList<string> Issues => _issues.AsReadOnly();

        public bool HasIssues => _issues.Count > 0;

        public int TotalAdded => AddedTags + AddedItems + AddedQuests;

        internal void AddIssue(string issue)
        {
            _issues.Add(issue);
        }
    }

    public sealed class ContentCatalogSynchronizer
    {
        private readonly ContentAssetFolders _folders;

        public ContentCatalogSynchronizer(ContentAssetFolders folders = null)
        {
            _folders = folders ?? ContentAssetFolders.Default;
        }

        public ContentCatalogSyncResult Synchronize(
            ItemCatalogAsset itemCatalog,
            GuildQuestCatalogAsset questCatalog)
        {
            if (itemCatalog == null)
            {
                throw new ArgumentNullException(nameof(itemCatalog));
            }

            if (questCatalog == null)
            {
                throw new ArgumentNullException(nameof(questCatalog));
            }

            var result = new ContentCatalogSyncResult();
            result.AddedTags = SynchronizeDefinitions(
                itemCatalog,
                "_tagDefinitions",
                itemCatalog.TagDefinitions,
                ContentEditorAssetUtility.FindAssets<ItemTagDefinition>(_folders.Tags),
                definition => definition.Id,
                "tag",
                result);
            result.AddedItems = SynchronizeDefinitions(
                itemCatalog,
                "_itemDefinitions",
                itemCatalog.ItemDefinitions,
                ContentEditorAssetUtility.FindAssets<ItemDefinition>(_folders.Items),
                definition => definition.Id,
                "item",
                result);
            result.AddedQuests = SynchronizeDefinitions(
                questCatalog,
                "_questDefinitions",
                questCatalog.QuestDefinitions,
                ContentEditorAssetUtility.FindAssets<GuildQuestDefinition>(_folders.Quests),
                definition => definition.Id,
                "quest",
                result);

            AssetDatabase.SaveAssets();
            return result;
        }

        private static int SynchronizeDefinitions<T>(
            ScriptableObject owner,
            string propertyName,
            IReadOnlyList<T> registered,
            IReadOnlyList<T> discovered,
            Func<T, string> idSelector,
            string contentType,
            ContentCatalogSyncResult result)
            where T : UnityEngine.Object
        {
            var registeredAssets = new HashSet<T>();
            var assetsById = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var asset in registered)
            {
                if (asset == null)
                {
                    continue;
                }

                registeredAssets.Add(asset);
                var id = idSelector(asset);
                if (!string.IsNullOrWhiteSpace(id) && !assetsById.ContainsKey(id))
                {
                    assetsById.Add(id, asset);
                }
            }

            var added = 0;
            foreach (var asset in discovered)
            {
                if (registeredAssets.Contains(asset))
                {
                    continue;
                }

                var id = idSelector(asset);
                if (!string.IsNullOrWhiteSpace(id) &&
                    assetsById.TryGetValue(id, out var existingAsset))
                {
                    result.AddIssue(
                        $"Skipped {contentType} '{asset.name}': ID '{id}' is already used by '{existingAsset.name}'.");
                    continue;
                }

                ContentEditorAssetUtility.AppendObjectReference(
                    owner,
                    propertyName,
                    asset,
                    $"Synchronize {contentType} catalog");
                registeredAssets.Add(asset);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    assetsById[id] = asset;
                }

                added++;
            }

            return added;
        }
    }
}
