using System;

namespace Azulon.Domain.Economy
{
    public sealed class Wallet
    {
        public Wallet(int initialBalance)
        {
            if (initialBalance < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(initialBalance),
                    "Initial wallet balance cannot be negative.");
            }

            Balance = initialBalance;
        }

        public int Balance { get; private set; }

        public bool CanAfford(int amount)
        {
            EnsurePositiveAmount(amount);
            return Balance >= amount;
        }

        public bool TrySpend(int amount)
        {
            EnsurePositiveAmount(amount);
            if (Balance < amount)
            {
                return false;
            }

            Balance -= amount;
            return true;
        }

        public void Credit(int amount)
        {
            EnsurePositiveAmount(amount);
            Balance = checked(Balance + amount);
        }

        private static void EnsurePositiveAmount(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Coin amount must be greater than zero.");
            }
        }
    }
}
