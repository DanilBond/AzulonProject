using System;
using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Items.Validation;
using Azulon.Configuration.Quests.Requirements;
using Azulon.Configuration.Validation;
using Azulon.Domain.Quests;

namespace Azulon.Configuration.Quests.Validation
{
    public static class GuildQuestCatalogValidator
    {
        public static GuildQuestCatalogValidationResult Validate(GuildQuestCatalogAsset catalog)
        {
            var issues = new List<CatalogValidationIssue>();
            if (catalog == null)
            {
                AddError(issues, "Guild quest catalog is missing.");
                return new GuildQuestCatalogValidationResult(issues);
            }

            var registeredItems = new HashSet<ItemDefinition>();
            var registeredTags = new HashSet<ItemTagDefinition>();
            ValidateItemCatalog(catalog.ItemCatalog, registeredItems, registeredTags, issues);
            ValidateQuests(catalog.QuestDefinitions, registeredItems, registeredTags, issues);
            return new GuildQuestCatalogValidationResult(issues);
        }

        private static void ValidateItemCatalog(
            ItemCatalogAsset itemCatalog,
            ISet<ItemDefinition> registeredItems,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            if (itemCatalog == null)
            {
                AddError(issues, "Guild quest catalog has no item catalog assigned.");
                return;
            }

            if (!ItemCatalogValidator.Validate(itemCatalog).IsValid)
            {
                AddError(issues, "Guild quest catalog references an invalid item catalog.");
            }

            foreach (var item in itemCatalog.ItemDefinitions)
            {
                if (item != null)
                {
                    registeredItems.Add(item);
                }
            }

            foreach (var tag in itemCatalog.TagDefinitions)
            {
                if (tag != null)
                {
                    registeredTags.Add(tag);
                }
            }
        }

        private static void ValidateQuests(
            IReadOnlyList<GuildQuestDefinition> quests,
            ISet<ItemDefinition> registeredItems,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            if (quests.Count == 0)
            {
                AddError(issues, "Guild quest catalog must contain at least one quest.");
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < quests.Count; index++)
            {
                var quest = quests[index];
                if (quest == null)
                {
                    AddError(issues, $"Quest entry at index {index} is missing.");
                    continue;
                }

                ValidateQuest(quest, ids, registeredItems, registeredTags, issues);
            }
        }

        private static void ValidateQuest(
            GuildQuestDefinition quest,
            ISet<string> ids,
            ISet<ItemDefinition> registeredItems,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            if (!QuestId.TryCreate(quest.Id, out _))
            {
                AddError(issues, $"Quest '{quest.name}' has invalid ID '{quest.Id}'.");
            }
            else if (!ids.Add(quest.Id))
            {
                AddError(issues, $"Duplicate quest ID '{quest.Id}'.");
            }

            if (string.IsNullOrWhiteSpace(quest.DisplayName))
            {
                AddError(issues, $"Quest '{quest.name}' has no display name.");
            }

            if (string.IsNullOrWhiteSpace(quest.Description))
            {
                AddError(issues, $"Quest '{quest.name}' has no description.");
            }

            if (quest.RewardCoins < 0 || quest.RewardReputation < 0)
            {
                AddError(issues, $"Quest '{quest.name}' cannot have negative rewards.");
            }
            else if (quest.RewardCoins == 0 && quest.RewardReputation == 0)
            {
                AddError(issues, $"Quest '{quest.name}' must have a coin or reputation reward.");
            }

            ValidateRequirements(quest, registeredItems, registeredTags, issues);
        }

        private static void ValidateRequirements(
            GuildQuestDefinition quest,
            ISet<ItemDefinition> registeredItems,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            if (quest.Requirements.Count == 0)
            {
                AddError(issues, $"Quest '{quest.name}' must contain at least one requirement.");
                return;
            }

            var uniqueRequirements = new HashSet<QuestRequirementDefinition>();
            var context = new QuestRequirementValidationContext(
                quest.name,
                registeredItems,
                registeredTags,
                issues);

            for (var index = 0; index < quest.Requirements.Count; index++)
            {
                var requirement = quest.Requirements[index];
                if (requirement == null)
                {
                    AddError(issues, $"Quest '{quest.name}' has a missing requirement at index {index}.");
                    continue;
                }

                if (!uniqueRequirements.Add(requirement))
                {
                    AddError(issues, $"Quest '{quest.name}' contains requirement '{requirement.name}' more than once.");
                }

                requirement.Validate(context);
            }
        }

        private static void AddError(ICollection<CatalogValidationIssue> issues, string message)
        {
            issues.Add(new CatalogValidationIssue(CatalogValidationSeverity.Error, message));
        }
    }
}
