using System;
using System.Collections.Generic;
using Azulon.Configuration.Game;
using Azulon.Configuration.Game.Validation;
using Azulon.Configuration.Items.Validation;
using Azulon.Configuration.Quests.Validation;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed class ContentValidationReport
    {
        internal ContentValidationReport(
            GameSessionConfigAsset configuration,
            ItemCatalogValidationResult itemCatalog,
            GuildQuestCatalogValidationResult questCatalog,
            GameSessionConfigValidationResult session)
        {
            Configuration = configuration;
            ItemCatalog = itemCatalog;
            QuestCatalog = questCatalog;
            Session = session;
        }

        public GameSessionConfigAsset Configuration { get; }

        public ItemCatalogValidationResult ItemCatalog { get; }

        public GuildQuestCatalogValidationResult QuestCatalog { get; }

        public GameSessionConfigValidationResult Session { get; }

        public bool IsValid =>
            ItemCatalog.IsValid && QuestCatalog.IsValid && Session.IsValid;
    }

    public static class ContentValidationService
    {
        public static ContentValidationReport Validate(
            GameSessionConfigAsset configuration)
        {
            return new ContentValidationReport(
                configuration,
                ItemCatalogValidator.Validate(configuration == null
                    ? null
                    : configuration.ItemCatalog),
                GuildQuestCatalogValidator.Validate(configuration == null
                    ? null
                    : configuration.QuestCatalog),
                GameSessionConfigValidator.Validate(configuration));
        }

        public static ContentValidationReport ValidateAndLog(
            GameSessionConfigAsset configuration,
            bool logSuccessfulValidation = true)
        {
            var report = Validate(configuration);
            if (report.IsValid)
            {
                if (logSuccessfulValidation)
                {
                    Debug.Log(BuildSuccessMessage(configuration), configuration);
                }

                return report;
            }

            Debug.LogError(BuildFailureMessage(report), configuration);
            return report;
        }

        public static IReadOnlyList<ContentValidationReport> ValidateAllProjectConfigurations(
            bool logSuccessfulValidation = true)
        {
            var configurations = FindProjectConfigurations();
            var reports = new List<ContentValidationReport>(configurations.Count);
            if (configurations.Count == 0)
            {
                Debug.LogWarning("Guild Relic Market has no game session configuration to validate.");
                return reports.AsReadOnly();
            }

            foreach (var configuration in configurations)
            {
                reports.Add(ValidateAndLog(configuration, logSuccessfulValidation));
            }

            return reports.AsReadOnly();
        }

        public static IReadOnlyList<GameSessionConfigAsset> FindProjectConfigurations()
        {
            var guids = AssetDatabase.FindAssets("t:GameSessionConfigAsset");
            var paths = new List<string>(guids.Length);
            foreach (var guid in guids)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            paths.Sort(StringComparer.Ordinal);
            var configurations = new List<GameSessionConfigAsset>(paths.Count);
            foreach (var path in paths)
            {
                var configuration =
                    AssetDatabase.LoadAssetAtPath<GameSessionConfigAsset>(path);
                if (configuration != null)
                {
                    configurations.Add(configuration);
                }
            }

            return configurations.AsReadOnly();
        }

        private static string BuildSuccessMessage(GameSessionConfigAsset configuration)
        {
            if (configuration == null)
            {
                return "Guild Relic Market content validation passed.";
            }

            var itemCatalog = configuration.ItemCatalog;
            var questCatalog = configuration.QuestCatalog;
            return $"Guild Relic Market content is valid: " +
                   $"{itemCatalog.TagDefinitions.Count} tag(s), " +
                   $"{itemCatalog.ItemDefinitions.Count} item(s), " +
                   $"{questCatalog.QuestDefinitions.Count} quest(s), " +
                   $"{configuration.VisitorSprites.Count} visitor sprite(s).";
        }

        private static string BuildFailureMessage(ContentValidationReport report)
        {
            var errors = report.Session.FormatErrors();
            return string.IsNullOrWhiteSpace(errors)
                ? "Guild Relic Market content validation failed."
                : $"Guild Relic Market content validation failed:{Environment.NewLine}{errors}";
        }
    }
}
