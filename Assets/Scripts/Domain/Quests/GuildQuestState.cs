using System;

namespace Azulon.Domain.Quests
{
    public sealed class GuildQuestState
    {
        public GuildQuestState(GuildQuest quest)
        {
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
        }

        public GuildQuest Quest { get; }

        public bool IsClaimed { get; private set; }

        internal void MarkClaimed()
        {
            if (IsClaimed)
            {
                throw new InvalidOperationException($"Quest '{Quest.Id}' has already been claimed.");
            }

            IsClaimed = true;
        }
    }
}
