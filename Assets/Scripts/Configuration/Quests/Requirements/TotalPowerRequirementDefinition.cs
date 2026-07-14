using Azulon.Domain.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests.Requirements
{
    [CreateAssetMenu(
        fileName = "Requirement_TotalPower",
        menuName = "Guild Relic Market/Quests/Requirements/Total Power",
        order = 130)]
    public sealed class TotalPowerRequirementDefinition : QuestRequirementDefinition
    {
        [SerializeField, Min(1)] private int _requiredPower = 1;

        public int RequiredPower => _requiredPower;

        public override IQuestRequirement CreateRequirement()
        {
            return new TotalPowerRequirement(_requiredPower);
        }

        protected override void ValidateFields(QuestRequirementValidationContext context)
        {
            if (_requiredPower <= 0)
            {
                context.AddError(this, "must require power greater than zero");
            }
        }
    }
}
