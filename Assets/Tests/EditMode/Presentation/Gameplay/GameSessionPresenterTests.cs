using Azulon.Domain.Items;
using Azulon.Presentation.Gameplay;
using Azulon.Tests.EditMode.Application.Gameplay;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Presentation.Gameplay
{
    public sealed class GameSessionPresenterTests
    {
        private GameSessionTestContext _context;
        private GameSessionPresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _context = new GameSessionTestContext();
            _presenter = new GameSessionPresenter(_context.CreateSession());
        }

        [Test]
        public void CreateViewData_AtSessionStart_ContainsCompleteScreenSnapshot()
        {
            var viewData = _presenter.CreateViewData();

            Assert.That(viewData.DayNumber, Is.EqualTo(1));
            Assert.That(viewData.VisitorNumber, Is.EqualTo(1));
            Assert.That(viewData.VisitorsPerDay, Is.EqualTo(2));
            Assert.That(viewData.Coins, Is.EqualTo(4));
            Assert.That(viewData.Reputation, Is.Zero);
            Assert.That(viewData.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Common));
            Assert.That(viewData.NextRarity, Is.EqualTo(ItemRarity.Uncommon));
            Assert.That(viewData.NextRarityRequiredReputation, Is.EqualTo(1));
            Assert.That(viewData.AvailableItemCount, Is.EqualTo(3));
            Assert.That(viewData.TotalOwnedItemCount, Is.Zero);
            Assert.That(viewData.CollectionItems.Count, Is.EqualTo(3));
            Assert.That(viewData.CollectionItems[0].IsOwned, Is.False);
            Assert.That(viewData.CollectionItems[0].IsUnlocked, Is.True);
            Assert.That(viewData.CollectionItems[1].IsUnlocked, Is.False);
            Assert.That(
                viewData.Offer.Item.TagNames,
                Is.EquivalentTo(new[] { "Fire", "Weapon" }));
            Assert.That(viewData.Offer.CanPurchase, Is.True);
            Assert.That(viewData.QuestCount, Is.EqualTo(2));
            Assert.That(viewData.Quests[0].Requirements[0].Current, Is.Zero);
            Assert.That(viewData.Quests[0].Requirements[0].Required, Is.EqualTo(1));
            Assert.That(viewData.Quests[0].CanClaim, Is.False);
            Assert.That(viewData.IsCompleted, Is.False);
        }

        [Test]
        public void PurchaseOffer_UpdatesInventoryAndReturnsTypedFeedback()
        {
            var offer = _presenter.CreateViewData().Offer;

            var purchase = _presenter.PurchaseOffer(offer.OfferId);
            var updated = _presenter.CreateViewData();
            var duplicate = _presenter.PurchaseOffer(offer.OfferId);

            Assert.That(purchase.Outcome, Is.EqualTo(GameActionOutcome.PurchaseSucceeded));
            Assert.That(purchase.ItemId, Is.EqualTo(_context.EmberBlade.Id));
            Assert.That(purchase.CoinDelta, Is.EqualTo(-4));
            Assert.That(updated.Coins, Is.Zero);
            Assert.That(updated.TotalOwnedItemCount, Is.EqualTo(1));
            Assert.That(updated.InventoryItems.Count, Is.EqualTo(1));
            Assert.That(updated.InventoryItems[0].Quantity, Is.EqualTo(1));
            Assert.That(updated.CollectionItems[0].OwnedQuantity, Is.EqualTo(1));
            Assert.That(updated.Offer.IsPurchased, Is.True);
            Assert.That(updated.Offer.CanPurchase, Is.False);
            Assert.That(duplicate.Outcome, Is.EqualTo(GameActionOutcome.OfferAlreadyPurchased));
        }

        [Test]
        public void ClaimQuest_MapsFailureAndUnlockRewardWithoutUiStrings()
        {
            var unavailableClaim = _presenter.ClaimQuest(_context.EmberCommission.Id);
            var offerId = _presenter.CreateViewData().Offer.OfferId;
            _presenter.PurchaseOffer(offerId);

            var successfulClaim = _presenter.ClaimQuest(_context.EmberCommission.Id);
            var updated = _presenter.CreateViewData();

            Assert.That(
                unavailableClaim.Outcome,
                Is.EqualTo(GameActionOutcome.QuestRequirementsNotMet));
            Assert.That(successfulClaim.Outcome, Is.EqualTo(GameActionOutcome.QuestClaimed));
            Assert.That(successfulClaim.CoinDelta, Is.EqualTo(4));
            Assert.That(successfulClaim.ReputationDelta, Is.EqualTo(1));
            Assert.That(successfulClaim.SessionCompleted, Is.False);
            Assert.That(updated.Reputation, Is.EqualTo(1));
            Assert.That(updated.MaximumUnlockedRarity, Is.EqualTo(ItemRarity.Uncommon));
            Assert.That(updated.NextRarity, Is.EqualTo(ItemRarity.Rare));
            Assert.That(updated.NextRarityRequiredReputation, Is.EqualTo(3));
            Assert.That(updated.Quests[0].IsClaimed, Is.True);
            Assert.That(updated.ClaimedQuestCount, Is.EqualTo(1));
        }

        [Test]
        public void AdvanceToNextVisitor_ReportsDayBoundaryAndStipend()
        {
            var sameDay = _presenter.AdvanceToNextVisitor();
            var newDay = _presenter.AdvanceToNextVisitor();
            var updated = _presenter.CreateViewData();

            Assert.That(sameDay.Outcome, Is.EqualTo(GameActionOutcome.VisitorAdvanced));
            Assert.That(sameDay.CoinDelta, Is.Zero);
            Assert.That(newDay.Outcome, Is.EqualTo(GameActionOutcome.NewDayStarted));
            Assert.That(newDay.CoinDelta, Is.EqualTo(4));
            Assert.That(updated.DayNumber, Is.EqualTo(2));
            Assert.That(updated.VisitorNumber, Is.EqualTo(1));
            Assert.That(updated.Coins, Is.EqualTo(8));
        }

        [Test]
        public void PurchaseOffer_FromPreviousVisitor_ReturnsUnavailableFeedback()
        {
            var staleOfferId = _presenter.CreateViewData().Offer.OfferId;
            _presenter.AdvanceToNextVisitor();

            var result = _presenter.PurchaseOffer(staleOfferId);
            var updated = _presenter.CreateViewData();

            Assert.That(result.Outcome, Is.EqualTo(GameActionOutcome.OfferUnavailable));
            Assert.That(updated.Coins, Is.EqualTo(4));
            Assert.That(updated.TotalOwnedItemCount, Is.Zero);
        }

        [Test]
        public void FinalQuestClaim_ProducesCompletedScreenAndLocksCommands()
        {
            var firstOffer = _presenter.CreateViewData().Offer;
            _presenter.PurchaseOffer(firstOffer.OfferId);
            _presenter.ClaimQuest(_context.EmberCommission.Id);
            _presenter.AdvanceToNextVisitor();

            var flameOrbOffer = _presenter.CreateViewData().Offer;
            Assert.That(flameOrbOffer.Item.Id, Is.EqualTo(_context.FlameOrb.Id));
            _presenter.PurchaseOffer(flameOrbOffer.OfferId);

            var finalClaim = _presenter.ClaimQuest(_context.FlameArsenal.Id);
            var completed = _presenter.CreateViewData();
            var rejectedAdvance = _presenter.AdvanceToNextVisitor();

            Assert.That(finalClaim.Outcome, Is.EqualTo(GameActionOutcome.QuestClaimed));
            Assert.That(finalClaim.SessionCompleted, Is.True);
            Assert.That(completed.IsCompleted, Is.True);
            Assert.That(completed.CanAdvanceVisitor, Is.False);
            Assert.That(completed.ClaimedQuestCount, Is.EqualTo(2));
            Assert.That(completed.NextRarity, Is.Null);
            Assert.That(completed.Offer.CanPurchase, Is.False);
            Assert.That(
                rejectedAdvance.Outcome,
                Is.EqualTo(GameActionOutcome.SessionCompleted));
        }
    }
}
