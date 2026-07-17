using System;
using System.Collections.Generic;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;
using Azulon.Domain.Randomness;

namespace Azulon.Application.Gameplay
{
    public sealed class GameSession
    {
        private readonly GameSessionSettings _settings;
        private readonly ItemCatalog _itemCatalog;
        private readonly Wallet _wallet;
        private readonly PlayerInventory _inventory;
        private readonly GuildProgression _progression;
        private readonly MarketOfferGenerator _offerGenerator;
        private readonly PurchaseService _purchaseService;
        private readonly QuestEvaluator _questEvaluator;
        private readonly QuestClaimService _questClaimService;
        private readonly IReadOnlyList<GuildQuestState> _questStates;
        private readonly Dictionary<QuestId, GuildQuestState> _questStatesById;

        public GameSession(
            GameSessionSettings settings,
            ItemCatalog itemCatalog,
            GuildQuestCatalog questCatalog,
            IRandomSource randomSource,
            IMarketOfferIdSource offerIdSource)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _itemCatalog = itemCatalog ?? throw new ArgumentNullException(nameof(itemCatalog));
            if (questCatalog == null)
            {
                throw new ArgumentNullException(nameof(questCatalog));
            }

            _wallet = new Wallet(settings.StartingCoins);
            _inventory = new PlayerInventory();
            _progression = settings.CreateProgression();
            _offerGenerator = new MarketOfferGenerator(randomSource, offerIdSource);
            _purchaseService = new PurchaseService();
            _questEvaluator = new QuestEvaluator();
            _questClaimService = new QuestClaimService(_questEvaluator);

            var questStates = new List<GuildQuestState>(questCatalog.Quests.Count);
            _questStatesById = new Dictionary<QuestId, GuildQuestState>();
            foreach (var quest in questCatalog.Quests)
            {
                var questState = new GuildQuestState(quest);
                questStates.Add(questState);
                _questStatesById.Add(quest.Id, questState);
            }

            _questStates = questStates.AsReadOnly();
            DayNumber = 1;
            VisitorNumber = 1;
            CurrentOffer = GenerateOffer();
        }

        public GameSessionSettings Settings => _settings;

        public ItemCatalog ItemCatalog => _itemCatalog;

        public IReadOnlyList<GuildQuestState> QuestStates => _questStates;

        public MarketOffer CurrentOffer { get; private set; }

        public int DayNumber { get; private set; }

        public int VisitorNumber { get; private set; }

        public int Coins => _wallet.Balance;

        public int Reputation => _progression.Reputation;

        public ItemRarity MaximumUnlockedRarity => _progression.MaximumUnlockedRarity;

        public int TotalOwnedItemCount => _inventory.TotalItemCount;

        public int UniqueOwnedItemCount => _inventory.UniqueItemCount;

        public bool IsCompleted
        {
            get
            {
                foreach (var questState in _questStates)
                {
                    if (!questState.IsClaimed)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public IReadOnlyList<InventoryEntry> CreateInventorySnapshot()
        {
            return _inventory.CreateSnapshot();
        }

        public bool TryGetQuestState(QuestId questId, out GuildQuestState questState)
        {
            return _questStatesById.TryGetValue(questId, out questState);
        }

        public PurchaseResult PurchaseOffer(MarketOfferId offerId)
        {
            EnsureSessionIsActive();
            if (offerId.IsEmpty)
            {
                throw new ArgumentException("Market offer ID cannot be empty.", nameof(offerId));
            }

            if (CurrentOffer.Id != offerId)
            {
                throw new KeyNotFoundException(
                    $"Current visitor does not have market offer '{offerId}'.");
            }

            return _purchaseService.Purchase(CurrentOffer, _wallet, _inventory);
        }

        public QuestEvaluation EvaluateQuest(QuestId questId)
        {
            var questState = GetQuestState(questId);
            return _questEvaluator.Evaluate(questState.Quest, _inventory, _itemCatalog);
        }

        public IReadOnlyList<QuestEvaluation> EvaluateQuests()
        {
            var evaluations = new List<QuestEvaluation>(_questStates.Count);
            foreach (var questState in _questStates)
            {
                evaluations.Add(_questEvaluator.Evaluate(
                    questState.Quest,
                    _inventory,
                    _itemCatalog));
            }

            return evaluations.AsReadOnly();
        }

        public QuestClaimResult ClaimQuest(QuestId questId)
        {
            var questState = GetQuestState(questId);
            return _questClaimService.Claim(
                questState,
                _inventory,
                _itemCatalog,
                _wallet,
                _progression);
        }

        public VisitorAdvanceResult AdvanceToNextVisitor()
        {
            EnsureSessionIsActive();

            var startsNewDay = VisitorNumber == _settings.VisitorsPerDay;
            var nextDayNumber = startsNewDay ? checked(DayNumber + 1) : DayNumber;
            var nextVisitorNumber = startsNewDay ? 1 : VisitorNumber + 1;
            if (startsNewDay &&
                (long)_wallet.Balance + _settings.DailyCoinStipend > int.MaxValue)
            {
                throw new OverflowException("Daily stipend exceeds the supported wallet range.");
            }

            var nextOffer = GenerateOffer();
            var creditedCoins = 0;
            if (startsNewDay)
            {
                creditedCoins = _settings.DailyCoinStipend;
                _wallet.Credit(creditedCoins);
            }

            DayNumber = nextDayNumber;
            VisitorNumber = nextVisitorNumber;
            CurrentOffer = nextOffer;

            return new VisitorAdvanceResult(
                startsNewDay,
                DayNumber,
                VisitorNumber,
                creditedCoins,
                _wallet.Balance);
        }

        private MarketOffer GenerateOffer()
        {
            return _offerGenerator.Generate(
                _itemCatalog,
                _progression.MaximumUnlockedRarity);
        }

        private GuildQuestState GetQuestState(QuestId questId)
        {
            if (questId.IsEmpty)
            {
                throw new ArgumentException("Quest ID cannot be empty.", nameof(questId));
            }

            if (!_questStatesById.TryGetValue(questId, out var questState))
            {
                throw new KeyNotFoundException($"Game session does not contain quest '{questId}'.");
            }

            return questState;
        }

        private void EnsureSessionIsActive()
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("The game session is already completed.");
            }
        }
    }
}
