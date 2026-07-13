using System;
using Azulon.Domain.Economy;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Economy
{
    public sealed class WalletTests
    {
        [Test]
        public void Constructor_WithNegativeBalance_Throws()
        {
            Assert.That(
                () => new Wallet(-1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void TrySpend_WithAffordableAmount_DeductsCoins()
        {
            var wallet = new Wallet(10);

            var spent = wallet.TrySpend(4);

            Assert.That(spent, Is.True);
            Assert.That(wallet.Balance, Is.EqualTo(6));
        }

        [Test]
        public void TrySpend_WithInsufficientBalance_DoesNotChangeWallet()
        {
            var wallet = new Wallet(3);

            var spent = wallet.TrySpend(4);

            Assert.That(spent, Is.False);
            Assert.That(wallet.Balance, Is.EqualTo(3));
        }

        [Test]
        public void Credit_WithPositiveAmount_AddsCoins()
        {
            var wallet = new Wallet(3);

            wallet.Credit(5);

            Assert.That(wallet.Balance, Is.EqualTo(8));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void CoinOperations_WithNonPositiveAmount_Throw(int amount)
        {
            var wallet = new Wallet(3);

            Assert.That(() => wallet.CanAfford(amount), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => wallet.TrySpend(amount), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => wallet.Credit(amount), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(wallet.Balance, Is.EqualTo(3));
        }
    }
}
