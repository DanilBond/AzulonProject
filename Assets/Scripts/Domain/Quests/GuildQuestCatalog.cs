using System;
using System.Collections.Generic;

namespace Azulon.Domain.Quests
{
    public sealed class GuildQuestCatalog
    {
        private readonly IReadOnlyList<GuildQuest> _quests;
        private readonly Dictionary<QuestId, GuildQuest> _questsById;

        public GuildQuestCatalog(IEnumerable<GuildQuest> quests)
        {
            if (quests == null)
            {
                throw new ArgumentNullException(nameof(quests));
            }

            var questList = new List<GuildQuest>();
            _questsById = new Dictionary<QuestId, GuildQuest>();
            foreach (var quest in quests)
            {
                if (quest == null)
                {
                    throw new ArgumentException("Quest catalog cannot contain null.", nameof(quests));
                }

                if (_questsById.ContainsKey(quest.Id))
                {
                    throw new ArgumentException($"Quest catalog contains duplicate ID '{quest.Id}'.", nameof(quests));
                }

                _questsById.Add(quest.Id, quest);
                questList.Add(quest);
            }

            if (questList.Count == 0)
            {
                throw new ArgumentException("Quest catalog must contain at least one quest.", nameof(quests));
            }

            _quests = questList.AsReadOnly();
        }

        public IReadOnlyList<GuildQuest> Quests => _quests;

        public bool TryGetQuest(QuestId questId, out GuildQuest quest)
        {
            return _questsById.TryGetValue(questId, out quest);
        }
    }
}
