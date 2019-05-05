using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Domain.CartAggregate;
using Shop.Domain.Repositories;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Shop.DataAccess.Repositories
{
    public class RedisCartRepository : ICartRepository
    {
        private readonly ILogger<RedisCartRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCartRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisCartRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<Cart> GetCartAsync(string id)
        {
            var data = await _database.StringGetAsync(id);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Cart>(data);
        }

        public async Task<Cart> PutCartAsync(Cart cart)
        {
            var created = await _database.StringSetAsync(cart.Id, JsonConvert.SerializeObject(cart));

            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Cart item persisted succesfully.");

            return await GetCartAsync(cart.Id);
        }

        public Task<Cart> UpdateCartAsync(Cart cart)
        {
            return PutCartAsync(cart);
        }

        public Task DeleteAsync(string id)
        {
            return _database.KeyDeleteAsync(id);
        }
    }
}