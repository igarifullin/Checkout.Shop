using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Domain.Repositories
{
    public interface ICatalogItemRepository
    {
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync();

        Task<CatalogItem> GetCatalogItemAsync(int catalogItemId);
    }
}