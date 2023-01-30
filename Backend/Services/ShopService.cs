using Backend.Entities;
using Backend.Entities.DataTransferObjects;
using Backend.Services.DTOs;
using Backend.Services.ShopApis;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Services
{
    public class ShopService
    {

        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly UserService _userService;

        public ShopService(IOptions<MongoDbConfig> dbConfig, UserService userService)
        {
            var mongoClient = new MongoClient(
           dbConfig.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbConfig.Value.DatabaseName);

            _ordersCollection = mongoDatabase.GetCollection<Order>("Orders");
            _userService = userService;
        }

        public async Task<List<Shop>> GetShopsAsync(string userId)
        {
            var shops = _userService.Get(userId).Shops;
            return shops == null ? new List<Shop>() : shops;
        }

        public async Task<Shop> GetShopAsync(string userId, string shopId)
        {
            var user = await _userService.GetAsync(userId);

            if (user.Shops == null)
                return null;
            return user.Shops.Find(s => s.Id == shopId);            
        }

        public async Task CreateShopAsync(string userId, Shop shopToInsert)
        {
            shopToInsert.Id = ObjectId.GenerateNewId().ToString();
            var user = await _userService.GetAsync(userId);

            user.Shops.Add(shopToInsert);
            await _userService.UpdateAsync(user);
        }

        public async Task RemoveShopAsync(string userId, string shopId)
        {
            var deleteResult = await _ordersCollection.DeleteManyAsync(order => order.ShopId == shopId);

            var user = await _userService.GetAsync(userId);
            user.Shops.RemoveAll(p => p.Id == shopId);

            await _userService.UpdateAsync(user);
        }

        public async Task UpdateShopAsync(string userId, Shop updatedShop)
        {
            var user = await _userService.GetAsync(userId);
            var shopIndex = user.Shops.FindIndex(s => s.Id == updatedShop.Id);
            user.Shops[shopIndex] = updatedShop;
            await _userService.UpdateAsync(user);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _ordersCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
        }

        public async Task InsertOrderAsync(Order order)
        {
            await _ordersCollection.InsertOneAsync(order);            
        }

        public async Task<List<Order>> GetOrdersAsync(string[] ids)
        {
            var filter = Builders<Order>.Filter.In(o => o.Id, ids);
            var sort = Builders<Order>.Sort.Ascending(o => o.TimeOrdered);
            var result =  await (_ordersCollection.FindAsync(filter, new FindOptions<Order> {  Sort=sort}));
            return await result.ToListAsync();
        }

        public async Task<Order> GetOrderAsync(string shopId, string orderNumber)
        {
            return await _ordersCollection.Find(o => o.OrderNumber == orderNumber && o.ShopId == shopId)
                .FirstOrDefaultAsync();
        }

        public bool IsInvoiceNumberExisting(IEnumerable<string> shopIds, long invoiceNumber)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.InvoiceNumber, invoiceNumber) & Builders<Order>.Filter.In(o => o.ShopId, shopIds);
            return _ordersCollection.CountDocuments(filter) > 0;
            
        }

        public async Task<long> GetOrdersCountAsync(OrdersFilter f)
        {
            return await _ordersCollection.CountDocumentsAsync(GetOrdersFilterDefinition(f));
        }

        public long GetOrdersCount(string shopId)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.ShopId, shopId); 
            return _ordersCollection.CountDocuments(filter);
            //return _ordersCollection.Count
        }

        public async Task<List<Order>> GetOrdersPageAsync(OrdersFilter f, int pageIndex, int pageSize)
        {
            var result = _ordersCollection.Find(GetOrdersFilterDefinition(f)).SortByDescending(x => x.TimeOrdered);            
            return await result.Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync();
        }

        public async Task ClearLastImport(string userId, string shopId)
        {
            var shop = await GetShopAsync(userId, shopId);
            shop.LastImport = null;
            await UpdateShopAsync(userId, shop);
        }

        public async Task DeleteOrdersAsync(OrdersFilter f)
        {
            await _ordersCollection.DeleteManyAsync(GetOrdersFilterDefinition(f));
        }

        public async Task DeleteOrdersAsync(IEnumerable<string> orderIds)
        {
           await _ordersCollection.DeleteManyAsync(o => orderIds.Contains(o.Id));
        }

        private FilterDefinition<Order> GetOrdersFilterDefinition(OrdersFilter f)
        {
            var builder = Builders<Order>.Filter;

            var filter = builder.Empty;

            if(f.HasInvoice != null)
            {
                filter &= f.HasInvoice.Value ? builder.Not(builder.Eq(o => o.InvoiceNumber, null)) : builder.Eq(o => o.InvoiceNumber, null);
            }

            if(f.IsPaid != null)
            {
                filter &= builder.Eq(o => o.IsPaid, f.IsPaid.Value);
            }

            if(f.MaxDateCreated != null)
            {
                filter &= builder.Lte(o => o.TimeOrdered, f.MaxDateCreated.Value.AddDays(1).AddSeconds(-1));
            }

            if (f.MinDateCreated != null)
            {
                filter &= builder.Gte(o => o.TimeOrdered, f.MinDateCreated.Value);
            }


            if (f.ShopIds != null && f.ShopIds.Length>0)
            {
                var filter2 = Builders<Order>.Filter.Eq(x => x.ShopId, f.ShopIds.Count() == 0 ? "" : f.ShopIds[0]);

                foreach (var shopId in f.ShopIds)
                {
                    filter2 |= Builders<Order>.Filter.Eq(x => x.ShopId, shopId);
                }

                filter &= filter2;
            }

            return filter;
        }

    }
}
