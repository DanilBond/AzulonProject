namespace Azulon.Application.Gameplay
{
    public sealed class VisitorAdvanceResult
    {
        internal VisitorAdvanceResult(
            bool startedNewDay,
            int dayNumber,
            int visitorNumber,
            int creditedCoins,
            int walletBalance)
        {
            StartedNewDay = startedNewDay;
            DayNumber = dayNumber;
            VisitorNumber = visitorNumber;
            CreditedCoins = creditedCoins;
            WalletBalance = walletBalance;
        }

        public bool StartedNewDay { get; }

        public int DayNumber { get; }

        public int VisitorNumber { get; }

        public int CreditedCoins { get; }

        public int WalletBalance { get; }
    }
}
