using System;
using Azulon.Domain.Inventory;

namespace Azulon.Domain.Quests.Requirements
{
    public sealed class TotalPowerRequirement : IQuestRequirement
    {
        public TotalPowerRequirement(int requiredPower)
        {
            if (requiredPower <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredPower));
            }

            RequiredPower = requiredPower;
        }

        public int RequiredPower { get; }

        public QuestRequirementProgress Evaluate(InventoryQuery inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            return new QuestRequirementProgress(
                inventory.CalculateTotalPower(),
                RequiredPower);
        }
    }
}
