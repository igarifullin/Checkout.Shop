using Shop.Client.Responses;
using System.Threading.Tasks;

namespace Shop.Client
{
    public interface ICartClient
    {
        Task<CartResponse> CreateCartAsync(string id);

        Task<CartResponse> GetCartAsync(string id);

        Task<CartResponse> AddProductAsync(string id, int catalogItemId, int quantity);

        Task<CartResponse> RemoveProductAsync(string id, int catalogItemId);

        Task<CartResponse> SetProductsQuantityAsync(string id, int catalogItemId, int quantity);

        Task ClearCartAsync(string id);
    }
}