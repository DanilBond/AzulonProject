using System;
using Azulon.Application.Gameplay;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Application.Gameplay
{
    public sealed class GameSessionSettingsTests
    {
        [Test]
        public void Constructor_WithValidValues_CopiesProgressionRules()
        {
            var source = new[]
            {
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 2)
            };

            var settings = new GameSessionSettings(5, 3, 2, 4, source);

            Assert.That(settings.StartingCoins, Is.EqualTo(5));
            Assert.That(settings.DailyCoinStipend, Is.EqualTo(3));
            Assert.That(settings.VisitorsPerDay, Is.EqualTo(2));
            Assert.That(settings.OffersPerVisitor, Is.EqualTo(4));
            Assert.That(settings.RarityThresholds.Count, Is.EqualTo(2));
            Assert.That(settings.RarityThresholds[1].RequiredReputation, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WithoutDailyStipend_RejectsSoftLockProneRules()
        {
            Assert.That(
                () => new GameSessionSettings(
                    5,
                    0,
                    2,
                    3,
                    CreateCommonThreshold()),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_WithNonContiguousRarities_RejectsInvalidProgression()
        {
            Assert.That(
                () => new GameSessionSettings(
                    5,
                    2,
                    2,
                    3,
                    new[]
                    {
                        new RarityUnlockThreshold(ItemRarity.Common, 0),
                        new RarityUnlockThreshold(ItemRarity.Rare, 3)
                    }),
                Throws.TypeOf<ArgumentException>());
        }

        private static RarityUnlockThreshold[] CreateCommonThreshold()
        {
            return new[]
            {
                new RarityUnlockThreshold(ItemRarity.Common, 0)
            };
        }
    }
}
