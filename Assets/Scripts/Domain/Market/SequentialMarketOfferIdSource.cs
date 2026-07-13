using System;

namespace Azulon.Domain.Market
{
    public sealed class SequentialMarketOfferIdSource : IMarketOfferIdSource
    {
        private long _nextValue;
        private bool _isExhausted;

        public SequentialMarketOfferIdSource(long firstValue = 1)
        {
            if (firstValue <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(firstValue),
                    "First market offer ID must be greater than zero.");
            }

            _nextValue = firstValue;
        }

        public MarketOfferId NextId()
        {
            if (_isExhausted)
            {
                throw new InvalidOperationException("Market offer ID sequence is exhausted.");
            }

            var id = new MarketOfferId(_nextValue);
            if (_nextValue == long.MaxValue)
            {
                _isExhausted = true;
            }
            else
            {
                _nextValue++;
            }

            return id;
        }
    }
}
