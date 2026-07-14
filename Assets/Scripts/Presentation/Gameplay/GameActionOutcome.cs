namespace Azulon.Presentation.Gameplay
{
    public enum GameActionOutcome
    {
        PurchaseSucceeded = 0,
        InsufficientFunds = 1,
        OfferAlreadyPurchased = 2,
        OfferUnavailable = 3,
        QuestClaimed = 4,
        QuestRequirementsNotMet = 5,
        QuestAlreadyClaimed = 6,
        QuestUnavailable = 7,
        VisitorAdvanced = 8,
        NewDayStarted = 9,
        SessionCompleted = 10
    }
}
