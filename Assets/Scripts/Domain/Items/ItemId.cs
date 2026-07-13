using System;
using Azulon.Domain.Common;

namespace Azulon.Domain.Items
{
    public readonly struct ItemId : IEquatable<ItemId>
    {
        private readonly string _value;

        public ItemId(string value)
        {
            if (!IdentifierRules.IsValid(value))
            {
                throw new ArgumentException($"Item ID {IdentifierRules.FormatDescription}.", nameof(value));
            }

            _value = value;
        }

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public static bool TryCreate(string value, out ItemId itemId)
        {
            if (!IdentifierRules.IsValid(value))
            {
                itemId = default;
                return false;
            }

            itemId = new ItemId(value);
            return true;
        }

        public bool Equals(ItemId other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(_value ?? string.Empty);
        }

        public override string ToString()
        {
            return _value ?? string.Empty;
        }

        public static bool operator ==(ItemId left, ItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemId left, ItemId right)
        {
            return !left.Equals(right);
        }
    }
}
