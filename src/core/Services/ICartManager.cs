using Shop.Domain.CartAggregate;
using Shop.Domain.Dto;
using System.Threading.Tasks;

namespace Shop.Services
{
    public interface ICartManager
    {
        Task<Cart> GetCartAsync(string id);

        Task<Cart> UpdateCartAsync(CustomerCartDto customerCart);

        Task DeleteCartAsync(string id);
    }
}