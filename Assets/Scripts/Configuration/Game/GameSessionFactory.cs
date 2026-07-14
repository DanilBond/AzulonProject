using System;
using Azulon.Application.Gameplay;
using Azulon.Configuration.Items;
using Azulon.Configuration.Quests;
using Azulon.Domain.Market;
using Azulon.Domain.Randomness;

namespace Azulon.Configuration.Game
{
    public static class GameSessionFactory
    {
        public static GameSession Create(
            GameSessionConfigAsset configAsset,
            IRandomSource randomSource,
            IMarketOfferIdSource offerIdSource)
        {
            if (randomSource == null)
            {
                throw new ArgumentNullException(nameof(randomSource));
            }

            if (offerIdSource == null)
            {
                throw new ArgumentNullException(nameof(offerIdSource));
            }

            var settings = GameSessionConfigMapper.ToSettings(configAsset);
            var itemCatalog = ItemCatalogMapper.ToDomain(configAsset.ItemCatalog);
            var questCatalog = GuildQuestCatalogMapper.ToDomain(configAsset.QuestCatalog);
            return new GameSession(
                settings,
                itemCatalog,
                questCatalog,
                randomSource,
                offerIdSource);
        }
    }
}
