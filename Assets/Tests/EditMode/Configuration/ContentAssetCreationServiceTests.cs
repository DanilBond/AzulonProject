using Azulon.Configuration.Editor;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Configuration.Quests.Requirements;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class ContentAssetCreationServiceTests
    {
        private const string TestRoot = "Assets/__AzulonContentAuthoringTests";

        private ContentAssetFolders _folders;
        private ContentAssetCreationService _creationService;

        [SetUp]
        public void SetUp()
        {
            DeleteTestRoot();
            AssetDatabase.CreateFolder("Assets", "__AzulonContentAuthoringTests");
            _folders = new ContentAssetFolders(
                $"{TestRoot}/Tags",
                $"{TestRoot}/Items",
                $"{TestRoot}/Requirements",
                $"{TestRoot}/Quests");
            _creationService = new ContentAssetCreationService(_folders);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestRoot();
        }

        [Test]
        public void CreateTag_CreatesAssetAndRegistersItInCatalog()
        {
            var catalog = CreateAsset<ItemCatalogAsset>("ItemCatalog.asset");

            var tag = _creationService.CreateTag(
                catalog,
                "fire",
                "Fire",
                Color.red);

            Assert.That(catalog.TagDefinitions, Has.Count.EqualTo(1));
            Assert.That(catalog.TagDefinitions[0], Is.SameAs(tag));
            Assert.That(AssetDatabase.GetAssetPath(tag), Does.StartWith(_folders.Tags));
        }

        [Test]
        public void CreateTag_WithDuplicateStableId_DoesNotCreateSecondAsset()
        {
            var catalog = CreateAsset<ItemCatalogAsset>("ItemCatalog.asset");
            _creationService.CreateTag(catalog, "fire", "Fire", Color.red);

            Assert.That(
                () => _creationService.CreateTag(catalog, "fire", "Other Fire", Color.yellow),
                Throws.TypeOf<System.InvalidOperationException>());
            Assert.That(catalog.TagDefinitions, Has.Count.EqualTo(1));
        }

        [Test]
        public void CreateQuest_CreatesRequirementAndRegistersQuest()
        {
            var itemCatalog = CreateAsset<ItemCatalogAsset>("ItemCatalog.asset");
            var questCatalog = CreateAsset<GuildQuestCatalogAsset>("QuestCatalog.asset");
            SetObjectReference(questCatalog, "_itemCatalog", itemCatalog);
            var requirement = _creationService.CreateTotalPowerRequirement(
                "Total Power",
                10);

            var quest = _creationService.CreateQuest(
                questCatalog,
                "power_test",
                "Power Test",
                "Collect enough power for the test.",
                3,
                1,
                new QuestRequirementDefinition[] { requirement });

            Assert.That(questCatalog.QuestDefinitions, Has.Count.EqualTo(1));
            Assert.That(questCatalog.QuestDefinitions[0], Is.SameAs(quest));
            Assert.That(quest.Requirements[0], Is.SameAs(requirement));
        }

        [Test]
        public void Synchronize_RegistersUnlistedAssetsFromConfiguredFolders()
        {
            var itemCatalog = CreateAsset<ItemCatalogAsset>("ItemCatalog.asset");
            var questCatalog = CreateAsset<GuildQuestCatalogAsset>("QuestCatalog.asset");
            var tag = ScriptableObject.CreateInstance<ItemTagDefinition>();
            tag.name = "Tag_Arcane";
            var serializedTag = new SerializedObject(tag);
            serializedTag.FindProperty("_id").stringValue = "arcane";
            serializedTag.FindProperty("_displayName").stringValue = "Arcane";
            serializedTag.ApplyModifiedPropertiesWithoutUndo();
            EnsureFolder(_folders.Tags);
            AssetDatabase.CreateAsset(tag, $"{_folders.Tags}/Tag_Arcane.asset");

            var result = new ContentCatalogSynchronizer(_folders).Synchronize(
                itemCatalog,
                questCatalog);

            Assert.That(result.AddedTags, Is.EqualTo(1));
            Assert.That(result.AddedItems, Is.Zero);
            Assert.That(result.AddedQuests, Is.Zero);
            Assert.That(itemCatalog.TagDefinitions[0], Is.SameAs(tag));
        }

        private static T CreateAsset<T>(string fileName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{TestRoot}/{fileName}");
            return asset;
        }

        private static void SetObjectReference(
            ScriptableObject target,
            string propertyName,
            Object value)
        {
            var serializedTarget = new SerializedObject(target);
            serializedTarget.FindProperty(propertyName).objectReferenceValue = value;
            serializedTarget.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string folder)
        {
            var parts = folder.Split('/');
            var current = parts[0];
            for (var index = 1; index < parts.Length; index++)
            {
                var next = $"{current}/{parts[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void DeleteTestRoot()
        {
            if (AssetDatabase.IsValidFolder(TestRoot))
            {
                AssetDatabase.DeleteAsset(TestRoot);
            }
        }
    }
}
