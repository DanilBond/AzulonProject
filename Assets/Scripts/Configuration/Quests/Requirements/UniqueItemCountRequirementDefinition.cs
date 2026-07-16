using Azulon.Domain.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests.Requirements
{
    [CreateAssetMenu(
        fileName = "Requirement_UniqueItems",
        menuName = "Guild Relic Market/Quests/Requirements/Unique Item Count",
        order = 135)]
    public sealed class UniqueItemCountRequirementDefinition : QuestRequirementDefinition
    {
        [SerializeField, Min(1)] private int _requiredUniqueItemCount = 1;

        public int RequiredUniqueItemCount => _requiredUniqueItemCount;

        public override IQuestRequirement CreateRequirement()
        {
            return new UniqueItemCountRequirement(_requiredUniqueItemCount);
        }

        protected override void ValidateFields(QuestRequirementValidationContext context)
        {
            if (_requiredUniqueItemCount <= 0)
            {
                context.AddError(this, "must require at least one unique item");
            }
            else if (_requiredUniqueItemCount > context.RegisteredItemCount)
            {
                context.AddError(
                    this,
                    $"requires {_requiredUniqueItemCount} unique items, but the catalog contains only {context.RegisteredItemCount}");
            }
        }
    }
}
