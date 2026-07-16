using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Configuration.Validation;

namespace Azulon.Configuration.Quests.Requirements
{
    public sealed class QuestRequirementValidationContext
    {
        private readonly string _questName;
        private readonly ISet<ItemDefinition> _registeredItems;
        private readonly ISet<ItemTagDefinition> _registeredTags;
        private readonly ICollection<CatalogValidationIssue> _issues;

        internal QuestRequirementValidationContext(
            string questName,
            ISet<ItemDefinition> registeredItems,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            _questName = questName;
            _registeredItems = registeredItems;
            _registeredTags = registeredTags;
            _issues = issues;
        }

        public int RegisteredItemCount => _registeredItems.Count;

        public void RequireRegisteredItem(
            QuestRequirementDefinition requirement,
            ItemDefinition item)
        {
            if (item == null)
            {
                AddError(requirement, "has no item assigned");
            }
            else if (!_registeredItems.Contains(item))
            {
                AddError(requirement, $"uses item '{item.name}' that is not registered in the item catalog");
            }
        }

        public void RequireRegisteredTag(
            QuestRequirementDefinition requirement,
            ItemTagDefinition tag)
        {
            if (tag == null)
            {
                AddError(requirement, "has no tag assigned");
            }
            else if (!_registeredTags.Contains(tag))
            {
                AddError(requirement, $"uses tag '{tag.name}' that is not registered in the item catalog");
            }
        }

        public void AddError(QuestRequirementDefinition requirement, string message)
        {
            _issues.Add(new CatalogValidationIssue(
                CatalogValidationSeverity.Error,
                $"Requirement '{requirement.name}' in quest '{_questName}' {message}."));
        }
    }
}
