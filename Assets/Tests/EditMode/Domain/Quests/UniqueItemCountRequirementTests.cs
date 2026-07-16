using Azulon.Domain.Inventory;
using Azulon.Domain.Quests.Requirements;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Quests
{
    public sealed class UniqueItemCountRequirementTests
    {
        [Test]
        public void Evaluate_CountsDifferentItemsInsteadOfTotalQuantity()
        {
            var context = new QuestTestContext();
            var inventory = new PlayerInventory();
            inventory.Add(context.EmberBlade.Id, 3);
            inventory.Add(context.FlameOrb.Id);
            var requirement = new UniqueItemCountRequirement(2);

            var progress = requirement.Evaluate(
                new InventoryQuery(inventory, context.Catalog));

            Assert.That(progress.Current, Is.EqualTo(2));
            Assert.That(progress.Required, Is.EqualTo(2));
            Assert.That(progress.IsSatisfied, Is.True);
        }

        [Test]
        public void Constructor_WithNonPositiveCount_Throws()
        {
            Assert.That(
                () => new UniqueItemCountRequirement(0),
                Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }
    }
}
