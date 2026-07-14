using System;

namespace Azulon.Domain.Quests.Requirements
{
    public readonly struct QuestRequirementProgress
    {
        public QuestRequirementProgress(int current, int required)
        {
            if (current < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(current));
            }

            if (required <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(required));
            }

            Current = current;
            Required = required;
        }

        public int Current { get; }

        public int Required { get; }

        public bool IsSatisfied => Current >= Required;
    }
}
