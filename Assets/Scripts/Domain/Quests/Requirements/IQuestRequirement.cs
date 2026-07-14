using Azulon.Domain.Inventory;

namespace Azulon.Domain.Quests.Requirements
{
    public interface IQuestRequirement
    {
        QuestRequirementProgress Evaluate(InventoryQuery inventory);
    }
}
