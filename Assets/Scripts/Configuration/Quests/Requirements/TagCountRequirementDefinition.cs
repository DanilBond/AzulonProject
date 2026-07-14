using Azulon.Configuration.Items;
using Azulon.Domain.Items;
using Azulon.Domain.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests.Requirements
{
    [CreateAssetMenu(
        fileName = "Requirement_TagCount",
        menuName = "Guild Relic Market/Quests/Requirements/Tag Count",
        order = 120)]
    public sealed class TagCountRequirementDefinition : QuestRequirementDefinition
    {
        [SerializeField] private ItemTagDefinition _tag;
        [SerializeField, Min(1)] private int _requiredQuantity = 1;

        public ItemTagDefinition Tag => _tag;

        public int RequiredQuantity => _requiredQuantity;

        public override IQuestRequirement CreateRequirement()
        {
            return new TagCountRequirement(new ItemTagId(_tag.Id), _requiredQuantity);
        }

        protected override void ValidateFields(QuestRequirementValidationContext context)
        {
            context.RequireRegisteredTag(this, _tag);
            if (_requiredQuantity <= 0)
            {
                context.AddError(this, "must require at least one tagged item");
            }
        }
    }
}
