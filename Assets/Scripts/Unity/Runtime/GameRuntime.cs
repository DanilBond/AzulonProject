using System;
using Azulon.Configuration.Game;
using Azulon.Domain.Market;
using Azulon.Domain.Randomness;
using Azulon.Presentation.Gameplay;
using UnityEngine;

namespace Azulon.Unity.Runtime
{
    [DisallowMultipleComponent]
    public sealed class GameRuntime : MonoBehaviour
    {
        [SerializeField] private GameSessionConfigAsset _configuration;
        [SerializeField] private bool _useFixedRandomSeed = true;
        [SerializeField] private int _randomSeed = 1729;

        public event Action<GameSessionPresenter> Initialized;

        public GameSessionConfigAsset Configuration => _configuration;

        public GameSessionPresenter Presenter { get; private set; }

        public bool IsInitialized => Presenter != null;

        public string InitializationError { get; private set; }

        private void Awake()
        {
            TryInitialize();
        }

        public bool TryInitialize()
        {
            if (IsInitialized)
            {
                return true;
            }

            if (_configuration == null)
            {
                return FailInitialization("Game session configuration is not assigned.");
            }

            try
            {
                IRandomSource randomSource = _useFixedRandomSeed
                    ? new SystemRandomSource(_randomSeed)
                    : new SystemRandomSource();
                var session = GameSessionFactory.Create(
                    _configuration,
                    randomSource,
                    new SequentialMarketOfferIdSource());

                Presenter = new GameSessionPresenter(session);
                InitializationError = null;
                Initialized?.Invoke(Presenter);

                var viewData = Presenter.CreateViewData();
                Debug.Log(
                    $"Guild Relic Market initialized: day {viewData.DayNumber}, " +
                    $"visitor {viewData.VisitorNumber}, {viewData.Offers.Count} offer(s).",
                    this);
                return true;
            }
            catch (Exception exception)
            {
                InitializationError = exception.Message;
                Debug.LogError(
                    $"Guild Relic Market initialization failed: {exception.Message}",
                    this);
                Debug.LogException(exception, this);
                return false;
            }
        }

        private bool FailInitialization(string message)
        {
            InitializationError = message;
            Debug.LogError($"Guild Relic Market initialization failed: {message}", this);
            return false;
        }
    }
}
