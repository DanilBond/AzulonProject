using Azulon.Presentation.Gameplay;
using Azulon.Tests.EditMode.Application.Gameplay;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Presentation.Gameplay
{
    public sealed class GameFeedbackFormatterTests
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
        public void Format_AfterPurchase_UsesItemNameAndCoinDelta()
        {
            var offerId = _presenter.CreateViewData().Offers[0].OfferId;
            var result = _presenter.PurchaseOffer(offerId);

            var feedback = GameFeedbackFormatter.Format(
                result,
                _presenter.CreateViewData());

            Assert.That(feedback.Tone, Is.EqualTo(GameFeedbackTone.Positive));
            Assert.That(feedback.Message, Does.Contain(_context.EmberBlade.DisplayName));
            Assert.That(feedback.Message, Does.Contain("4 coins"));
        }

        [Test]
        public void Format_AfterQuestClaim_ListsBothRewards()
        {
            var offerId = _presenter.CreateViewData().Offers[0].OfferId;
            _presenter.PurchaseOffer(offerId);
            var result = _presenter.ClaimQuest(_context.EmberCommission.Id);

            var feedback = GameFeedbackFormatter.Format(
                result,
                _presenter.CreateViewData());

            Assert.That(feedback.Message, Does.Contain("Ember Commission"));
            Assert.That(feedback.Message, Does.Contain("+4 coins"));
            Assert.That(feedback.Message, Does.Contain("+1 reputation"));
        }
    }
}
