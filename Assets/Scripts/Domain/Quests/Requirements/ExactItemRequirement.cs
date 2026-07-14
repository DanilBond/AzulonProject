using System;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;

namespace Azulon.Domain.Quests.Requirements
{
    public sealed class ExactItemRequirement : IQuestRequirement
    {
        public ExactItemRequirement(ItemId itemId, int requiredQuantity)
        {
            if (itemId.IsEmpty)
            {
                throw new ArgumentException("Required item ID cannot be empty.", nameof(itemId));
            }

            if (requiredQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredQuantity));
            }

            ItemId = itemId;
            RequiredQuantity = requiredQuantity;
        }

        public ItemId ItemId { get; }

        public int RequiredQuantity { get; }

        public QuestRequirementProgress Evaluate(InventoryQuery inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            return new QuestRequirementProgress(
                inventory.GetItemQuantity(ItemId),
                RequiredQuantity);
        }
    }
}
