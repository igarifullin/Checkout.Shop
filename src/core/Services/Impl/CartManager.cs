using System.Linq;
using System.Threading.Tasks;
using Shop.Domain;
using Shop.Domain.CartAggregate;
using Shop.Domain.Dto;
using Shop.Domain.Repositories;
using Shop.Utils.Extensions;

namespace Shop.Services.Impl
{
    public class CartManager : ICartManager
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICatalogItemRepository _catalogItemRepository;

        public CartManager(ICartRepository cartRepository,
            ICatalogItemRepository catalogItemRepository)
        {
            _cartRepository = cartRepository;
            _catalogItemRepository = catalogItemRepository;
        }

        public async Task<Cart> GetCartAsync(string id)
        {
            var cart = await _cartRepository.GetCartAsync(id);
            if (cart == null)
            {
                cart = await _cartRepository.PutCartAsync(new Cart { Id = id });
            }

            return cart;
        }

        public async Task<Cart> UpdateCartAsync(CustomerCartDto customerCart)
        {
            var cart = await GetCartAsync(customerCart.Id);
            cart.Clear();

            await customerCart.Items.ForEachAsync(async x =>
             {
                 CatalogItem catalogItem = await _catalogItemRepository.GetCatalogItemAsync(x.CatalogItemId);
                 if (catalogItem != null)
                 {
                     cart.AddItem(catalogItem, x.Quantity);
                 }
             });

            return await _cartRepository.UpdateCartAsync(cart);
        }

        public Task DeleteCartAsync(string id)
        {
            return _cartRepository.DeleteAsync(id);
        }
    }
}