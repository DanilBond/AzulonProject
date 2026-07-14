using System;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;
using Azulon.Domain.Items;
using Azulon.Domain.Quests;
using Azulon.Domain.Quests.Requirements;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Quests
{
    public sealed class QuestClaimServiceTests
    {
        private QuestTestContext _context;
        private QuestClaimService _service;

        [SetUp]
        public void SetUp()
        {
            _context = new QuestTestContext();
            _service = new QuestClaimService(new QuestEvaluator());
        }

        [Test]
        public void Claim_WhenRequirementsAreMissing_DoesNotMutateProgression()
        {
            var quest = _context.CreateQuest(
                new ExactItemRequirement(_context.EmberBlade.Id, 1));
            var state = new GuildQuestState(quest);
            var wallet = new Wallet(2);
            var progression = _context.CreateProgression();

            var result = _service.Claim(
                state,
                new PlayerInventory(),
                _context.Catalog,
                wallet,
                progression);

            Assert.That(result.Status, Is.EqualTo(QuestClaimStatus.RequirementsNotMet));
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.AwardedCoins, Is.Zero);
            Assert.That(wallet.Balance, Is.EqualTo(2));
            Assert.That(progression.Reputation, Is.Zero);
            Assert.That(state.IsClaimed, Is.False);
        }

        [Test]
        public void Claim_WhenCompleted_AwardsProgressionWithoutConsumingItems()
        {
            var inventory = new PlayerInventory();
            inventory.Add(_context.EmberBlade.Id);
            var quest = _context.CreateQuest(
                new ExactItemRequirement(_context.EmberBlade.Id, 1));
            var state = new GuildQuestState(quest);
            var wallet = new Wallet(2);
            var progression = _context.CreateProgression();

            var result = _service.Claim(
                state,
                inventory,
                _context.Catalog,
                wallet,
                progression);

            Assert.That(result.Status, Is.EqualTo(QuestClaimStatus.Success));
            Assert.That(result.AwardedCoins, Is.EqualTo(6));
            Assert.That(result.AwardedReputation, Is.EqualTo(1));
            Assert.That(result.WalletBalance, Is.EqualTo(8));
            Assert.That(result.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Uncommon));
            Assert.That(inventory.GetQuantity(_context.EmberBlade.Id), Is.EqualTo(1));
            Assert.That(state.IsClaimed, Is.True);
        }

        [Test]
        public void Claim_WhenCalledTwice_DoesNotAwardTwice()
        {
            var inventory = new PlayerInventory();
            inventory.Add(_context.EmberBlade.Id);
            var state = new GuildQuestState(_context.CreateQuest(
                new ExactItemRequirement(_context.EmberBlade.Id, 1)));
            var wallet = new Wallet(2);
            var progression = _context.CreateProgression();
            _service.Claim(state, inventory, _context.Catalog, wallet, progression);

            var second = _service.Claim(state, inventory, _context.Catalog, wallet, progression);

            Assert.That(second.Status, Is.EqualTo(QuestClaimStatus.AlreadyClaimed));
            Assert.That(second.AwardedCoins, Is.Zero);
            Assert.That(second.AwardedReputation, Is.Zero);
            Assert.That(wallet.Balance, Is.EqualTo(8));
            Assert.That(progression.Reputation, Is.EqualTo(1));
        }

        [Test]
        public void Claim_WhenRewardWouldOverflow_LeavesAllStateUnchanged()
        {
            var inventory = new PlayerInventory();
            inventory.Add(_context.EmberBlade.Id);
            var quest = _context.CreateQuest(
                1,
                1,
                new ExactItemRequirement(_context.EmberBlade.Id, 1));
            var state = new GuildQuestState(quest);
            var wallet = new Wallet(int.MaxValue);
            var progression = _context.CreateProgression();

            Assert.That(
                () => _service.Claim(
                    state,
                    inventory,
                    _context.Catalog,
                    wallet,
                    progression),
                Throws.TypeOf<OverflowException>());
            Assert.That(wallet.Balance, Is.EqualTo(int.MaxValue));
            Assert.That(progression.Reputation, Is.Zero);
            Assert.That(state.IsClaimed, Is.False);
        }
    }
}
