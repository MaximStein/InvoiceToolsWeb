using Backend.Entities;
using Microsoft.Extensions.Options;
using System.Web;

namespace Backend.Services.ShopApis
{
    public class MockShopService : ShopApiServiceBase
    {

        private static readonly Address[] _addresses = new[] {
            new Address { Name = "Max Mustermann", City = "Musterstadt",
                Country = "Deutschland", Street = "Musterstraße 1", ZipCode ="1234"
            }
        };

        private static readonly Person[] _samplePersons = new[]
        {
            new Person() { Email = "alfred12345@alphamail.de", Name = "Alfred Altmann", UserAccountName = "altmann12345" },
            new Person() { Email = "bbengel@betamail.com", Name = "Bernd Bengel", UserAccountName = "bengel123" },
            new Person() { Email = "claudio@gammamail.net", Name = "Claudio Corn", UserAccountName = "x123y1231z" },
        };

        private static readonly LineItem[] _sampleItems = new[]
        {
            new LineItem { Title = "Schlüsselanhänger mit Beschriftung", Price = 5, Quantity = 1 },
            new LineItem { Title = "Flaschenöffner / Korkenzieher mit persönlicher Gravur - Holz Wunschgravur - personalisiert", Price = 22.99, Quantity = 1 },
            new LineItem { Title = "Namensanhänger aus Holz - Schlüsselanhänger mit Gravur nach Wunsch - Nussbaum", Price = 6.99, Quantity = 1 },
            new LineItem { Title = "Plexiglas LED Aufsteller mit Schriftzug nach Wunsch- personalisierte Leuchttafel", Price = 34.99, Quantity = 1 },
        };

        private static readonly string[] _stringAddresses = new[]
        {
            @"Max Mustermmann
Musterstraße 2
12345 Musterstadt
Deutschland",
            @"Erika Musterfrau
Hauptstraße 5
2356 Linz
Österreich"
        };

        private readonly List<Order> _orders = new List<Order>();

        private static Order GetRandomizedOrder(int seed = 0)
        {
            Random random = new Random(seed);
            Random r2 = new Random();

            var addr = _stringAddresses[random.Next(2)];
            var order = new Order()
            {
                BillingsAddress = addr,
                ShippingAddress = addr,
                Items = new List<LineItem>(),
                Buyer = _samplePersons[random.Next(0,_samplePersons.Length)],
                TimeOrdered = DateTime.Now.AddDays(- (random.Next() % 60)),
                OrderNumber = "" + random.Next(),                
                CurrencyCode = Enums.CurrencyCode.EUR,
                IsPaid = Convert.ToBoolean(random.Next(4))
            };

            if(!order.IsPaid.Value)
            {
                var val = r2.Next(5);
                if (val == 0) 
                    order.IsPaid = true;
            }

            var limit = random.Next(1, _sampleItems.Length+1);
            for(int i = 0; i < limit; i++)
            {
                var sampleItem = new LineItem { Title = _sampleItems[i].Title, Price = _sampleItems[i].Price, Quantity = random.Next(1, 5) };
                order.Items.Add(sampleItem);
            }


            if (order.IsPaid.Value)                            
                order.TimeShipped = order.TimeOrdered.AddHours(random.Next() % 720);            
            else
                order.TimeShipped = null;


            return order;
        }



        public MockShopService(ApiCredentialsConfig apiConfig, IHttpClientFactory httpClientFactory, ShopService shopService, string userId) : base(apiConfig, httpClientFactory, shopService, userId)
        {
            _orders = new List<Order>();
            for (int i = 0; i < 100; i++)
            {
                _orders.Add(GetRandomizedOrder(i));
            }
        }

        public override async Task<List<Order>> GetOrdersAsync(DateTime start, DateTime end, long offset, long limit)
        {
             var orders = _orders.Where(o => o.TimeOrdered >= start && o.TimeOrdered <= end).ToList();        
              var result = orders.Skip((int)offset).Take((int)limit).ToList();
            return result;
        }

        public override async Task<long> GetOrdersCountAsync(DateTime start, DateTime end)
        {          
            return _orders.Where(o => o.TimeOrdered >= start && o.TimeOrdered <= end).Count();
        }

        public override async Task<bool> ProcessAuthorizationResultAsync(string code, string state)
        {
            Shop.OAuthAccess = new OAuthAccountAccess
            {
                ShopApiType = Enums.ShopApiType.DemoTestShop
            };

            return true;
        }

        public override string GetAuthorizationUrl(string state)
        {
            return "/shops?return-url="+ HttpUtility.UrlEncode(state)+"&mock-authorization=1";
        }

        public override Task<bool> RefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public override string GetApiEndpointUrl(string apiAction)
        {
            throw new NotImplementedException();
        }

    }
}
