namespace Shop.Client.Requests
{
    public class CartUpdateRequest
    {
        public string Id { get; set; }

        public CartItemRequest[] Items { get; set; }
    }

    public class CartItemRequest
    {
        public int Quantity { get; set; }

        public int CatalogItemId { get; set; }
    }
}