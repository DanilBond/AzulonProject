using System;
using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Configuration.Quests.Requirements;
using Azulon.Domain.Items;
using Azulon.Domain.Quests;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed class ContentAssetCreationService
    {
        private readonly ContentAssetFolders _folders;

        public ContentAssetCreationService(ContentAssetFolders folders = null)
        {
            _folders = folders ?? ContentAssetFolders.Default;
        }

        public ItemTagDefinition CreateTag(
            ItemCatalogAsset catalog,
            string id,
            string displayName,
            Color displayColor)
        {
            RequireCatalog(catalog);
            if (!ItemTagId.TryCreate(id, out _))
            {
                throw new ArgumentException(
                    "Tag ID must use lowercase letters, digits, and underscores.",
                    nameof(id));
            }

            RequireText(displayName, "Tag display name");
            EnsureUniqueTagId(catalog, id);

            var definition = ScriptableObject.CreateInstance<ItemTagDefinition>();
            definition.name = $"Tag_{ContentAuthoringNameUtility.ToAssetSuffix(id)}";
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_id").stringValue = id;
            serializedDefinition.FindProperty("_displayName").stringValue = displayName.Trim();
            serializedDefinition.FindProperty("_displayColor").colorValue = displayColor;
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();

            ContentEditorAssetUtility.CreateAsset(
                definition,
                _folders.Tags,
                definition.name);
            ContentEditorAssetUtility.AppendObjectReference(
                catalog,
                "_tagDefinitions",
                definition,
                "Register Item Tag");
            AssetDatabase.SaveAssets();
            return definition;
        }

        public ItemDefinition CreateItem(
            ItemCatalogAsset catalog,
            string id,
            string displayName,
            string description,
            Sprite icon,
            int price,
            int power,
            ItemRarity rarity,
            ItemCategory category,
            IReadOnlyList<ItemTagDefinition> tags)
        {
            RequireCatalog(catalog);
            if (!ItemId.TryCreate(id, out _))
            {
                throw new ArgumentException(
                    "Item ID must use lowercase letters, digits, and underscores.",
                    nameof(id));
            }

            RequireText(displayName, "Item display name");
            RequireText(description, "Item description");
            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon), "Item icon is required.");
            }

            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "Item price must be positive.");
            }

            if (power < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(power), "Item power cannot be negative.");
            }

            if (!Enum.IsDefined(typeof(ItemRarity), rarity))
            {
                throw new ArgumentOutOfRangeException(nameof(rarity));
            }

            if (!Enum.IsDefined(typeof(ItemCategory), category))
            {
                throw new ArgumentOutOfRangeException(nameof(category));
            }

            ValidateItemTags(catalog, tags);
            EnsureUniqueItemId(catalog, id);

            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            definition.name = $"Item_{ContentAuthoringNameUtility.ToAssetSuffix(id)}";
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_id").stringValue = id;
            serializedDefinition.FindProperty("_displayName").stringValue = displayName.Trim();
            serializedDefinition.FindProperty("_description").stringValue = description.Trim();
            serializedDefinition.FindProperty("_icon").objectReferenceValue = icon;
            serializedDefinition.FindProperty("_price").intValue = price;
            serializedDefinition.FindProperty("_power").intValue = power;
            serializedDefinition.FindProperty("_rarity").intValue = (int)rarity;
            serializedDefinition.FindProperty("_category").intValue = (int)category;
            ContentEditorAssetUtility.SetObjectReferences(
                serializedDefinition.FindProperty("_tags"),
                tags);
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();

            ContentEditorAssetUtility.CreateAsset(
                definition,
                _folders.Items,
                definition.name);
            ContentEditorAssetUtility.AppendObjectReference(
                catalog,
                "_itemDefinitions",
                definition,
                "Register Item");
            AssetDatabase.SaveAssets();
            return definition;
        }

        public ExactItemRequirementDefinition CreateExactItemRequirement(
            ItemCatalogAsset catalog,
            string displayName,
            ItemDefinition item,
            int requiredQuantity)
        {
            RequireCatalog(catalog);
            RequireText(displayName, "Requirement display name");
            if (item == null || !ContainsReference(catalog.ItemDefinitions, item))
            {
                throw new ArgumentException(
                    "Exact item must be registered in the selected item catalog.",
                    nameof(item));
            }

            RequirePositive(requiredQuantity, nameof(requiredQuantity));
            var definition = ScriptableObject.CreateInstance<ExactItemRequirementDefinition>();
            SetRequirementBase(definition, displayName);
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_item").objectReferenceValue = item;
            serializedDefinition.FindProperty("_requiredQuantity").intValue = requiredQuantity;
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
            return CreateRequirementAsset(definition, "ExactItem", displayName);
        }

        public TagCountRequirementDefinition CreateTagCountRequirement(
            ItemCatalogAsset catalog,
            string displayName,
            ItemTagDefinition tag,
            int requiredQuantity)
        {
            RequireCatalog(catalog);
            RequireText(displayName, "Requirement display name");
            if (tag == null || !ContainsReference(catalog.TagDefinitions, tag))
            {
                throw new ArgumentException(
                    "Tag must be registered in the selected item catalog.",
                    nameof(tag));
            }

            RequirePositive(requiredQuantity, nameof(requiredQuantity));
            var definition = ScriptableObject.CreateInstance<TagCountRequirementDefinition>();
            SetRequirementBase(definition, displayName);
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_tag").objectReferenceValue = tag;
            serializedDefinition.FindProperty("_requiredQuantity").intValue = requiredQuantity;
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
            return CreateRequirementAsset(definition, "TagCount", displayName);
        }

        public TotalPowerRequirementDefinition CreateTotalPowerRequirement(
            string displayName,
            int requiredPower)
        {
            RequireText(displayName, "Requirement display name");
            RequirePositive(requiredPower, nameof(requiredPower));
            var definition = ScriptableObject.CreateInstance<TotalPowerRequirementDefinition>();
            SetRequirementBase(definition, displayName);
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_requiredPower").intValue = requiredPower;
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
            return CreateRequirementAsset(definition, "TotalPower", displayName);
        }

        public UniqueItemCountRequirementDefinition CreateUniqueItemCountRequirement(
            ItemCatalogAsset catalog,
            string displayName,
            int requiredUniqueItemCount)
        {
            RequireCatalog(catalog);
            RequireText(displayName, "Requirement display name");
            RequirePositive(requiredUniqueItemCount, nameof(requiredUniqueItemCount));
            if (requiredUniqueItemCount > catalog.ItemDefinitions.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredUniqueItemCount),
                    $"Unique item count cannot exceed the {catalog.ItemDefinitions.Count} catalog items.");
            }

            var definition = ScriptableObject.CreateInstance<UniqueItemCountRequirementDefinition>();
            SetRequirementBase(definition, displayName);
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_requiredUniqueItemCount").intValue =
                requiredUniqueItemCount;
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
            return CreateRequirementAsset(definition, "UniqueItems", displayName);
        }

        public GuildQuestDefinition CreateQuest(
            GuildQuestCatalogAsset catalog,
            string id,
            string displayName,
            string description,
            int rewardCoins,
            int rewardReputation,
            IReadOnlyList<QuestRequirementDefinition> requirements)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (!QuestId.TryCreate(id, out _))
            {
                throw new ArgumentException(
                    "Quest ID must use lowercase letters, digits, and underscores.",
                    nameof(id));
            }

            RequireText(displayName, "Quest display name");
            RequireText(description, "Quest description");
            if (rewardCoins < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rewardCoins),
                    "Quest rewards cannot be negative.");
            }

            if (rewardReputation < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rewardReputation),
                    "Quest rewards cannot be negative.");
            }

            if (rewardCoins == 0 && rewardReputation == 0)
            {
                throw new ArgumentException("Quest must have at least one reward.");
            }

            ValidateRequirements(catalog, requirements);
            EnsureUniqueQuestId(catalog, id);

            var definition = ScriptableObject.CreateInstance<GuildQuestDefinition>();
            definition.name = $"Quest_{ContentAuthoringNameUtility.ToAssetSuffix(id)}";
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_id").stringValue = id;
            serializedDefinition.FindProperty("_displayName").stringValue = displayName.Trim();
            serializedDefinition.FindProperty("_description").stringValue = description.Trim();
            serializedDefinition.FindProperty("_rewardCoins").intValue = rewardCoins;
            serializedDefinition.FindProperty("_rewardReputation").intValue = rewardReputation;
            ContentEditorAssetUtility.SetObjectReferences(
                serializedDefinition.FindProperty("_requirements"),
                requirements);
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();

            ContentEditorAssetUtility.CreateAsset(
                definition,
                _folders.Quests,
                definition.name);
            ContentEditorAssetUtility.AppendObjectReference(
                catalog,
                "_questDefinitions",
                definition,
                "Register Guild Quest");
            AssetDatabase.SaveAssets();
            return definition;
        }

        private T CreateRequirementAsset<T>(
            T definition,
            string typeName,
            string displayName)
            where T : QuestRequirementDefinition
        {
            definition.name =
                $"Requirement_{typeName}_{ContentAuthoringNameUtility.ToAssetSuffix(displayName)}";
            ContentEditorAssetUtility.CreateAsset(
                definition,
                _folders.Requirements,
                definition.name);
            AssetDatabase.SaveAssets();
            return definition;
        }

        private static void SetRequirementBase(
            QuestRequirementDefinition definition,
            string displayName)
        {
            var serializedDefinition = new SerializedObject(definition);
            serializedDefinition.FindProperty("_displayName").stringValue = displayName.Trim();
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ValidateItemTags(
            ItemCatalogAsset catalog,
            IReadOnlyList<ItemTagDefinition> tags)
        {
            if (tags == null || tags.Count == 0)
            {
                throw new ArgumentException("Item must have at least one tag.", nameof(tags));
            }

            var uniqueTags = new HashSet<ItemTagDefinition>();
            foreach (var tag in tags)
            {
                if (tag == null || !ContainsReference(catalog.TagDefinitions, tag))
                {
                    throw new ArgumentException(
                        "Every item tag must be registered in the selected catalog.",
                        nameof(tags));
                }

                if (!uniqueTags.Add(tag))
                {
                    throw new ArgumentException("Item tags must be unique.", nameof(tags));
                }
            }
        }

        private static void ValidateRequirements(
            GuildQuestCatalogAsset catalog,
            IReadOnlyList<QuestRequirementDefinition> requirements)
        {
            if (requirements == null || requirements.Count == 0)
            {
                throw new ArgumentException(
                    "Quest must have at least one requirement.",
                    nameof(requirements));
            }

            var uniqueRequirements = new HashSet<QuestRequirementDefinition>();
            foreach (var requirement in requirements)
            {
                if (requirement == null)
                {
                    throw new ArgumentException(
                        "Quest requirements cannot contain missing assets.",
                        nameof(requirements));
                }

                if (!uniqueRequirements.Add(requirement))
                {
                    throw new ArgumentException(
                        "Quest cannot contain the same requirement asset twice.",
                        nameof(requirements));
                }

                ValidateRequirementCompatibility(catalog, requirement);
            }
        }

        private static void ValidateRequirementCompatibility(
            GuildQuestCatalogAsset catalog,
            QuestRequirementDefinition requirement)
        {
            RequireText(requirement.DisplayName, "Requirement display name");
            if (catalog.ItemCatalog == null)
            {
                throw new InvalidOperationException(
                    "Quest catalog must reference an item catalog before quests can be created.");
            }

            if (requirement is ExactItemRequirementDefinition exactItem)
            {
                if (exactItem.Item == null ||
                    !ContainsReference(catalog.ItemCatalog.ItemDefinitions, exactItem.Item))
                {
                    throw new ArgumentException(
                        $"Requirement '{requirement.name}' references an unregistered item.");
                }

                RequirePositive(exactItem.RequiredQuantity, nameof(exactItem.RequiredQuantity));
            }
            else if (requirement is TagCountRequirementDefinition tagCount)
            {
                if (tagCount.Tag == null ||
                    !ContainsReference(catalog.ItemCatalog.TagDefinitions, tagCount.Tag))
                {
                    throw new ArgumentException(
                        $"Requirement '{requirement.name}' references an unregistered tag.");
                }

                RequirePositive(tagCount.RequiredQuantity, nameof(tagCount.RequiredQuantity));
            }
            else if (requirement is TotalPowerRequirementDefinition totalPower)
            {
                RequirePositive(totalPower.RequiredPower, nameof(totalPower.RequiredPower));
            }
            else if (requirement is UniqueItemCountRequirementDefinition uniqueItems)
            {
                RequirePositive(
                    uniqueItems.RequiredUniqueItemCount,
                    nameof(uniqueItems.RequiredUniqueItemCount));
                if (uniqueItems.RequiredUniqueItemCount >
                    catalog.ItemCatalog.ItemDefinitions.Count)
                {
                    throw new ArgumentException(
                        $"Requirement '{requirement.name}' exceeds the item catalog size.");
                }
            }
        }

        private static bool ContainsReference<T>(IReadOnlyList<T> values, T target)
            where T : UnityEngine.Object
        {
            foreach (var value in values)
            {
                if (value == target)
                {
                    return true;
                }
            }

            return false;
        }

        private static void EnsureUniqueTagId(ItemCatalogAsset catalog, string id)
        {
            foreach (var tag in catalog.TagDefinitions)
            {
                if (tag != null && string.Equals(tag.Id, id, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"Tag ID '{id}' already exists.");
                }
            }
        }

        private static void EnsureUniqueItemId(ItemCatalogAsset catalog, string id)
        {
            foreach (var item in catalog.ItemDefinitions)
            {
                if (item != null && string.Equals(item.Id, id, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"Item ID '{id}' already exists.");
                }
            }
        }

        private static void EnsureUniqueQuestId(GuildQuestCatalogAsset catalog, string id)
        {
            foreach (var quest in catalog.QuestDefinitions)
            {
                if (quest != null && string.Equals(quest.Id, id, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"Quest ID '{id}' already exists.");
                }
            }
        }

        private static void RequireCatalog(ItemCatalogAsset catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
        }

        private static void RequireText(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{fieldName} cannot be empty.");
            }
        }

        private static void RequirePositive(int value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    "Value must be greater than zero.");
            }
        }
    }
}
