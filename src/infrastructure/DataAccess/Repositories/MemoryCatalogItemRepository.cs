using Shop.Domain;
using Shop.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.DataAccess.Repositories
{
    public class MemoryCatalogItemRepository : ICatalogItemRepository
    {
        private static IEnumerable<CatalogItem> _cachedItems = GetCache();

        public MemoryCatalogItemRepository()
        {
        }

        public Task<CatalogItem> GetCatalogItemAsync(int catalogItemId)
        {
            return Task.FromResult(_cachedItems.FirstOrDefault(x => x.Id == catalogItemId));
        }

        public Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync()
        {
            return Task.FromResult(_cachedItems);
        }

        private static IEnumerable<CatalogItem> GetCache()
        {
            yield return new CatalogItem
            {
                Id = 1,
                Name = "DVD game of thrones 1 season",
                Description = "Three king families are fighting for the throne. Could they do this? Who knows...",
                Price = 100.00m,
                PictureUrl = "https://www.hbo.com/content/dam/hbodata/series/game-of-thrones/episodes/1/1/winter-is-coming-1920.jpg/_jcr_content/renditions/cq5dam.web.1200.675.jpeg",
            };

            yield return new CatalogItem
            {
                Id = 2,
                Name = "iPhone X",
                Description = "Apple device",
                Price = 1500.00m,
                PictureUrl = "http://cdn.osxdaily.com/wp-content/uploads/2017/08/convert-photo-to-pdf-ios.jpg"
            };

            yield return new CatalogItem
            {
                Id = 3,
                Name = "MacBook Pro",
                Description = "Apple laptop",
                Price = 3000.00m,
                PictureUrl = "https://static.re-store.ru/upload/icons-tovar/macbook_pro/macbook@2x.png"
            };
        }
    }
}