using System;
using Azulon.Domain.Items;
using Azulon.Domain.Progression;
using Azulon.Domain.Quests;
using Azulon.Unity.UI;
using NUnit.Framework;

namespace Azulon.Tests.EditMode.Configuration
{
    public sealed class GameContentViewCatalogTests
    {
        private ScriptableObjectTestFactory _factory;
        private GameSessionConfigTestContext _context;

        [SetUp]
        public void SetUp()
        {
            _factory = new ScriptableObjectTestFactory();
            _context = new GameSessionConfigTestContext(_factory);
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public void Catalog_ResolvesItemIconAndRequirementLabelByStableIds()
        {
            var configuration = _context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0),
                new RarityUnlockThreshold(ItemRarity.Uncommon, 1));

            var catalog = new GameContentViewCatalog(configuration);

            Assert.That(
                catalog.GetItemIcon(new ItemId(_context.Item.Id)),
                Is.SameAs(_context.Item.Icon));
            Assert.That(
                catalog.GetRequirementDisplayName(new QuestId(_context.Quest.Id), 0),
                Is.EqualTo("Ember Blade"));
            Assert.That(
                catalog.GetVisitorSprite(1, 1, 2),
                Is.SameAs(_context.VisitorSprites[0]));
        }

        [Test]
        public void GetRequirementDisplayName_WithUnknownIndex_ThrowsDescriptiveError()
        {
            var configuration = _context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0));
            var catalog = new GameContentViewCatalog(configuration);

            Assert.That(
                () => catalog.GetRequirementDisplayName(
                    new QuestId(_context.Quest.Id),
                    1),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains("has no requirement at index 1"));
        }

        [Test]
        public void GetVisitorSprite_CyclesAcrossDayBoundary()
        {
            var configuration = _context.CreateConfig(
                new RarityUnlockThreshold(ItemRarity.Common, 0));
            var catalog = new GameContentViewCatalog(configuration);

            var firstVisitor = catalog.GetVisitorSprite(1, 1, 2);
            var secondVisitor = catalog.GetVisitorSprite(1, 2, 2);
            var nextDayVisitor = catalog.GetVisitorSprite(2, 1, 2);

            Assert.That(firstVisitor, Is.SameAs(_context.VisitorSprites[0]));
            Assert.That(secondVisitor, Is.SameAs(_context.VisitorSprites[1]));
            Assert.That(nextDayVisitor, Is.SameAs(_context.VisitorSprites[0]));
            Assert.That(secondVisitor, Is.Not.SameAs(nextDayVisitor));
        }
    }
}
