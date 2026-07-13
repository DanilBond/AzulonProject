using System;
using System.Globalization;

namespace Azulon.Domain.Market
{
    public readonly struct MarketOfferId : IEquatable<MarketOfferId>
    {
        private readonly long _value;

        public MarketOfferId(long value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Market offer ID must be greater than zero.");
            }

            _value = value;
        }

        public bool IsEmpty => _value <= 0;

        public long Value => _value;

        public bool Equals(MarketOfferId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is MarketOfferId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool operator ==(MarketOfferId left, MarketOfferId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MarketOfferId left, MarketOfferId right)
        {
            return !left.Equals(right);
        }
    }
}
