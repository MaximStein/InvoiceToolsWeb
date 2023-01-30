using Backend.Entities;
using Backend.Entities.DataTransferObjects;
using Backend.Enums;
using Backend.Services.ShopApis.Etsy.Types;
using Backend.Utils;
using DotNetAuth.Profiles;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using OAuth2.Client.Impl;
using OAuth2.Infrastructure;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Backend.Services.ShopApis.Etsy
{

    public class EtsyApiService : ShopApiServiceBase
    {


        private readonly string _apiKey;

        public EtsyApiService(ApiCredentialsConfig apiConfig, IHttpClientFactory httpClientFactory, ShopService shopService, string userId) : 
            base(apiConfig, httpClientFactory, shopService, userId) {
            _apiKey = apiConfig.EtsyApiKey;
        }

        public override async Task<bool> ProcessAuthorizationResultAsync(string code, string state)
        {

            var client = _httpClientFactory.CreateClient();
            var url = "https://api.etsy.com/v3/public/oauth/token";

            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "authorization_code");
            dict.Add("client_id", _apiKey);
            dict.Add("redirect_uri", GetReturnUrl());
            dict.Add("code", code);
            dict.Add("code_verifier", GetOAuth2CodeVerifier(state));

            var msg = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };

            var res = await client.SendAsync(msg);

            if (res.StatusCode == HttpStatusCode.OK)
            {
                var json = await res.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponseDto>(json);

                Shop.OAuthAccess = new OAuthAccountAccess
                {
                    AccessToken = tokenResponse.access_token,
                    RefreshToken = tokenResponse.refresh_token,
                    AccessTokenExpiration = DateTime.Now.AddSeconds(tokenResponse.expires_in),
                    ShopApiType = ShopApiType.Etsy,
                    
                };

                await _shopService.UpdateShopAsync(_userId, Shop);

                return true;

            }

            return false;
        }


        public override string GetAuthorizationUrl(string shopId)
        {
            var scope = "transactions_r%20transactions_w%20shops_r%20profile_r";

            var url = "https://www.etsy.com/oauth/connect?response_type=code&redirect_uri=" + GetReturnUrl() + "&scope=" + scope + "&client_id=" + _apiKey
                + "&code_challenge=" + GetSha256CodeChallenge(shopId) + "&code_challenge_method=S256&state=" + shopId;

            return url;

        }


        public override async Task<List<Order>> GetOrdersAsync(DateTime start, DateTime end, long offset, long limit)
        {
            var shop = await GetShopAsync();

            var endpoint = "application/shops/" + shop.shop_id + "/receipts?"+GetFilterQueryString(start,end,offset,limit);
            var result = await SendAuthorizedGetRequestAsync(endpoint);

            if (result == null)
                return null;
            
            var receipts = JsonConvert.DeserializeObject<Types.ListResult<Receipt>>(result);

            return receipts.results.Select(r => ConvertReceiptToOrder(r)).ToList();
            
            //var response = GetHttpResponse(url);

            //var request = GetNewAuthorizedRequest("application/shops/{shop_id}/receipts")
        }

        private string GetFilterQueryString(DateTime start, DateTime end, long offset, long limit)
        {
            return "min_created="
                + ((DateTimeOffset)start).ToUnixTimeSeconds() + "&max_created=" + ((DateTimeOffset)end).ToUnixTimeSeconds()
                + "&limit=" + limit + "&offset=" + offset + "&language=de";
        }

        

        private Order ConvertReceiptToOrder(Receipt r)
        {
            var order = new Order()
            {

                ImportedFrom = ShopApiType.Etsy,
                OrderNumber = "" + r.receipt_id,
                Buyer = new Person { Email = r.buyer_email, UserAccountName = "" + r.buyer_user_id, Name = r.name },
                ShippingAddress = r.formatted_address,
                BillingsAddress = r.formatted_address,
                PaymentMethod = r.payment_method,
                TimeOrdered = TimeUtils.FromUnixTime(r.created_timestamp),
                IsPaid = r.is_paid,
                IsCancelled = r.status == "canceled",
                TimeShipped = r.shipments.Count > 0 ? TimeUtils.FromUnixTime(r.shipments[0].shipment_notification_timestamp) : null,
             //   VatPercent = r.total_vat_cost.amount,
                CurrencyCode = Enum.Parse<CurrencyCode>(r.grandtotal.currency_code),
                BuyerMessage = r.message_from_buyer,
                GiftOptions = r.is_gift ? new OrderGiftOptions { GiftMessage = r.gift_message, IsGift = true } : null,
                Items = r.transactions.Select(t =>
                    new LineItem
                    {
                        Title = t.title,                        
                        Quantity = (int)t.quantity,
                        Price = 1d*t.price.amount / t.price.divisor,
                        Variation = t.variations.Select(v => v.formatted_name+": "+v.formatted_value).Join("; ")
                    }).ToList()

            };

            if(r.total_shipping_cost.amount > 0)
            {
                order.Items.Add(new LineItem
                {
                    Title = "Verpackung & Versand",
                    Price = r.total_shipping_cost.amount / r.total_shipping_cost.divisor,
                    Quantity = 1
                });
            }

            return order;
            
        }

        private async Task<Types.Shop> GetShopAsync()
        {
            var endpoint = "application/users/" + GetUserId() + "/shops";
            var result = await SendAuthorizedGetRequestAsync(endpoint);

            var shop = JsonConvert.DeserializeObject<Types.Shop>(result);
            return shop;
        }

        public override async Task<long> GetOrdersCountAsync(DateTime start, DateTime end)
        {
            var shop = await GetShopAsync();

            var endpoint = "application/shops/" + shop.shop_id + "/receipts?" + GetFilterQueryString(start, end, 0, 0);
            var result = await SendAuthorizedGetRequestAsync(endpoint);

            if (result == null)
                return 0;
            
            var receipts = JsonConvert.DeserializeObject<Types.ListResult<Receipt>>(result);
            return receipts.count;
        }

        public override async Task<bool> RefreshTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();

            var url = "https://api.etsy.com/v3/public/oauth/token";

            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", "refresh_token");
            dict.Add("client_id", _apiKey);
            dict.Add("refresh_token", Shop.OAuthAccess!.RefreshToken!);

            var msg = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };

            var res = await client.SendAsync(msg);

            if (res.StatusCode == HttpStatusCode.OK)
            {
                var json = await res.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponseDto>(json);

                Shop.OAuthAccess.AccessToken = tokenResponse.access_token;
                Shop.OAuthAccess.RefreshToken = tokenResponse.refresh_token;
                Shop.OAuthAccess.AccessTokenExpiration = DateTime.Now.AddSeconds(tokenResponse.expires_in);

                await _shopService.UpdateShopAsync(_userId, Shop);
                return true;
            }

            return false;
        }

        public override string GetApiEndpointUrl(string apiAction) => "https://api.etsy.com/v3/" + apiAction;


        private HttpRequestMessage GetNewRequestMessage(string apiAction)
        {
            var url = GetApiEndpointUrl(apiAction);

            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("x-api-key", _apiKey);
            msg.Headers.Add("authorization", "Bearer " + Shop.OAuthAccess.AccessToken);
            
            return msg;
        }

        private async Task<string> SendAuthorizedGetRequestAsync(string apiAction)
        {
            var client = _httpClientFactory.CreateClient();

            

            var res = await client.SendAsync(GetNewRequestMessage(apiAction));

            if (res.StatusCode == HttpStatusCode.OK)
            {
                return await res.Content.ReadAsStringAsync();
            }

            else if(res.StatusCode == HttpStatusCode.Unauthorized)
            {
                await this.RefreshTokenAsync();
                res = await client.SendAsync(GetNewRequestMessage(apiAction));

                if(res.StatusCode == HttpStatusCode.OK)
                    return await res.Content.ReadAsStringAsync(); 
            }
            return null;
        }

        private string GetUserId()
        {
            return Shop.OAuthAccess.AccessToken.Split(".")[0];
        }
    }
}
