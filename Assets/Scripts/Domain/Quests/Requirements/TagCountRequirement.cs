using System;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;

namespace Azulon.Domain.Quests.Requirements
{
    public sealed class TagCountRequirement : IQuestRequirement
    {
        public TagCountRequirement(ItemTagId tagId, int requiredQuantity)
        {
            if (tagId.IsEmpty)
            {
                throw new ArgumentException("Required tag ID cannot be empty.", nameof(tagId));
            }

            if (requiredQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredQuantity));
            }

            TagId = tagId;
            RequiredQuantity = requiredQuantity;
        }

        public ItemTagId TagId { get; }

        public int RequiredQuantity { get; }

        public QuestRequirementProgress Evaluate(InventoryQuery inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            return new QuestRequirementProgress(
                inventory.CountItemsWithTag(TagId),
                RequiredQuantity);
        }
    }
}
