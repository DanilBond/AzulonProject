using System;
using Azulon.Domain.Common;

namespace Azulon.Domain.Items
{
    public readonly struct ItemTagId : IEquatable<ItemTagId>
    {
        private readonly string _value;

        public ItemTagId(string value)
        {
            if (!IdentifierRules.IsValid(value))
            {
                throw new ArgumentException($"Item tag ID {IdentifierRules.FormatDescription}.", nameof(value));
            }

            _value = value;
        }

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public static bool TryCreate(string value, out ItemTagId tagId)
        {
            if (!IdentifierRules.IsValid(value))
            {
                tagId = default;
                return false;
            }

            tagId = new ItemTagId(value);
            return true;
        }

        public bool Equals(ItemTagId other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemTagId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(_value ?? string.Empty);
        }

        public override string ToString()
        {
            return _value ?? string.Empty;
        }

        public static bool operator ==(ItemTagId left, ItemTagId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemTagId left, ItemTagId right)
        {
            return !left.Equals(right);
        }
    }
}
