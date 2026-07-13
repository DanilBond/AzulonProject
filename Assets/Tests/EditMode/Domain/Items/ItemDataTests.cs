using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Items
{
    public sealed class ItemDataTests
    {
        [Test]
        public void Constructor_CopiesTagsAndExposesItemAttributes()
        {
            var fire = new ItemTagId("fire");
            var weapon = new ItemTagId("weapon");
            var sourceTags = new[] { fire, weapon };

            var item = new ItemData(
                new ItemId("ember_blade"),
                "Ember Blade",
                "A blade holding a steady magical flame.",
                4,
                7,
                ItemRarity.Uncommon,
                ItemCategory.Weapon,
                sourceTags);

            sourceTags[0] = new ItemTagId("water");

            Assert.That(item.Price, Is.EqualTo(4));
            Assert.That(item.Power, Is.EqualTo(7));
            Assert.That(item.HasTag(fire), Is.True);
            Assert.That(item.HasTag(new ItemTagId("water")), Is.False);
        }

        [Test]
        public void Constructor_WithDuplicateTag_Throws()
        {
            var fire = new ItemTagId("fire");

            Assert.That(
                () => new ItemData(
                    new ItemId("ember_blade"),
                    "Ember Blade",
                    "A blade holding a steady magical flame.",
                    4,
                    7,
                    ItemRarity.Uncommon,
                    ItemCategory.Weapon,
                    new[] { fire, fire }),
                Throws.ArgumentException);
        }

        [Test]
        public void Constructor_WithoutTags_Throws()
        {
            Assert.That(
                () => new ItemData(
                    new ItemId("plain_blade"),
                    "Plain Blade",
                    "A practical guild weapon.",
                    2,
                    2,
                    ItemRarity.Common,
                    ItemCategory.Weapon,
                    new ItemTagId[0]),
                Throws.ArgumentException);
        }
    }
}
