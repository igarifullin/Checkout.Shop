using Shop.Domain.CartAggregate;

namespace Shop.Web.ViewModels
{
    public class CustomerCartItemViewModel
    {
        public int CatalogItemId { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }

        public decimal UnitPrice { get; set; }

        public static CustomerCartItemViewModel FromEntity(CartItem entity) => new CustomerCartItemViewModel
        {
            CatalogItemId = entity.CatalogItem.Id,
            Quantity = entity.Quantity,
            PictureUrl = entity.CatalogItem.PictureUrl,
            UnitPrice = entity.CatalogItem.Price
        };
    }
}