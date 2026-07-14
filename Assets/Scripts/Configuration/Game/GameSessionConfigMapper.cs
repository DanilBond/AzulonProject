using System;
using System.Collections.Generic;
using Azulon.Application.Gameplay;
using Azulon.Configuration.Game.Validation;
using Azulon.Domain.Progression;

namespace Azulon.Configuration.Game
{
    public static class GameSessionConfigMapper
    {
        public static GameSessionSettings ToSettings(GameSessionConfigAsset configAsset)
        {
            var validation = GameSessionConfigValidator.Validate(configAsset);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    $"Cannot build game session settings because the configuration is invalid:{Environment.NewLine}{validation.FormatErrors()}");
            }

            var thresholds = new List<RarityUnlockThreshold>(configAsset.RarityThresholds.Count);
            foreach (var threshold in configAsset.RarityThresholds)
            {
                thresholds.Add(new RarityUnlockThreshold(
                    threshold.Rarity,
                    threshold.RequiredReputation));
            }

            return new GameSessionSettings(
                configAsset.StartingCoins,
                configAsset.DailyCoinStipend,
                configAsset.VisitorsPerDay,
                configAsset.OffersPerVisitor,
                thresholds);
        }
    }
}
