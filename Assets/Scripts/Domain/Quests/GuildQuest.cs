using System;
using System.Collections.Generic;
using Azulon.Domain.Quests.Requirements;

namespace Azulon.Domain.Quests
{
    public sealed class GuildQuest
    {
        private readonly IReadOnlyList<IQuestRequirement> _requirements;

        public GuildQuest(
            QuestId id,
            string displayName,
            string description,
            int rewardCoins,
            int rewardReputation,
            IEnumerable<IQuestRequirement> requirements)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("Quest ID cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Quest display name cannot be empty.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Quest description cannot be empty.", nameof(description));
            }

            if (rewardCoins < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rewardCoins));
            }

            if (rewardReputation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rewardReputation));
            }

            if (rewardCoins == 0 && rewardReputation == 0)
            {
                throw new ArgumentException("Quest must reward coins, reputation, or both.");
            }

            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            var requirementList = new List<IQuestRequirement>();
            foreach (var requirement in requirements)
            {
                if (requirement == null)
                {
                    throw new ArgumentException("Quest requirements cannot contain null.", nameof(requirements));
                }

                requirementList.Add(requirement);
            }

            if (requirementList.Count == 0)
            {
                throw new ArgumentException("Quest must contain at least one requirement.", nameof(requirements));
            }

            Id = id;
            DisplayName = displayName;
            Description = description;
            RewardCoins = rewardCoins;
            RewardReputation = rewardReputation;
            _requirements = requirementList.AsReadOnly();
        }

        public QuestId Id { get; }

        public string DisplayName { get; }

        public string Description { get; }

        public int RewardCoins { get; }

        public int RewardReputation { get; }

        public IReadOnlyList<IQuestRequirement> Requirements => _requirements;
    }
}
