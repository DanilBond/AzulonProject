using Azulon.Unity.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Azulon.Tests.EditMode.Unity.UI
{
    public sealed class UiVisibilityAnimatorTests
    {
        private static readonly Vector2 VisiblePosition = new Vector2(12f, 34f);
        private static readonly Vector2 HiddenOffset = new Vector2(20f, -8f);

        private GameObject _root;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private UiVisibilityAnimator _animator;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(
                "UiVisibilityAnimator_Test",
                typeof(RectTransform),
                typeof(CanvasGroup));
            _root.SetActive(false);
            _rectTransform = _root.GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = VisiblePosition;
            _rectTransform.localScale = Vector3.one;
            _canvasGroup = _root.GetComponent<CanvasGroup>();
            _animator = _root.AddComponent<UiVisibilityAnimator>();

            var serializedAnimator = new SerializedObject(_animator);
            serializedAnimator.FindProperty("_hiddenOffset").vector2Value = HiddenOffset;
            serializedAnimator.FindProperty("_hiddenScale").floatValue = 0.9f;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();

            _root.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void SetVisibleFalseInstantly_AppliesHiddenNonInteractiveState()
        {
            _animator.SetVisible(false, true);

            Assert.That(_animator.IsVisible, Is.False);
            Assert.That(_animator.IsAnimating, Is.False);
            Assert.That(_root.activeSelf, Is.False);
            Assert.That(_canvasGroup.alpha, Is.Zero);
            Assert.That(_canvasGroup.interactable, Is.False);
            Assert.That(_canvasGroup.blocksRaycasts, Is.False);
            Assert.That(
                _rectTransform.anchoredPosition,
                Is.EqualTo(VisiblePosition + HiddenOffset));
            Assert.That(_rectTransform.localScale.x, Is.EqualTo(0.9f));
            Assert.That(_rectTransform.localScale.y, Is.EqualTo(0.9f));
        }

        [Test]
        public void SetVisibleTrueInstantly_RestoresAuthoredTransformAndInteraction()
        {
            _animator.SetVisible(false, true);

            _animator.SetVisible(true, true);

            Assert.That(_animator.IsVisible, Is.True);
            Assert.That(_animator.IsAnimating, Is.False);
            Assert.That(_root.activeSelf, Is.True);
            Assert.That(_canvasGroup.alpha, Is.EqualTo(1f));
            Assert.That(_canvasGroup.interactable, Is.True);
            Assert.That(_canvasGroup.blocksRaycasts, Is.True);
            Assert.That(_rectTransform.anchoredPosition, Is.EqualTo(VisiblePosition));
            Assert.That(_rectTransform.localScale, Is.EqualTo(Vector3.one));
        }

        [Test]
        public void PlayEntranceWithZeroDuration_CompletesSynchronously()
        {
            var serializedAnimator = new SerializedObject(_animator);
            serializedAnimator.FindProperty("_showDuration").floatValue = 0f;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();
            _animator.SetVisible(false, true);

            _animator.PlayEntrance();

            Assert.That(_animator.IsVisible, Is.True);
            Assert.That(_animator.IsAnimating, Is.False);
            Assert.That(_root.activeSelf, Is.True);
            Assert.That(_canvasGroup.alpha, Is.EqualTo(1f));
            Assert.That(_rectTransform.anchoredPosition, Is.EqualTo(VisiblePosition));
        }
    }
}
