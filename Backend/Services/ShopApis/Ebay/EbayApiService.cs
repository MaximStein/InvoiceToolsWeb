using Backend.Entities;

namespace Backend.Services.ShopApis.Ebay
{
    public class EbayApiService : ShopApiServiceBase
    {
        private string _clientId;

        public EbayApiService(ApiCredentialsConfig apiConfig, IHttpClientFactory httpClientFactory, ShopService shopService, string userId) : base(apiConfig, httpClientFactory, shopService, userId)
        {
            _clientId = apiConfig.EbayClientId;
        }

        public override string GetApiEndpointUrl(string apiAction)
        {
            throw new NotImplementedException();
        }

        public override string? GetAuthorizationUrl(string state)
        {
            var scope = "https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.finances https://api.ebay.com/oauth/api_scope/sell.payment.dispute https://api.ebay.com/oauth/api_scope/commerce.identity.readonly https://api.ebay.com/oauth/api_scope/commerce.notification.subscription https://api.ebay.com/oauth/api_scope/commerce.notification.subscription.readonly";

            var url = "https://auth.ebay.com/oauth2/authorize?response_type=code&redirect_uri=" + GetReturnUrl() + "&scope=" + scope + "&client_id=" 
                + _clientId;

            return url;
        }

        public override string GetReturnUrl() => _clientId;

        public override Task<List<Order>> GetOrdersAsync(DateTime start, DateTime end, long offset, long limit)
        {
            throw new NotImplementedException();
        }

        public override Task<long> GetOrdersCountAsync(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ProcessAuthorizationResultAsync(string code, string state)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RefreshTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}
