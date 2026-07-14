using System;

namespace Azulon.Presentation.Gameplay
{
    public sealed class QuestRequirementViewData
    {
        public QuestRequirementViewData(int index, int current, int required)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (current < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(current));
            }

            if (required <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(required));
            }

            Index = index;
            Current = current;
            Required = required;
        }

        public int Index { get; }

        public int Current { get; }

        public int Required { get; }

        public bool IsSatisfied => Current >= Required;
    }
}
