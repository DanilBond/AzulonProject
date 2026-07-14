using System;
using System.Collections.Generic;
using Azulon.Configuration.Quests.Validation;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;

namespace Azulon.Configuration.Quests
{
    public static class GuildQuestCatalogMapper
    {
        public static GuildQuestCatalog ToDomain(GuildQuestCatalogAsset catalogAsset)
        {
            var validation = GuildQuestCatalogValidator.Validate(catalogAsset);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    $"Cannot build guild quest catalog because its configuration is invalid:{Environment.NewLine}{validation.FormatErrors()}");
            }

            var quests = new List<GuildQuest>(catalogAsset.QuestDefinitions.Count);
            foreach (var questDefinition in catalogAsset.QuestDefinitions)
            {
                var requirements = new List<IQuestRequirement>(questDefinition.Requirements.Count);
                foreach (var requirementDefinition in questDefinition.Requirements)
                {
                    requirements.Add(requirementDefinition.CreateRequirement());
                }

                quests.Add(new GuildQuest(
                    new QuestId(questDefinition.Id),
                    questDefinition.DisplayName,
                    questDefinition.Description,
                    questDefinition.RewardCoins,
                    questDefinition.RewardReputation,
                    requirements));
            }

            return new GuildQuestCatalog(quests);
        }
    }
}
