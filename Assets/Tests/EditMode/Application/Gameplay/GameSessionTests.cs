using System.Collections.Generic;
using Azulon.Domain.Items;
using Azulon.Domain.Market;
using Azulon.Domain.Quests;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Application.Gameplay
{
    public sealed class GameSessionTests
    {
        private GameSessionTestContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new GameSessionTestContext();
        }

        [Test]
        public void Session_CompletesPurchaseQuestUnlockAndSynergyLoop()
        {
            var session = _context.CreateSession();

            Assert.That(session.DayNumber, Is.EqualTo(1));
            Assert.That(session.VisitorNumber, Is.EqualTo(1));
            Assert.That(session.CurrentOffers.Offers.Count, Is.EqualTo(1));
            Assert.That(session.CurrentOffers.Offers[0].Item.Id, Is.EqualTo(_context.EmberBlade.Id));
            Assert.That(session.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Common));

            var firstPurchase = session.PurchaseOffer(session.CurrentOffers.Offers[0].Id);
            var commissionProgress = session.EvaluateQuest(_context.EmberCommission.Id);
            var commissionClaim = session.ClaimQuest(_context.EmberCommission.Id);

            Assert.That(firstPurchase.Status, Is.EqualTo(PurchaseStatus.Success));
            Assert.That(commissionProgress.IsCompleted, Is.True);
            Assert.That(commissionClaim.Succeeded, Is.True);
            Assert.That(session.Coins, Is.EqualTo(4));
            Assert.That(session.Reputation, Is.EqualTo(1));
            Assert.That(session.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Uncommon));

            session.AdvanceToNextVisitor();
            var uncommonOffer = GameSessionTestContext.FindOffer(session, _context.FlameOrb.Id);

            Assert.That(uncommonOffer, Is.Not.Null);
            Assert.That(
                GameSessionTestContext.FindOffer(session, _context.PhoenixSeal.Id),
                Is.Null);

            session.PurchaseOffer(uncommonOffer.Id);
            var synergyProgress = session.EvaluateQuest(_context.FlameArsenal.Id);
            var synergyClaim = session.ClaimQuest(_context.FlameArsenal.Id);

            Assert.That(synergyProgress.IsCompleted, Is.True);
            Assert.That(synergyClaim.Succeeded, Is.True);
            Assert.That(session.Reputation, Is.EqualTo(3));
            Assert.That(session.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Rare));
            Assert.That(session.IsCompleted, Is.True);
            Assert.That(session.TotalOwnedItemCount, Is.EqualTo(2));
            Assert.That(session.CreateInventorySnapshot().Count, Is.EqualTo(2));
        }

        [Test]
        public void AdvanceToNextVisitor_OnDayBoundary_CreditsDailyStipendOnce()
        {
            var session = _context.CreateSession();

            var sameDay = session.AdvanceToNextVisitor();
            var nextDay = session.AdvanceToNextVisitor();

            Assert.That(sameDay.StartedNewDay, Is.False);
            Assert.That(sameDay.CreditedCoins, Is.Zero);
            Assert.That(sameDay.DayNumber, Is.EqualTo(1));
            Assert.That(sameDay.VisitorNumber, Is.EqualTo(2));
            Assert.That(nextDay.StartedNewDay, Is.True);
            Assert.That(nextDay.CreditedCoins, Is.EqualTo(4));
            Assert.That(nextDay.WalletBalance, Is.EqualTo(8));
            Assert.That(nextDay.DayNumber, Is.EqualTo(2));
            Assert.That(nextDay.VisitorNumber, Is.EqualTo(1));
        }

        [Test]
        public void PurchaseOffer_FromPreviousVisitor_IsRejectedWithoutMutation()
        {
            var session = _context.CreateSession();
            var previousOfferId = session.CurrentOffers.Offers[0].Id;
            session.AdvanceToNextVisitor();

            Assert.That(
                () => session.PurchaseOffer(previousOfferId),
                Throws.TypeOf<KeyNotFoundException>());
            Assert.That(session.Coins, Is.EqualTo(4));
            Assert.That(session.TotalOwnedItemCount, Is.Zero);
        }

        [Test]
        public void ClaimQuest_WhenRequirementsAreMissing_ReturnsDomainFeedback()
        {
            var session = _context.CreateSession();

            var result = session.ClaimQuest(new QuestId("ember_commission"));

            Assert.That(result.Status, Is.EqualTo(QuestClaimStatus.RequirementsNotMet));
            Assert.That(session.Coins, Is.EqualTo(4));
            Assert.That(session.Reputation, Is.Zero);
            Assert.That(session.IsCompleted, Is.False);
        }
    }
}
