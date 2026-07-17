using System;
using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Items.Validation;
using Azulon.Configuration.Quests.Validation;
using Azulon.Configuration.Validation;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;

namespace Azulon.Configuration.Game.Validation
{
    public static class GameSessionConfigValidator
    {
        public static GameSessionConfigValidationResult Validate(GameSessionConfigAsset config)
        {
            var issues = new List<CatalogValidationIssue>();
            if (config == null)
            {
                AddError(issues, "Game session configuration is missing.");
                return new GameSessionConfigValidationResult(issues);
            }

            ValidateItemCatalog(config.ItemCatalog, issues);
            ValidateQuestCatalog(config, issues);
            ValidateVisitorSprites(config, issues);
            ValidateBalance(config, issues);
            var progression = ValidateProgression(config.RarityThresholds, issues);
            ValidateMarketReachability(config.ItemCatalog, progression, issues);
            return new GameSessionConfigValidationResult(issues);
        }

        private static void ValidateItemCatalog(
            ItemCatalogAsset itemCatalog,
            ICollection<CatalogValidationIssue> issues)
        {
            var validation = ItemCatalogValidator.Validate(itemCatalog);
            AddNestedErrors("Item catalog", validation.Issues, issues);

            if (itemCatalog == null)
            {
                return;
            }

            foreach (var item in itemCatalog.ItemDefinitions)
            {
                if (item != null && item.Rarity == ItemRarity.Common)
                {
                    return;
                }
            }

            AddError(
                issues,
                "Item catalog must contain at least one Common item so the first visitor can be generated.");
        }

        private static void ValidateQuestCatalog(
            GameSessionConfigAsset config,
            ICollection<CatalogValidationIssue> issues)
        {
            var validation = GuildQuestCatalogValidator.Validate(config.QuestCatalog);
            AddNestedErrors("Quest catalog", validation.Issues, issues);

            if (config.ItemCatalog != null &&
                config.QuestCatalog != null &&
                config.QuestCatalog.ItemCatalog != config.ItemCatalog)
            {
                AddError(
                    issues,
                    "Game session and quest catalog must reference the same item catalog asset.");
            }
        }

        private static void ValidateBalance(
            GameSessionConfigAsset config,
            ICollection<CatalogValidationIssue> issues)
        {
            if (config.StartingCoins < 0)
            {
                AddError(issues, "Starting coins cannot be negative.");
            }

            if (config.DailyCoinStipend <= 0)
            {
                AddError(
                    issues,
                    "Daily coin stipend must be greater than zero to prevent a permanent soft lock.");
            }

            if (config.VisitorsPerDay <= 0)
            {
                AddError(issues, "Visitors per day must be greater than zero.");
            }
        }

        private static void ValidateVisitorSprites(
            GameSessionConfigAsset config,
            ICollection<CatalogValidationIssue> issues)
        {
            if (config.VisitorSprites == null || config.VisitorSprites.Count < 2)
            {
                AddError(
                    issues,
                    "At least two visitor sprites are required so consecutive visitors can look different.");
                return;
            }

            var uniqueSprites = new HashSet<UnityEngine.Sprite>();
            for (var index = 0; index < config.VisitorSprites.Count; index++)
            {
                var sprite = config.VisitorSprites[index];
                if (sprite == null)
                {
                    AddError(issues, $"Visitor sprite at index {index} is missing.");
                    continue;
                }

                if (!uniqueSprites.Add(sprite))
                {
                    AddError(issues, $"Visitor sprite at index {index} is duplicated.");
                }
            }
        }

        private static GuildProgression ValidateProgression(
            IReadOnlyList<RarityUnlockThresholdConfig> thresholdConfigs,
            ICollection<CatalogValidationIssue> issues)
        {
            if (thresholdConfigs.Count == 0)
            {
                AddError(issues, "At least the Common rarity threshold is required.");
                return null;
            }

            var thresholds = new List<RarityUnlockThreshold>(thresholdConfigs.Count);
            var entriesAreValid = true;
            for (var index = 0; index < thresholdConfigs.Count; index++)
            {
                var threshold = thresholdConfigs[index];
                if (threshold == null)
                {
                    AddError(issues, $"Rarity threshold at index {index} is missing.");
                    entriesAreValid = false;
                    continue;
                }

                if (!Enum.IsDefined(typeof(ItemRarity), threshold.Rarity))
                {
                    AddError(issues, $"Rarity threshold at index {index} has an unknown rarity.");
                    entriesAreValid = false;
                    continue;
                }

                if (threshold.RequiredReputation < 0)
                {
                    AddError(
                        issues,
                        $"Rarity '{threshold.Rarity}' cannot require negative reputation.");
                    entriesAreValid = false;
                    continue;
                }

                thresholds.Add(new RarityUnlockThreshold(
                    threshold.Rarity,
                    threshold.RequiredReputation));
            }

            if (!entriesAreValid)
            {
                return null;
            }

            try
            {
                return new GuildProgression(thresholds);
            }
            catch (ArgumentException exception)
            {
                AddError(issues, $"Rarity progression is invalid: {exception.Message}");
                return null;
            }
        }

        private static void ValidateMarketReachability(
            ItemCatalogAsset itemCatalog,
            GuildProgression progression,
            ICollection<CatalogValidationIssue> issues)
        {
            if (itemCatalog == null || progression == null)
            {
                return;
            }

            var highestConfiguredRarity =
                progression.Thresholds[progression.Thresholds.Count - 1].Rarity;
            foreach (var item in itemCatalog.ItemDefinitions)
            {
                if (item != null && (int)item.Rarity > (int)highestConfiguredRarity)
                {
                    AddError(
                        issues,
                        $"Item '{item.name}' can never appear because rarity '{item.Rarity}' has no unlock threshold.");
                }
            }
        }

        private static void AddNestedErrors(
            string prefix,
            IReadOnlyList<CatalogValidationIssue> nestedIssues,
            ICollection<CatalogValidationIssue> issues)
        {
            foreach (var issue in nestedIssues)
            {
                if (issue.Severity == CatalogValidationSeverity.Error)
                {
                    AddError(issues, $"{prefix}: {issue.Message}");
                }
            }
        }

        private static void AddError(
            ICollection<CatalogValidationIssue> issues,
            string message)
        {
            issues.Add(new CatalogValidationIssue(CatalogValidationSeverity.Error, message));
        }
    }
}
