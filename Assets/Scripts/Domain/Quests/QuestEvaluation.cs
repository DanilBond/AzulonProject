using System;
using System.Collections.Generic;
using Azulon.Domain.Quests.Requirements;

namespace Azulon.Domain.Quests
{
    public sealed class QuestEvaluation
    {
        private readonly IReadOnlyList<QuestRequirementProgress> _requirementProgress;

        public QuestEvaluation(
            GuildQuest quest,
            IEnumerable<QuestRequirementProgress> requirementProgress)
        {
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
            if (requirementProgress == null)
            {
                throw new ArgumentNullException(nameof(requirementProgress));
            }

            var progressList = new List<QuestRequirementProgress>(requirementProgress);
            if (progressList.Count != quest.Requirements.Count)
            {
                throw new ArgumentException(
                    "Quest progress count must match the number of requirements.",
                    nameof(requirementProgress));
            }

            _requirementProgress = progressList.AsReadOnly();
            IsCompleted = true;
            foreach (var progress in _requirementProgress)
            {
                if (!progress.IsSatisfied)
                {
                    IsCompleted = false;
                    break;
                }
            }
        }

        public GuildQuest Quest { get; }

        public IReadOnlyList<QuestRequirementProgress> RequirementProgress => _requirementProgress;

        public bool IsCompleted { get; }
    }
}
