using Microsoft.Extensions.Caching.Memory;
using Shop.Domain.CartAggregate;
using Shop.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Shop.DataAccess.Repositories
{
    public class MemoryCartRepository : ICartRepository
    {
        private readonly IMemoryCache _cache;

        public MemoryCartRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<Cart> GetCartAsync(string id)
        {
            return Task.FromResult(_cache.Get<Cart>(id));
        }

        public async Task<Cart> PutCartAsync(Cart cart)
        {
            Cart cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(cart.Id, out cacheEntry))
            {
                cacheEntry = await UpdateCartAsync(cart).ConfigureAwait(false);
            }
            
            return cart;
        }

        public Task<Cart> UpdateCartAsync(Cart cart)
        {
            Cart cacheEntry = cart;

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromDays(7));

            // Save data in cache.
            _cache.Set(cart.Id, cacheEntry, cacheEntryOptions);

            return Task.FromResult(cacheEntry);
        }

        public Task DeleteAsync(string id)
        {
            _cache.Remove(id);
            return Task.CompletedTask;
        }
    }
}