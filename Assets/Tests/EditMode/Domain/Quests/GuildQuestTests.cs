using System;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Quests
{
    public sealed class GuildQuestTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Invalid Quest")]
        public void QuestId_WithInvalidValue_IsRejected(string value)
        {
            Assert.That(QuestId.TryCreate(value, out _), Is.False);
        }

        [Test]
        public void Constructor_WithoutRewardOrRequirements_Throws()
        {
            var requirement = new TotalPowerRequirement(1);

            Assert.That(
                () => new GuildQuest(
                    new QuestId("invalid_quest"),
                    "Invalid Quest",
                    "A deliberately invalid quest.",
                    0,
                    0,
                    new[] { requirement }),
                Throws.ArgumentException);
            Assert.That(
                () => new GuildQuest(
                    new QuestId("invalid_quest"),
                    "Invalid Quest",
                    "A deliberately invalid quest.",
                    1,
                    0,
                    new IQuestRequirement[0]),
                Throws.ArgumentException);
        }

        [Test]
        public void QuestCatalog_WithDuplicateId_Throws()
        {
            var context = new QuestTestContext();
            var first = context.CreateQuest(new TotalPowerRequirement(1));
            var second = context.CreateQuest(new TotalPowerRequirement(2));

            Assert.That(
                () => new GuildQuestCatalog(new[] { first, second }),
                Throws.TypeOf<ArgumentException>());
        }
    }
}
