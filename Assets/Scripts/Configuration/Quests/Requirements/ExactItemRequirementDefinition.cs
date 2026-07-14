using Azulon.Configuration.Items;
using Azulon.Domain.Items;
using Azulon.Domain.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests.Requirements
{
    [CreateAssetMenu(
        fileName = "Requirement_ExactItem",
        menuName = "Guild Relic Market/Quests/Requirements/Exact Item",
        order = 110)]
    public sealed class ExactItemRequirementDefinition : QuestRequirementDefinition
    {
        [SerializeField] private ItemDefinition _item;
        [SerializeField, Min(1)] private int _requiredQuantity = 1;

        public ItemDefinition Item => _item;

        public int RequiredQuantity => _requiredQuantity;

        public override IQuestRequirement CreateRequirement()
        {
            return new ExactItemRequirement(new ItemId(_item.Id), _requiredQuantity);
        }

        protected override void ValidateFields(QuestRequirementValidationContext context)
        {
            context.RequireRegisteredItem(this, _item);
            if (_requiredQuantity <= 0)
            {
                context.AddError(this, "must require at least one item");
            }
        }
    }
}
