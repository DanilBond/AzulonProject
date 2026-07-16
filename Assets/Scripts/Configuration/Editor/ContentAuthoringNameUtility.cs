using System;
using System.Text;

namespace Azulon.Configuration.Editor
{
    public static class ContentAuthoringNameUtility
    {
        public static string ToStableId(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(displayName.Length);
            var separatorPending = false;
            foreach (var character in displayName.Trim())
            {
                if (character >= 'A' && character <= 'Z')
                {
                    AppendPendingSeparator(builder, ref separatorPending);
                    builder.Append((char)(character + ('a' - 'A')));
                }
                else if ((character >= 'a' && character <= 'z') ||
                         (character >= '0' && character <= '9'))
                {
                    AppendPendingSeparator(builder, ref separatorPending);
                    builder.Append(character);
                }
                else if (builder.Length > 0)
                {
                    separatorPending = true;
                }
            }

            return builder.ToString();
        }

        public static string ToAssetSuffix(string value)
        {
            var stableId = ToStableId(value);
            if (stableId.Length == 0)
            {
                return "New";
            }

            var builder = new StringBuilder(stableId.Length);
            var capitalize = true;
            foreach (var character in stableId)
            {
                if (character == '_')
                {
                    capitalize = true;
                    continue;
                }

                builder.Append(capitalize
                    ? char.ToUpperInvariant(character)
                    : character);
                capitalize = false;
            }

            return builder.ToString();
        }

        private static void AppendPendingSeparator(
            StringBuilder builder,
            ref bool separatorPending)
        {
            if (separatorPending && builder.Length > 0)
            {
                builder.Append('_');
            }

            separatorPending = false;
        }
    }
}
