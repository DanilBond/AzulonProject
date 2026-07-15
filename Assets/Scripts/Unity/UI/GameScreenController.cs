using System;
using Azulon.Domain.Market;
using Azulon.Domain.Quests;
using Azulon.Presentation.Gameplay;
using Azulon.Unity.Runtime;
using Azulon.Unity.UI.Views;
using UnityEngine;

namespace Azulon.Unity.UI
{
    [DisallowMultipleComponent]
    public sealed class GameScreenController : MonoBehaviour
    {
        [SerializeField] private GameRuntime _runtime;
        [SerializeField] private GameScreenView _view;

        private GameSessionPresenter _presenter;
        private GameContentViewCatalog _contentCatalog;

        private void Start()
        {
            if (_runtime == null)
            {
                Fail("Game screen has no GameRuntime assigned.");
                return;
            }

            if (_view == null)
            {
                Fail("Game screen has no GameScreenView assigned.");
                return;
            }

            if (!_view.TryValidateReferences(out var validationError))
            {
                Fail(validationError);
                return;
            }

            if (!_runtime.TryInitialize())
            {
                Fail(_runtime.InitializationError);
                return;
            }

            try
            {
                _presenter = _runtime.Presenter;
                _contentCatalog = new GameContentViewCatalog(_runtime.Configuration);
                _view.Initialize(
                    HandlePurchaseRequested,
                    HandleClaimRequested,
                    HandleNextVisitorRequested);
                _view.Render(_presenter.CreateViewData(), _contentCatalog);
            }
            catch (Exception exception)
            {
                Fail(exception.Message, exception);
            }
        }

        private void OnDestroy()
        {
            if (_view != null)
            {
                _view.Release();
            }
        }

        private void HandlePurchaseRequested(MarketOfferId offerId)
        {
            ApplyAction(_presenter.PurchaseOffer(offerId));
        }

        private void HandleClaimRequested(QuestId questId)
        {
            ApplyAction(_presenter.ClaimQuest(questId));
        }

        private void HandleNextVisitorRequested()
        {
            ApplyAction(_presenter.AdvanceToNextVisitor());
        }

        private void ApplyAction(GameActionResult result)
        {
            try
            {
                var viewData = _presenter.CreateViewData();
                _view.Render(viewData, _contentCatalog);
                _view.ShowFeedback(GameFeedbackFormatter.Format(result, viewData));
            }
            catch (Exception exception)
            {
                Fail(exception.Message, exception);
            }
        }

        private void Fail(string message, Exception exception = null)
        {
            var resolvedMessage = string.IsNullOrWhiteSpace(message)
                ? "Unknown game screen initialization error."
                : message;
            Debug.LogError($"Game screen failed: {resolvedMessage}", this);
            if (exception != null)
            {
                Debug.LogException(exception, this);
            }

            if (_view != null)
            {
                _view.ShowFatalError(resolvedMessage);
            }

            enabled = false;
        }
    }
}
