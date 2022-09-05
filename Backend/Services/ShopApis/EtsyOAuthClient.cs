using OAuth2.Client;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;

namespace Backend.Services.ShopApis
{
    public class EtsyOAuthClient : OAuth2Client
    {
        public EtsyOAuthClient(IRequestFactory factory, IClientConfiguration configuration) : base(factory, configuration)
        {
        }

        public override string Name => throw new NotImplementedException();

      

        protected override OAuth2.Client.Endpoint AccessTokenServiceEndpoint => new OAuth2.Client.Endpoint()
        {
            BaseUri = "https://www.etsy.com",
            Resource = "/oauth/connect"
        };

        protected override OAuth2.Client.Endpoint UserInfoServiceEndpoint => throw new NotImplementedException();

        protected override OAuth2.Client.Endpoint AccessCodeServiceEndpoint => new OAuth2.Client.Endpoint()
        {
            BaseUri = "https://www.etsy.com",
            Resource = "/oauth/connect"
        };


        protected override UserInfo ParseUserInfo(string content)
        {
            throw new NotImplementedException();
        }
    }
}
