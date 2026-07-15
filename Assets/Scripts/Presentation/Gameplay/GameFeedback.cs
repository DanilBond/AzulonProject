using System;

namespace Azulon.Presentation.Gameplay
{
    public sealed class GameFeedback
    {
        public GameFeedback(string message, GameFeedbackTone tone)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Feedback message cannot be empty.", nameof(message));
            }

            Message = message;
            Tone = tone;
        }

        public string Message { get; }

        public GameFeedbackTone Tone { get; }
    }
}
