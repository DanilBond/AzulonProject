using System;
using DG.Tweening;
using UnityEngine;

namespace Azulon.Unity.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class VisitorView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float _exitDuration = 0.16f;
        [SerializeField, Min(0f)] private float _enterDuration = 0.28f;
        [SerializeField] private Vector3 _entranceOffset = new Vector3(-1.25f, 0f, 0f);
        [SerializeField] private Vector3 _exitOffset = new Vector3(1f, 0f, 0f);
        [SerializeField, Range(0.01f, 1f)] private float _hiddenScale = 0.96f;
        [SerializeField] private Ease _exitEase = Ease.InCubic;
        [SerializeField] private Ease _enterEase = Ease.OutCubic;
        [SerializeField] private bool _useUnscaledTime = true;

        private Vector3 _visibleLocalPosition;
        private Vector3 _visibleLocalScale;
        private Color _visibleColor;
        private Sequence _sequence;
        private Sprite _boundSprite;
        private bool _hasBoundSprite;
        private bool _isInitialized;

        public Sprite CurrentSprite => _boundSprite;

        public bool IsAnimating => _sequence != null && _sequence.IsActive();

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnDisable()
        {
            if (!_isInitialized || !_hasBoundSprite)
            {
                return;
            }

            KillSequence();
            _spriteRenderer.sprite = _boundSprite;
            ApplyVisibleState();
        }

        private void OnDestroy()
        {
            KillSequence();
        }

        public void Bind(Sprite sprite)
        {
            if (sprite == null)
            {
                throw new ArgumentNullException(nameof(sprite));
            }

            EnsureInitialized();
            if (_hasBoundSprite && _boundSprite == sprite)
            {
                return;
            }

            KillSequence();
            var hadPreviousSprite = _hasBoundSprite;
            if (hadPreviousSprite)
            {
                _spriteRenderer.sprite = _boundSprite;
                ApplyVisibleState();
            }

            _boundSprite = sprite;
            _hasBoundSprite = true;

            if (!isActiveAndEnabled)
            {
                CompleteTransition();
                return;
            }

            if (hadPreviousSprite)
            {
                PlaySwap(sprite);
            }
            else
            {
                PlayEntrance(sprite);
            }
        }

        public bool TryValidateReferences(out string error)
        {
            ResolveSpriteRenderer();
            if (_spriteRenderer == null)
            {
                error = $"World visitor view '{name}' requires a SpriteRenderer.";
                return false;
            }

            error = null;
            return true;
        }

        private void PlayEntrance(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            if (_enterDuration <= 0f)
            {
                CompleteTransition();
                return;
            }

            ApplyEntranceState();
            var sequence = DOTween.Sequence();
            AppendEntrance(sequence);
            StartSequence(sequence);
        }

        private void PlaySwap(Sprite sprite)
        {
            if (_exitDuration <= 0f && _enterDuration <= 0f)
            {
                _spriteRenderer.sprite = sprite;
                CompleteTransition();
                return;
            }

            var sequence = DOTween.Sequence();
            if (_exitDuration > 0f)
            {
                AppendExit(sequence);
            }

            sequence.AppendCallback(() =>
            {
                _spriteRenderer.sprite = sprite;
                ApplyEntranceState();
            });

            if (_enterDuration > 0f)
            {
                AppendEntrance(sequence);
            }

            StartSequence(sequence);
        }

        private void AppendExit(Sequence sequence)
        {
            sequence.Append(DOTween
                .To(
                    () => _spriteRenderer.color,
                    value => _spriteRenderer.color = value,
                    WithAlpha(_visibleColor, 0f),
                    _exitDuration)
                .SetEase(_exitEase));
            sequence.Join(DOTween
                .To(
                    () => transform.localPosition,
                    value => transform.localPosition = value,
                    _visibleLocalPosition + _exitOffset,
                    _exitDuration)
                .SetEase(_exitEase));
            sequence.Join(DOTween
                .To(
                    () => transform.localScale,
                    value => transform.localScale = value,
                    GetHiddenScale(),
                    _exitDuration)
                .SetEase(_exitEase));
        }

        private void AppendEntrance(Sequence sequence)
        {
            sequence.Append(DOTween
                .To(
                    () => _spriteRenderer.color,
                    value => _spriteRenderer.color = value,
                    _visibleColor,
                    _enterDuration)
                .SetEase(_enterEase));
            sequence.Join(DOTween
                .To(
                    () => transform.localPosition,
                    value => transform.localPosition = value,
                    _visibleLocalPosition,
                    _enterDuration)
                .SetEase(_enterEase));
            sequence.Join(DOTween
                .To(
                    () => transform.localScale,
                    value => transform.localScale = value,
                    _visibleLocalScale,
                    _enterDuration)
                .SetEase(_enterEase));
        }

        private void StartSequence(Sequence sequence)
        {
            sequence.SetUpdate(_useUnscaledTime);
            sequence.OnComplete(CompleteTransition);
            _sequence = sequence;
        }

        private void CompleteTransition()
        {
            _sequence = null;
            _spriteRenderer.sprite = _boundSprite;
            ApplyVisibleState();
        }

        private void ApplyEntranceState()
        {
            _spriteRenderer.color = WithAlpha(_visibleColor, 0f);
            transform.localPosition = _visibleLocalPosition + _entranceOffset;
            transform.localScale = GetHiddenScale();
        }

        private void ApplyVisibleState()
        {
            _spriteRenderer.color = _visibleColor;
            transform.localPosition = _visibleLocalPosition;
            transform.localScale = _visibleLocalScale;
        }

        private Vector3 GetHiddenScale()
        {
            return new Vector3(
                _visibleLocalScale.x * _hiddenScale,
                _visibleLocalScale.y * _hiddenScale,
                _visibleLocalScale.z);
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            ResolveSpriteRenderer();
            if (_spriteRenderer == null)
            {
                throw new MissingComponentException(
                    $"World visitor view '{name}' requires a SpriteRenderer.");
            }

            _visibleLocalPosition = transform.localPosition;
            _visibleLocalScale = transform.localScale;
            _visibleColor = _spriteRenderer.color;
            _isInitialized = true;
        }

        private void ResolveSpriteRenderer()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
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

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
