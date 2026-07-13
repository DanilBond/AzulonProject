using Azulon.Domain.Items;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Domain.Items
{
    public sealed class ItemIdTests
    {
        [Test]
        public void TryCreate_WithCanonicalId_CreatesValue()
        {
            var created = ItemId.TryCreate("ancient_blade_2", out var itemId);

            Assert.That(created, Is.True);
            Assert.That(itemId.ToString(), Is.EqualTo("ancient_blade_2"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("2_blades")]
        [TestCase("AncientBlade")]
        [TestCase("ancient blade")]
        [TestCase("ancient-blade")]
        [TestCase("item_\u0661")]
        public void TryCreate_WithNonCanonicalId_ReturnsFalse(string value)
        {
            Assert.That(ItemId.TryCreate(value, out _), Is.False);
        }

        [Test]
        public void Equality_UsesOrdinalIdValue()
        {
            var first = new ItemId("ancient_blade");
            var second = new ItemId("ancient_blade");
            var different = new ItemId("sun_charm");

            Assert.That(first, Is.EqualTo(second));
            Assert.That(first == second, Is.True);
            Assert.That(first != different, Is.True);
        }
    }
}
