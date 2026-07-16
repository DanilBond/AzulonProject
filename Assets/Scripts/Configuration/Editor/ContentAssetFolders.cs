using System;

namespace Azulon.Configuration.Editor
{
    public sealed class ContentAssetFolders
    {
        public static ContentAssetFolders Default { get; } = new ContentAssetFolders(
            "Assets/Data/Items/Tags",
            "Assets/Data/Items/Definitions",
            "Assets/Data/Quests/Requirements",
            "Assets/Data/Quests/Definitions");

        public ContentAssetFolders(
            string tags,
            string items,
            string requirements,
            string quests)
        {
            Tags = ValidateFolder(tags, nameof(tags));
            Items = ValidateFolder(items, nameof(items));
            Requirements = ValidateFolder(requirements, nameof(requirements));
            Quests = ValidateFolder(quests, nameof(quests));
        }

        public string Tags { get; }

        public string Items { get; }

        public string Requirements { get; }

        public string Quests { get; }

        private static string ValidateFolder(string folder, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(folder) ||
                (!folder.Equals("Assets", StringComparison.Ordinal) &&
                 !folder.StartsWith("Assets/", StringComparison.Ordinal)))
            {
                throw new ArgumentException(
                    "Content folder must be an Assets-relative path.",
                    parameterName);
            }

            return folder.TrimEnd('/');
        }
    }
}
