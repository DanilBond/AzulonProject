using System;
using Azulon.Configuration.Game;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;
using Azulon.Domain.Randomness;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class GameSessionConfigMapperTests
    {
        private ScriptableObjectTestFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new ScriptableObjectTestFactory();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public void ToSettings_WithValidConfiguration_MapsBalanceAndProgression()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            var settings = GameSessionConfigMapper.ToSettings(config);

            Assert.That(settings.StartingCoins, Is.EqualTo(4));
            Assert.That(settings.DailyCoinStipend, Is.EqualTo(3));
            Assert.That(settings.VisitorsPerDay, Is.EqualTo(2));
            Assert.That(settings.RarityThresholds.Count, Is.EqualTo(2));
        }

        [Test]
        public void Factory_WithValidConfiguration_BuildsPlayableSession()
        {
            var context = new GameSessionConfigTestContext(_factory);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            var session = GameSessionFactory.Create(
                config,
                new FirstItemRandomSource(),
                new SequentialMarketOfferIdSource());
            var purchase = session.PurchaseOffer(session.CurrentOffer.Id);
            var claim = session.ClaimQuest(new QuestId(context.Quest.Id));

            Assert.That(purchase.Succeeded, Is.True);
            Assert.That(claim.Succeeded, Is.True);
            Assert.That(session.IsCompleted, Is.True);
            Assert.That(session.Reputation, Is.EqualTo(1));
        }

        [Test]
        public void ToSettings_WithInvalidConfiguration_ThrowsDescriptiveException()
        {
            var context = new GameSessionConfigTestContext(
                _factory,
                ItemRarity.Uncommon);
            var config = context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            Assert.That(
                () => GameSessionConfigMapper.ToSettings(config),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("at least one Common item"));
        }

        private sealed class FirstItemRandomSource : IRandomSource
        {
            public int NextIndex(int exclusiveUpperBound)
            {
                return 0;
            }
        }
    }
}
