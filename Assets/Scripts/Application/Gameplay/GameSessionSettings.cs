using System;
using System.Collections.Generic;
using Azulon.Domain.Progression;

namespace Azulon.Application.Gameplay
{
    public sealed class GameSessionSettings
    {
        private readonly IReadOnlyList<RarityUnlockThreshold> _rarityThresholds;

        public GameSessionSettings(
            int startingCoins,
            int dailyCoinStipend,
            int visitorsPerDay,
            IEnumerable<RarityUnlockThreshold> rarityThresholds)
        {
            if (startingCoins < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startingCoins),
                    "Starting coins cannot be negative.");
            }

            if (dailyCoinStipend <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dailyCoinStipend),
                    "Daily coin stipend must be greater than zero to prevent a permanent soft lock.");
            }

            if (visitorsPerDay <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(visitorsPerDay),
                    "Visitors per day must be greater than zero.");
            }

            if (rarityThresholds == null)
            {
                throw new ArgumentNullException(nameof(rarityThresholds));
            }

            var validatedProgression = new GuildProgression(rarityThresholds);
            _rarityThresholds = new List<RarityUnlockThreshold>(validatedProgression.Thresholds)
                .AsReadOnly();

            StartingCoins = startingCoins;
            DailyCoinStipend = dailyCoinStipend;
            VisitorsPerDay = visitorsPerDay;
        }

        public int StartingCoins { get; }

        public int DailyCoinStipend { get; }

        public int VisitorsPerDay { get; }

        public IReadOnlyList<RarityUnlockThreshold> RarityThresholds => _rarityThresholds;

        internal GuildProgression CreateProgression()
        {
            return new GuildProgression(_rarityThresholds);
        }
    }
}
