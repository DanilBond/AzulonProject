using Azulon.Domain.Inventory;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Quests
{
    public sealed class QuestEvaluatorTests
    {
        [Test]
        public void Evaluate_WhenEveryRequirementIsMet_CompletesAndSynergy()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id);
            inventory.Add(context.FlameOrb.Id);
            var quest = context.CreateQuest(
                new ExactItemRequirement(context.EmberBlade.Id, 1),
                new TagCountRequirement(context.FireTagId, 2),
                new TotalPowerRequirement(12));

            var evaluation = new QuestEvaluator().Evaluate(quest, inventory, context.Catalog);

            Assert.That(evaluation.IsCompleted, Is.True);
            Assert.That(evaluation.RequirementProgress[0].Current, Is.EqualTo(1));
            Assert.That(evaluation.RequirementProgress[1].Current, Is.EqualTo(2));
            Assert.That(evaluation.RequirementProgress[2].Current, Is.EqualTo(12));
        }

        [Test]
        public void Evaluate_WhenOneRequirementIsMissing_DoesNotCompleteAndGroup()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id);
            var quest = context.CreateQuest(
                new ExactItemRequirement(context.EmberBlade.Id, 1),
                new TagCountRequirement(context.FireTagId, 2),
                new TotalPowerRequirement(7));

            var evaluation = new QuestEvaluator().Evaluate(quest, inventory, context.Catalog);

            Assert.That(evaluation.IsCompleted, Is.False);
            Assert.That(evaluation.RequirementProgress[0].IsSatisfied, Is.True);
            Assert.That(evaluation.RequirementProgress[1].IsSatisfied, Is.False);
            Assert.That(evaluation.RequirementProgress[2].IsSatisfied, Is.True);
        }

        [Test]
        public void Evaluate_ReportsActualProgressAboveRequirement()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id, 3);
            var quest = context.CreateQuest(
                new TagCountRequirement(context.FireTagId, 2),
                new TotalPowerRequirement(10));

            var evaluation = new QuestEvaluator().Evaluate(quest, inventory, context.Catalog);

            Assert.That(evaluation.RequirementProgress[0].Current, Is.EqualTo(3));
            Assert.That(evaluation.RequirementProgress[1].Current, Is.EqualTo(21));
            Assert.That(evaluation.IsCompleted, Is.True);
        }

        [Test]
        public void Evaluate_ExactItemAbsentFromInventory_ReturnsZeroProgress()
        {
            var context = new QuestTestContext();
            var quest = context.CreateQuest(
                new ExactItemRequirement(context.EmberBlade.Id, 1));

            var evaluation = new QuestEvaluator().Evaluate(
                quest,
                new PlayerInventory(),
                context.Catalog);

            Assert.That(evaluation.RequirementProgress[0].Current, Is.Zero);
            Assert.That(evaluation.IsCompleted, Is.False);
        }

        [Test]
        public void Evaluate_WithCustomRequirement_RequiresNoEvaluatorChanges()
        {
            var context = new QuestTestContext();
            var quest = context.CreateQuest(new AlwaysSatisfiedRequirement());

            var evaluation = new QuestEvaluator().Evaluate(
                quest,
                new PlayerInventory(),
                context.Catalog);

            Assert.That(evaluation.IsCompleted, Is.True);
        }

        private sealed class AlwaysSatisfiedRequirement : IQuestRequirement
        {
            public QuestRequirementProgress Evaluate(InventoryQuery inventory)
            {
                return new QuestRequirementProgress(1, 1);
            }
        }
    }
}
