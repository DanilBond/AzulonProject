using System;
using Azulon.Domain.Common;

namespace Azulon.Domain.Quests
{
    public readonly struct QuestId : IEquatable<QuestId>
    {
        private readonly string _value;

        public QuestId(string value)
        {
            if (!IdentifierRules.IsValid(value))
            {
                throw new ArgumentException($"Quest ID {IdentifierRules.FormatDescription}.", nameof(value));
            }

            _value = value;
        }

        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public static bool TryCreate(string value, out QuestId questId)
        {
            if (!IdentifierRules.IsValid(value))
            {
                questId = default;
                return false;
            }

            questId = new QuestId(value);
            return true;
        }

        public bool Equals(QuestId other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is QuestId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(_value ?? string.Empty);
        }

        public override string ToString()
        {
            return _value ?? string.Empty;
        }

        public static bool operator ==(QuestId left, QuestId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(QuestId left, QuestId right)
        {
            return !left.Equals(right);
        }
    }
}
