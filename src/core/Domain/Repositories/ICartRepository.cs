using Shop.Domain.CartAggregate;
using System.Threading.Tasks;

namespace Shop.Domain.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAsync(string id);

        Task<Cart> PutCartAsync(Cart cart);

        Task<Cart> UpdateCartAsync(Cart cart);

        Task DeleteAsync(string id);
    }
}