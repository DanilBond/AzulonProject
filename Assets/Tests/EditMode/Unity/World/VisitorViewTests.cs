using Azulon.Unity.World;
using NUnit.Framework;
using UnityEngine;

namespace Azulon.Tests.EditMode.Unity.World
{
    public sealed class VisitorViewTests
    {
        private GameObject _root;
        private SpriteRenderer _spriteRenderer;
        private VisitorView _view;
        private Texture2D _firstTexture;
        private Texture2D _secondTexture;
        private Sprite _firstSprite;
        private Sprite _secondSprite;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("VisitorView_Test", typeof(SpriteRenderer));
            _spriteRenderer = _root.GetComponent<SpriteRenderer>();
            _view = _root.AddComponent<VisitorView>();

            _firstTexture = new Texture2D(1, 1);
            _secondTexture = new Texture2D(1, 1);
            _firstSprite = CreateSprite(_firstTexture);
            _secondSprite = CreateSprite(_secondTexture);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
            Object.DestroyImmediate(_firstSprite);
            Object.DestroyImmediate(_secondSprite);
            Object.DestroyImmediate(_firstTexture);
            Object.DestroyImmediate(_secondTexture);
        }

        [Test]
        public void Bind_FirstVisitor_AssignsSpriteAndStartsEntrance()
        {
            _view.Bind(_firstSprite);

            Assert.That(_spriteRenderer.sprite, Is.SameAs(_firstSprite));
            Assert.That(_view.CurrentSprite, Is.SameAs(_firstSprite));
            Assert.That(_view.IsAnimating, Is.True);
        }

        [Test]
        public void Bind_WhileDisabled_ReplacesSpriteSynchronously()
        {
            _root.SetActive(false);
            _view.Bind(_firstSprite);

            _view.Bind(_secondSprite);

            Assert.That(_spriteRenderer.sprite, Is.SameAs(_secondSprite));
            Assert.That(_view.CurrentSprite, Is.SameAs(_secondSprite));
            Assert.That(_view.IsAnimating, Is.False);
        }

        [Test]
        public void Bind_WithoutSprite_Throws()
        {
            Assert.That(
                () => _view.Bind(null),
                Throws.ArgumentNullException);
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f));
        }
    }
}
