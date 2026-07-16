using System;
using Azulon.Domain.Inventory;

namespace Azulon.Domain.Quests.Requirements
{
    public sealed class UniqueItemCountRequirement : IQuestRequirement
    {
        public UniqueItemCountRequirement(int requiredUniqueItemCount)
        {
            if (requiredUniqueItemCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredUniqueItemCount));
            }

            RequiredUniqueItemCount = requiredUniqueItemCount;
        }

        public int RequiredUniqueItemCount { get; }

        public QuestRequirementProgress Evaluate(InventoryQuery inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            return new QuestRequirementProgress(
                inventory.CountUniqueItems(),
                RequiredUniqueItemCount);
        }
    }
}
