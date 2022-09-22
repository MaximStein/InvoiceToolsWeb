using Backend.Entities;
using Microsoft.Extensions.Options;
using Backend.Enums;
using Backend.Services.ShopApis.Etsy;
using Backend.Services.ShopApis.Ebay;

namespace Backend.Services.ShopApis
{
    public class ShopApiProvider
    {
        //private Dictionary<ShopApiType, ShopApiServiceBase> _apiServices = new Dictionary<ShopApiType, ShopApiServiceBase>();
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiCredentialsConfig _config;
        private readonly ShopService _shopService;
        
  
        public ShopApiProvider(IOptions<ApiCredentialsConfig> apiConfig, IHttpClientFactory httpClientFactory, ShopService shopService)
        {
            _config = apiConfig.Value;
            _httpClientFactory = httpClientFactory;
            _shopService = shopService;

            //_apiServices.Add(ShopApiType.Etsy, new EtsyApiService(dbConfig, httpClientFactory));
            //_apiServices.Add(ShopApiType.MockShop, new MockShopService(dbConfig, httpClientFactory));

        }

        private ShopApiServiceBase GetNewApiService(ShopApiType type, string _userId)
        {
            switch(type)
            {
                case ShopApiType.Etsy:
                    return new EtsyApiService(_config, _httpClientFactory, _shopService, _userId);
                case ShopApiType.Ebay:
                    return new EbayApiService(_config, _httpClientFactory, _shopService, _userId);
                case ShopApiType.DemoTestShop:
                    return new MockShopService(_config, _httpClientFactory, _shopService, _userId);
                default:
                    return null;
            }
        }

        public ShopApiServiceBase GetApiService(ShopApiType type, string userId)
        {
            return GetNewApiService(type, userId);            
        }

        public ShopApiServiceBase GetShopAssociatedApiService(Shop shop, string userId)
        {
            var api = GetNewApiService(shop.OAuthAccess.ShopApiType, userId);
            api.Shop = shop;
            return api;
        }

        public ShopApiServiceBase GetApiServiceByReferrer(String referrer, string userId)
        {
            if (referrer.Contains("etsy.com"))
                return GetNewApiService(ShopApiType.Etsy, userId);
            else
                return GetNewApiService(ShopApiType.DemoTestShop, userId);
        }
    }
}
