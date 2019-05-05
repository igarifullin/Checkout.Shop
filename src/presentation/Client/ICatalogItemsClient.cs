using Shop.Client.Responses;
using System.Threading.Tasks;

namespace Shop.Client
{
    public interface ICatalogItemsClient
    {
        Task<CatalogItemsResponse> GetCatalogItemsAsync();
    }
}