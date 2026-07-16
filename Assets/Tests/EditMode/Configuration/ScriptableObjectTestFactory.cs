using System;
using System.Collections.Generic;
using Azulon.Configuration.Game;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Configuration.Quests.Requirements;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Azulon.Tests.EditMode.Configuration
{
    internal sealed class ScriptableObjectTestFactory : IDisposable
    {
        private readonly List<Object> _createdObjects = new List<Object>();

        public ItemTagDefinition CreateTag(string id, string displayName)
        {
            var definition = Track(ScriptableObject.CreateInstance<ItemTagDefinition>());
            definition.name = $"Tag_{id}";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_id").stringValue = id;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public ItemDefinition CreateItem(
            string id,
            string displayName,
            string description,
            Sprite icon,
            int price,
            int power,
            ItemRarity rarity,
            ItemCategory category,
            params ItemTagDefinition[] tags)
        {
            var definition = Track(ScriptableObject.CreateInstance<ItemDefinition>());
            definition.name = $"Item_{id}";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_id").stringValue = id;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_description").stringValue = description;
            serializedObject.FindProperty("_icon").objectReferenceValue = icon;
            serializedObject.FindProperty("_price").intValue = price;
            serializedObject.FindProperty("_power").intValue = power;
            serializedObject.FindProperty("_rarity").intValue = (int)rarity;
            serializedObject.FindProperty("_category").intValue = (int)category;
            SetObjectArray(serializedObject.FindProperty("_tags"), tags);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public ItemCatalogAsset CreateCatalog(
            ItemTagDefinition[] tags,
            ItemDefinition[] items)
        {
            var catalog = Track(ScriptableObject.CreateInstance<ItemCatalogAsset>());
            catalog.name = "ItemCatalog_Test";

            var serializedObject = new SerializedObject(catalog);
            SetObjectArray(serializedObject.FindProperty("_tagDefinitions"), tags);
            SetObjectArray(serializedObject.FindProperty("_itemDefinitions"), items);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }

        public ExactItemRequirementDefinition CreateExactItemRequirement(
            string displayName,
            ItemDefinition item,
            int requiredQuantity)
        {
            var definition = Track(ScriptableObject.CreateInstance<ExactItemRequirementDefinition>());
            definition.name = "Requirement_ExactItem_Test";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_item").objectReferenceValue = item;
            serializedObject.FindProperty("_requiredQuantity").intValue = requiredQuantity;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public TagCountRequirementDefinition CreateTagCountRequirement(
            string displayName,
            ItemTagDefinition tag,
            int requiredQuantity)
        {
            var definition = Track(ScriptableObject.CreateInstance<TagCountRequirementDefinition>());
            definition.name = "Requirement_TagCount_Test";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_tag").objectReferenceValue = tag;
            serializedObject.FindProperty("_requiredQuantity").intValue = requiredQuantity;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public TotalPowerRequirementDefinition CreateTotalPowerRequirement(
            string displayName,
            int requiredPower)
        {
            var definition = Track(ScriptableObject.CreateInstance<TotalPowerRequirementDefinition>());
            definition.name = "Requirement_TotalPower_Test";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_requiredPower").intValue = requiredPower;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public UniqueItemCountRequirementDefinition CreateUniqueItemCountRequirement(
            string displayName,
            int requiredUniqueItemCount)
        {
            var definition = Track(
                ScriptableObject.CreateInstance<UniqueItemCountRequirementDefinition>());
            definition.name = "Requirement_UniqueItemCount_Test";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_requiredUniqueItemCount").intValue =
                requiredUniqueItemCount;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public GuildQuestDefinition CreateQuest(
            string id,
            string displayName,
            string description,
            int rewardCoins,
            int rewardReputation,
            params QuestRequirementDefinition[] requirements)
        {
            var definition = Track(ScriptableObject.CreateInstance<GuildQuestDefinition>());
            definition.name = $"Quest_{id}";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_id").stringValue = id;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_description").stringValue = description;
            serializedObject.FindProperty("_rewardCoins").intValue = rewardCoins;
            serializedObject.FindProperty("_rewardReputation").intValue = rewardReputation;
            SetObjectArray(serializedObject.FindProperty("_requirements"), requirements);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public GuildQuestCatalogAsset CreateQuestCatalog(
            ItemCatalogAsset itemCatalog,
            params GuildQuestDefinition[] quests)
        {
            var catalog = Track(ScriptableObject.CreateInstance<GuildQuestCatalogAsset>());
            catalog.name = "GuildQuestCatalog_Test";

            var serializedObject = new SerializedObject(catalog);
            serializedObject.FindProperty("_itemCatalog").objectReferenceValue = itemCatalog;
            SetObjectArray(serializedObject.FindProperty("_questDefinitions"), quests);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }

        public GameSessionConfigAsset CreateGameSessionConfig(
            ItemCatalogAsset itemCatalog,
            GuildQuestCatalogAsset questCatalog,
            int startingCoins,
            int dailyCoinStipend,
            int visitorsPerDay,
            int offersPerVisitor,
            params RarityUnlockThreshold[] rarityThresholds)
        {
            if (rarityThresholds == null)
            {
                throw new ArgumentNullException(nameof(rarityThresholds));
            }

            var config = Track(ScriptableObject.CreateInstance<GameSessionConfigAsset>());
            config.name = "GameSessionConfig_Test";

            var serializedObject = new SerializedObject(config);
            serializedObject.FindProperty("_itemCatalog").objectReferenceValue = itemCatalog;
            serializedObject.FindProperty("_questCatalog").objectReferenceValue = questCatalog;
            serializedObject.FindProperty("_startingCoins").intValue = startingCoins;
            serializedObject.FindProperty("_dailyCoinStipend").intValue = dailyCoinStipend;
            serializedObject.FindProperty("_visitorsPerDay").intValue = visitorsPerDay;
            serializedObject.FindProperty("_offersPerVisitor").intValue = offersPerVisitor;

            var thresholdsProperty = serializedObject.FindProperty("_rarityThresholds");
            thresholdsProperty.arraySize = rarityThresholds.Length;
            for (var index = 0; index < rarityThresholds.Length; index++)
            {
                var thresholdProperty = thresholdsProperty.GetArrayElementAtIndex(index);
                thresholdProperty.FindPropertyRelative("_rarity").intValue =
                    (int)rarityThresholds[index].Rarity;
                thresholdProperty.FindPropertyRelative("_requiredReputation").intValue =
                    rarityThresholds[index].RequiredReputation;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return config;
        }

        public Sprite CreateIcon()
        {
            var texture = Track(new Texture2D(1, 1, TextureFormat.RGBA32, false));
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Track(Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f)));
        }

        public void Dispose()
        {
            for (var index = _createdObjects.Count - 1; index >= 0; index--)
            {
                if (_createdObjects[index] != null)
                {
                    Object.DestroyImmediate(_createdObjects[index]);
                }
            }

            _createdObjects.Clear();
        }

        private T Track<T>(T instance) where T : Object
        {
            _createdObjects.Add(instance);
            return instance;
        }

        private static void SetObjectArray<T>(SerializedProperty property, IReadOnlyList<T> values)
            where T : Object
        {
            property.arraySize = values.Count;
            for (var index = 0; index < values.Count; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }
    }
}
