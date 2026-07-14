using System.Collections.Generic;
using Azulon.Configuration.Quests.Requirements;
using UnityEngine;

namespace Azulon.Configuration.Quests
{
    [CreateAssetMenu(
        fileName = "Quest_New",
        menuName = "Guild Relic Market/Quests/Quest Definition",
        order = 140)]
    public sealed class GuildQuestDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(2, 4)] private string _description;
        [SerializeField, Min(0)] private int _rewardCoins = 3;
        [SerializeField, Min(0)] private int _rewardReputation = 1;
        [SerializeField] private List<QuestRequirementDefinition> _requirements =
            new List<QuestRequirementDefinition>();

        public string Id => _id;

        public string DisplayName => _displayName;

        public string Description => _description;

        public int RewardCoins => _rewardCoins;

        public int RewardReputation => _rewardReputation;

        public IReadOnlyList<QuestRequirementDefinition> Requirements => _requirements;
    }
}
