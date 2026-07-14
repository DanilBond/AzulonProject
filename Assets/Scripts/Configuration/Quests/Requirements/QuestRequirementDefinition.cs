using Azulon.Domain.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests.Requirements
{
    public abstract class QuestRequirementDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;

        public string DisplayName => _displayName;

        public abstract IQuestRequirement CreateRequirement();

        internal void Validate(QuestRequirementValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(_displayName))
            {
                context.AddError(this, "has no display name");
            }

            ValidateFields(context);
        }

        protected abstract void ValidateFields(QuestRequirementValidationContext context);
    }
}
