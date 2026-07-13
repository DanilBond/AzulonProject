namespace Azulon.Domain.Market
{
    public interface IMarketOfferIdSource
    {
        MarketOfferId NextId();
    }
}
