using System;
using System.Collections.Generic;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Quests.Requirements;

namespace Azulon.Domain.Quests
{
    public sealed class QuestEvaluator
    {
        public QuestEvaluation Evaluate(
            GuildQuest quest,
            PlayerInventory inventory,
            ItemCatalog catalog)
        {
            if (quest == null)
            {
                throw new ArgumentNullException(nameof(quest));
            }

            var query = new InventoryQuery(inventory, catalog);
            var progress = new List<QuestRequirementProgress>(quest.Requirements.Count);
            foreach (var requirement in quest.Requirements)
            {
                progress.Add(requirement.Evaluate(query));
            }

            return new QuestEvaluation(quest, progress);
        }
    }
}
