using Backend.Entities;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

using System.Net;
using System.Web;
using Backend.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services.ShopApis
{
    public abstract class ShopApiServiceBase
    {

        
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ApiCredentialsConfig _config;
        protected readonly ShopService _shopService;
        protected readonly string _userId;


        private Shop _shop;
        public Shop Shop { set => _shop = value; get => _shop; }

        public ShopApiServiceBase(ApiCredentialsConfig apiConfig, IHttpClientFactory httpClientFactory, ShopService shopService, string userId)
        {
            _httpClientFactory = httpClientFactory;
            _config = apiConfig;
            _shopService = shopService;
            _userId = userId;
            
        }

        protected HttpResponseMessage GetHttpResponse(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, url));

            return response;
        }

        public abstract Task<bool> ProcessAuthorizationResultAsync(string code, string state);

        public abstract string? GetAuthorizationUrl(string state);

        public string GetReturnUrl() => "https://localhost:4200/shops";

        public string GetOAuth2CodeVerifier(string state) => "vvkdljkejllufrvbhgeiegrnvufrhvrffnkvcknjvfid";

        public string GetSha256CodeChallenge(string state)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(GetOAuth2CodeVerifier(state)));
                var code_challenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
                return code_challenge;
            }
        }

        public abstract Task<bool> RefreshTokenAsync();

        public abstract Task<long> GetOrdersCountAsync(DateTime start, DateTime end);

        public abstract Task<List<Order>> GetOrdersAsync(DateTime start, DateTime end, long offset, long limit);

        public abstract string GetApiEndpointUrl(string apiAction);

        public async Task<ImportResult> ImportOrdersAsync(DateTime start, DateTime end, bool updateExisting = true)
        {
            var result = new ImportResult { Successful = true, Time = DateTime.Now, OrdersCreated = 0, OrdersUpdated = 0 };
            var totalOrdersCount = await GetOrdersCountAsync(start, end);
            var remainingOrdersCount = totalOrdersCount;

            while (remainingOrdersCount > 0)
            {

                var orders = await GetOrdersAsync(start, end, totalOrdersCount - remainingOrdersCount, 50);

                if (orders == null)
                {
                    result.Successful = false;
                    return result;
                }

                foreach(Order o in orders )
                {
                    o.ShopId = Shop.Id;

                    var existing = await _shopService.GetOrderAsync(Shop.Id, o.OrderNumber);
                    if (existing != null)
                    {
                        if (updateExisting && await TryUpdateAsync(existing, o))
                        {
                            result.OrdersUpdated++;
                        }
                    }
                    else
                    {
                        await _shopService.InsertOrderAsync(o);

                        result.OrdersCreated++;
                    }

                }

                remainingOrdersCount -= orders.Count;
            }

            return result;

        }

        private async Task<bool> TryUpdateAsync(Order existing, Order newOrder)
        {
            bool hasChange = 
                   existing.IsPaid != newOrder.IsPaid
                || existing.IsCancelled != newOrder.IsCancelled
                || (Nullable.Compare<DateTime>(newOrder.TimeShipped, newOrder.TimeShipped) != 0)
                || existing.TotalFees != newOrder.TotalFees;

            if (!hasChange)
                return false;

            existing.IsPaid = newOrder.IsPaid;
            existing.IsCancelled = newOrder.IsCancelled;
            existing.TimeShipped = newOrder.TimeShipped;
            existing.TotalFees = newOrder.TotalFees;

            await _shopService.UpdateOrderAsync(existing);

            return true;
        }


    }
}
