using Backend.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Services
{
    public class UserService
    {
        private readonly IMongoCollection<ApplicationUser> _usersCollection;
        private readonly IMongoCollection<Order> _ordersCollection;


        public UserService(IOptions<MongoDbConfig> dbConfig)
        {
            var mongoClient = new MongoClient(
           dbConfig.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                dbConfig.Value.DatabaseName);

            _ordersCollection = mongoDatabase.GetCollection<Order>("Orders");

            _usersCollection = mongoDatabase.GetCollection<ApplicationUser>("Users");
        }


        public async Task<List<ApplicationUser>> GetAsync() =>
         await _usersCollection.Find(_ => true).ToListAsync();

        public ApplicationUser Get(string userId) {
            return _usersCollection.Find(x => x.Id == new ObjectId(userId)).FirstOrDefault();
        }

        public async Task<ApplicationUser?> GetAsync(string userId) =>
            await _usersCollection.Find(x => x.Id == new ObjectId(userId)).FirstOrDefaultAsync();

        public async Task CreateAsync(ApplicationUser newProfile) =>
            await _usersCollection.InsertOneAsync(newProfile);

        public async Task UpdateAsync(string userId, ApplicationUser updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == new ObjectId(userId), updatedUser);

        public async Task UpdateAsync(ApplicationUser updatedUser) =>
         await _usersCollection.ReplaceOneAsync(x => x.Id == updatedUser.Id, updatedUser);


        public async Task RemoveAsync(string userId) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == new ObjectId(userId));



    }
}
