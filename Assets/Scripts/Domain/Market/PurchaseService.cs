using System;
using Azulon.Domain.Economy;
using Azulon.Domain.Inventory;

namespace Azulon.Domain.Market
{
    public sealed class PurchaseService
    {
        public PurchaseResult Purchase(
            MarketOffer offer,
            Wallet wallet,
            PlayerInventory inventory)
        {
            if (offer == null)
            {
                throw new ArgumentNullException(nameof(offer));
            }

            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (offer.IsPurchased)
            {
                return CreateResult(PurchaseStatus.AlreadyPurchased, offer, wallet, inventory);
            }

            if (!wallet.CanAfford(offer.Price))
            {
                return CreateResult(PurchaseStatus.InsufficientFunds, offer, wallet, inventory);
            }

            inventory.Add(offer.Item.Id);
            if (!wallet.TrySpend(offer.Price))
            {
                throw new InvalidOperationException("Wallet balance changed during a synchronous purchase.");
            }

            offer.MarkPurchased();
            return CreateResult(PurchaseStatus.Success, offer, wallet, inventory);
        }

        private static PurchaseResult CreateResult(
            PurchaseStatus status,
            MarketOffer offer,
            Wallet wallet,
            PlayerInventory inventory)
        {
            return new PurchaseResult(
                status,
                offer.Id,
                offer.Item.Id,
                wallet.Balance,
                inventory.GetQuantity(offer.Item.Id));
        }
    }
}
