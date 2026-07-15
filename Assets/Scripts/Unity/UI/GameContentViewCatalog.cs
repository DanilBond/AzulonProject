using System;
using System.Collections.Generic;
using Azulon.Configuration.Game;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Domain.Items;
using Azulon.Domain.Quests;
using UnityEngine;

namespace Azulon.Unity.UI
{
    public sealed class GameContentViewCatalog
    {
        private readonly Dictionary<ItemId, ItemDefinition> _itemsById;
        private readonly Dictionary<QuestId, GuildQuestDefinition> _questsById;

        public GameContentViewCatalog(GameSessionConfigAsset configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.ItemCatalog == null)
            {
                throw new InvalidOperationException("Game session has no item catalog assigned.");
            }

            if (configuration.QuestCatalog == null)
            {
                throw new InvalidOperationException("Game session has no quest catalog assigned.");
            }

            _itemsById = BuildItemLookup(configuration.ItemCatalog.ItemDefinitions);
            _questsById = BuildQuestLookup(configuration.QuestCatalog.QuestDefinitions);
        }

        public Sprite GetItemIcon(ItemId itemId)
        {
            if (!_itemsById.TryGetValue(itemId, out var item))
            {
                throw new KeyNotFoundException($"No visual data exists for item '{itemId}'.");
            }

            return item.Icon;
        }

        public string GetRequirementDisplayName(QuestId questId, int requirementIndex)
        {
            if (!_questsById.TryGetValue(questId, out var quest))
            {
                throw new KeyNotFoundException($"No visual data exists for quest '{questId}'.");
            }

            if (requirementIndex < 0 || requirementIndex >= quest.Requirements.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requirementIndex),
                    $"Quest '{questId}' has no requirement at index {requirementIndex}.");
            }

            return quest.Requirements[requirementIndex].DisplayName;
        }

        private static Dictionary<ItemId, ItemDefinition> BuildItemLookup(
            IReadOnlyList<ItemDefinition> items)
        {
            var lookup = new Dictionary<ItemId, ItemDefinition>();
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Item visual catalog contains a missing definition.");
                }

                var itemId = new ItemId(item.Id);
                if (lookup.ContainsKey(itemId))
                {
                    throw new InvalidOperationException($"Item visual catalog contains duplicate ID '{itemId}'.");
                }

                lookup.Add(itemId, item);
            }

            return lookup;
        }

        private static Dictionary<QuestId, GuildQuestDefinition> BuildQuestLookup(
            IReadOnlyList<GuildQuestDefinition> quests)
        {
            var lookup = new Dictionary<QuestId, GuildQuestDefinition>();
            foreach (var quest in quests)
            {
                if (quest == null)
                {
                    throw new InvalidOperationException("Quest visual catalog contains a missing definition.");
                }

                var questId = new QuestId(quest.Id);
                if (lookup.ContainsKey(questId))
                {
                    throw new InvalidOperationException($"Quest visual catalog contains duplicate ID '{questId}'.");
                }

                lookup.Add(questId, quest);
            }

            return lookup;
        }
    }
}
