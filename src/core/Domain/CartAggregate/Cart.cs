using System.Collections.Generic;
using System.Linq;

namespace Shop.Domain.CartAggregate
{
    public class Cart
    {
        public string Id { get; set; }

        private List<CartItem> _items = new List<CartItem>();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public void AddItem(CatalogItem catalogItem, int quantity)
        {
            CartItem cartItem = _items
                .Where(g => g.CatalogItem.Id == catalogItem.Id)
                .FirstOrDefault();

            if (cartItem == null)
            {
                _items.Add(new CartItem
                {
                    CatalogItem = catalogItem,
                    Quantity = quantity
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
        }

        public void RemoveItem(CatalogItem catalogItem)
        {
            _items.RemoveAll(l => l.CatalogItem.Id == catalogItem.Id);
        }

        public decimal Total()
        {
            return _items.Sum(e => e.CatalogItem.Price * e.Quantity);

        }
        public void Clear()
        {
            _items.Clear();
        }
    }
}