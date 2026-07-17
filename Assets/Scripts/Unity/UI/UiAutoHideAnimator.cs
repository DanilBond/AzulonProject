using DG.Tweening;
using UnityEngine;

namespace Azulon.Unity.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UiVisibilityAnimator))]
    public sealed class UiAutoHideAnimator : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _visibleDuration = 1.6f;
        [SerializeField] private bool _useUnscaledTime = true;

        private UiVisibilityAnimator _visibilityAnimator;
        private Tween _hideDelay;

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnDisable()
        {
            KillHideDelay();
        }

        private void OnDestroy()
        {
            KillHideDelay();
        }

        public void Play()
        {
            EnsureInitialized();
            KillHideDelay();
            _visibilityAnimator.PlayEntrance();

            if (_visibleDuration <= 0f)
            {
                HideAfterDelay();
                return;
            }

            _hideDelay = DOVirtual
                .DelayedCall(_visibleDuration, HideAfterDelay)
                .SetUpdate(_useUnscaledTime);
        }

        public void Hide(bool instant = false)
        {
            EnsureInitialized();
            KillHideDelay();
            _visibilityAnimator.SetVisible(false, instant);
        }

        private void HideAfterDelay()
        {
            _hideDelay = null;
            _visibilityAnimator.SetVisible(false);
        }

        private void EnsureInitialized()
        {
            if (_visibilityAnimator == null)
            {
                _visibilityAnimator = GetComponent<UiVisibilityAnimator>();
            }
        }

        private void KillHideDelay()
        {
            if (_hideDelay == null)
            {
                return;
            }

            if (_hideDelay.IsActive())
            {
                _hideDelay.Kill();
            }

            _hideDelay = null;
        }
    }
}
