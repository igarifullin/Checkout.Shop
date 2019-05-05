namespace Shop.Domain.Dto
{
    public class CustomerCartItemDto
    {
        public int CatalogItemId { get; set; }

        public int Quantity { get; set; }

        public CustomerCartItemDto()
        {
        }

        public CustomerCartItemDto(int catalogItemId, int quantity)
        {
            this.CatalogItemId = catalogItemId;
            this.Quantity = quantity;
        }
    }
}