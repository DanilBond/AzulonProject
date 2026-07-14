using System;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using Azulon.Tests.EditMode.Domain.Quests;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Progression
{
    public sealed class GuildProgressionTests
    {
        [Test]
        public void AddReputation_UnlocksRaritiesAtConfiguredThresholds()
        {
            var progression = new QuestTestContext().CreateProgression();

            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Common));

            progression.AddReputation(1);
            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Uncommon));

            progression.AddReputation(2);
            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Rare));

            progression.AddReputation(3);
            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Epic));
        }

        [Test]
        public void Constructor_WithInitialReputation_ResolvesUnlockedRarity()
        {
            var progression = new QuestTestContext().CreateProgression(10);

            Assert.That(progression.Reputation, Is.EqualTo(10));
            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Legendary));
        }

        [Test]
        public void Constructor_WithGapOrNonIncreasingThreshold_Throws()
        {
            Assert.That(
                () => new GuildProgression(new[]
                {
                    new RarityUnlockThreshold(ItemRarity.Common, 0),
                    new RarityUnlockThreshold(ItemRarity.Rare, 1)
                }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(
                () => new GuildProgression(new[]
                {
                    new RarityUnlockThreshold(ItemRarity.Common, 0),
                    new RarityUnlockThreshold(ItemRarity.Uncommon, 0)
                }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void AddReputation_WhenValueWouldOverflow_DoesNotMutateProgression()
        {
            var progression = new QuestTestContext().CreateProgression(int.MaxValue);

            Assert.That(() => progression.AddReputation(1), Throws.TypeOf<OverflowException>());
            Assert.That(progression.Reputation, Is.EqualTo(int.MaxValue));
            Assert.That(progression.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Legendary));
        }
    }
}
