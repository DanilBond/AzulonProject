using DG.Tweening;
using UnityEngine;

namespace Azulon.Unity.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public sealed class UiVisibilityAnimator : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _showDuration = 0.18f;
        [SerializeField, Min(0f)] private float _hideDuration = 0.14f;
        [SerializeField] private Vector2 _hiddenOffset = new Vector2(24f, 0f);
        [SerializeField, Range(0.01f, 1f)] private float _hiddenScale = 0.98f;
        [SerializeField] private Ease _showEase = Ease.OutCubic;
        [SerializeField] private Ease _hideEase = Ease.InCubic;
        [SerializeField] private bool _useUnscaledTime = true;
        [SerializeField] private bool _disableWhenHidden = true;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _visiblePosition;
        private Vector3 _visibleScale;
        private Sequence _sequence;
        private bool _isInitialized;
        private bool _hasAppliedState;
        private bool _targetVisible;

        public bool IsVisible => _targetVisible;

        public bool IsAnimating => _sequence != null && _sequence.IsActive();

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnDisable()
        {
            if (!_isInitialized)
            {
                return;
            }

            KillSequence();
            if (_targetVisible)
            {
                ApplyVisibleState();
            }
            else
            {
                ApplyHiddenState();
            }
        }

        private void OnDestroy()
        {
            KillSequence();
        }

        public void SetVisible(bool visible, bool instant = false)
        {
            EnsureInitialized();
            if (_hasAppliedState && _targetVisible == visible && !instant)
            {
                return;
            }

            _hasAppliedState = true;
            _targetVisible = visible;
            KillSequence();

            if (instant)
            {
                if (visible)
                {
                    CompleteShow();
                }
                else
                {
                    CompleteHide();
                }

                return;
            }

            if (visible)
            {
                if (!gameObject.activeSelf)
                {
                    ApplyHiddenState();
                    gameObject.SetActive(true);
                }

                SetInteraction(false);
                StartShow(0f);
            }
            else
            {
                SetInteraction(false);
                if (!gameObject.activeSelf || _hideDuration <= 0f)
                {
                    CompleteHide();
                    return;
                }

                StartHide();
            }
        }

        public void PlayEntrance(float delay = 0f)
        {
            EnsureInitialized();
            KillSequence();

            _hasAppliedState = true;
            _targetVisible = true;
            ApplyHiddenState();
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            SetInteraction(false);
            StartShow(Mathf.Max(0f, delay));
        }

        private void StartShow(float delay)
        {
            if (_showDuration <= 0f && delay <= 0f)
            {
                CompleteShow();
                return;
            }

            var sequence = DOTween.Sequence();
            if (delay > 0f)
            {
                sequence.AppendInterval(delay);
            }

            sequence.Append(DOTween
                .To(
                    () => _canvasGroup.alpha,
                    value => _canvasGroup.alpha = value,
                    1f,
                    _showDuration)
                .SetEase(_showEase));
            sequence.Join(DOTween
                .To(
                    () => _rectTransform.anchoredPosition,
                    value => _rectTransform.anchoredPosition = value,
                    _visiblePosition,
                    _showDuration)
                .SetEase(_showEase));
            sequence.Join(DOTween
                .To(
                    () => _rectTransform.localScale,
                    value => _rectTransform.localScale = value,
                    _visibleScale,
                    _showDuration)
                .SetEase(_showEase));
            sequence.SetUpdate(_useUnscaledTime);
            sequence.OnComplete(CompleteShow);
            _sequence = sequence;
        }

        private void StartHide()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween
                .To(
                    () => _canvasGroup.alpha,
                    value => _canvasGroup.alpha = value,
                    0f,
                    _hideDuration)
                .SetEase(_hideEase));
            sequence.Join(DOTween
                .To(
                    () => _rectTransform.anchoredPosition,
                    value => _rectTransform.anchoredPosition = value,
                    GetHiddenPosition(),
                    _hideDuration)
                .SetEase(_hideEase));
            sequence.Join(DOTween
                .To(
                    () => _rectTransform.localScale,
                    value => _rectTransform.localScale = value,
                    GetHiddenScale(),
                    _hideDuration)
                .SetEase(_hideEase));
            sequence.SetUpdate(_useUnscaledTime);
            sequence.OnComplete(CompleteHide);
            _sequence = sequence;
        }

        private void CompleteShow()
        {
            _sequence = null;
            ApplyVisibleState();
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        private void CompleteHide()
        {
            _sequence = null;
            ApplyHiddenState();
            if (_disableWhenHidden && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        private void ApplyVisibleState()
        {
            _canvasGroup.alpha = 1f;
            _rectTransform.anchoredPosition = _visiblePosition;
            _rectTransform.localScale = _visibleScale;
            SetInteraction(true);
        }

        private void ApplyHiddenState()
        {
            _canvasGroup.alpha = 0f;
            _rectTransform.anchoredPosition = GetHiddenPosition();
            _rectTransform.localScale = GetHiddenScale();
            SetInteraction(false);
        }

        private Vector2 GetHiddenPosition()
        {
            return _visiblePosition + _hiddenOffset;
        }

        private Vector3 GetHiddenScale()
        {
            return new Vector3(
                _visibleScale.x * _hiddenScale,
                _visibleScale.y * _hiddenScale,
                _visibleScale.z);
        }

        private void SetInteraction(bool isEnabled)
        {
            _canvasGroup.interactable = isEnabled;
            _canvasGroup.blocksRaycasts = isEnabled;
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            _rectTransform = transform as RectTransform;
            if (_rectTransform == null)
            {
                throw new MissingComponentException(
                    $"UI visibility animator '{name}' requires a RectTransform.");
            }

            _canvasGroup = GetComponent<CanvasGroup>();
            _visiblePosition = _rectTransform.anchoredPosition;
            _visibleScale = _rectTransform.localScale;
            _targetVisible = gameObject.activeSelf;
            _isInitialized = true;
        }

        private void KillSequence()
        {
            if (_sequence == null)
            {
                return;
            }

            if (_sequence.IsActive())
            {
                _sequence.Kill();
            }

            _sequence = null;
        }
    }
}
