namespace Azulon.Domain.Common
{
    internal static class IdentifierRules
    {
        public const string FormatDescription =
            "must start with a lowercase letter and contain only lowercase letters, digits, or underscores";

        public static bool IsValid(string value)
        {
            if (string.IsNullOrEmpty(value) || !IsLowercaseLetter(value[0]))
            {
                return false;
            }

            for (var index = 1; index < value.Length; index++)
            {
                var character = value[index];
                if (!IsLowercaseLetter(character) && !IsDigit(character) && character != '_')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsLowercaseLetter(char character)
        {
            return character >= 'a' && character <= 'z';
        }

        private static bool IsDigit(char character)
        {
            return character >= '0' && character <= '9';
        }
    }
}
