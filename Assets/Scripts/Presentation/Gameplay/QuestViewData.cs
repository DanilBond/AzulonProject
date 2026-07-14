using System;
using System.Collections.Generic;
using Azulon.Domain.Quests;

namespace Azulon.Presentation.Gameplay
{
    public sealed class QuestViewData
    {
        private readonly IReadOnlyList<QuestRequirementViewData> _requirements;

        public QuestViewData(
            QuestId id,
            string displayName,
            string description,
            int rewardCoins,
            int rewardReputation,
            bool isClaimed,
            bool isCompleted,
            IEnumerable<QuestRequirementViewData> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            Id = id;
            DisplayName = displayName;
            Description = description;
            RewardCoins = rewardCoins;
            RewardReputation = rewardReputation;
            IsClaimed = isClaimed;
            IsCompleted = isCompleted;
            _requirements = new List<QuestRequirementViewData>(requirements).AsReadOnly();
        }

        public QuestId Id { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int RewardCoins { get; }

        public int RewardReputation { get; }

        public bool IsClaimed { get; }

        public bool IsCompleted { get; }

        public bool CanClaim => IsCompleted && !IsClaimed;

        public IReadOnlyList<QuestRequirementViewData> Requirements => _requirements;
    }
}
