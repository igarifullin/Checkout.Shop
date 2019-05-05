namespace Shop.Domain.CartAggregate
{
    public class CartItem
    {
        public CatalogItem CatalogItem { get; set; }

        public int Quantity { get; set; }
    }
}