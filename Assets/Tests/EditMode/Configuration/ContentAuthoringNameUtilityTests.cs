using Azulon.Configuration.Editor;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class ContentAuthoringNameUtilityTests
    {
        [TestCase("Dragonforged Axe", "dragonforged_axe")]
        [TestCase("  Frost--Tonic 2  ", "frost_tonic_2")]
        [TestCase("Guildmaster's Crown", "guildmaster_s_crown")]
        public void ToStableId_NormalizesSupportedNames(string displayName, string expected)
        {
            Assert.That(
                ContentAuthoringNameUtility.ToStableId(displayName),
                Is.EqualTo(expected));
        }

        [Test]
        public void ToAssetSuffix_UsesPascalCase()
        {
            Assert.That(
                ContentAuthoringNameUtility.ToAssetSuffix("winter_signet"),
                Is.EqualTo("WinterSignet"));
        }
    }
}
